using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.WebSockets;
using Adin.Pusher.Api.Model;
using Adin.Pusher.Domain.Logger;
using Adin.Pusher.Domain.Utils;
using Newtonsoft.Json;

namespace Adin.Pusher.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class WebSocketController : ApiController
    {
        public HttpResponseMessage Get()
        {
            if (HttpContext.Current.IsWebSocketRequest)
            {
                HttpContext.Current.AcceptWebSocketRequest(AcceptMessage);
            }

            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }

        static readonly ConcurrentDictionary<Guid, WebSocket> _users = new ConcurrentDictionary<Guid, WebSocket>();

        static readonly ConcurrentDictionary<string, UserGroup> _userGroups = new ConcurrentDictionary<string, UserGroup>();

        private async Task AcceptMessage(AspNetWebSocketContext context)
        {
            WebSocket socket = context.WebSocket;

            var url = context.RequestUri;


            //            Visitor myUser = new Visitor { Url = url };
            Guid guid = Guid.NewGuid();
            _users.AddOrUpdate(guid, socket, (p, w) => socket);
            var myobject = JsonConvert.SerializeObject(new WebsocketData
            {
                Code = 0,
                Data = guid
            });
            var bufferData = new ArraySegment<byte>(Encoding.UTF8.GetBytes(myobject));
            await socket.SendAsync(bufferData, WebSocketMessageType.Text, true, CancellationToken.None);
            while (true)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
                WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                if (socket.State == WebSocketState.Open)
                {
                    string userMessage = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                    userMessage = "You sent: " + userMessage + " at " + DateTime.Now.ToLongTimeString();
                    buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(userMessage));
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    break;
                }
            }

            // when the connection ends, try to remove the user
            WebSocket ows;
            if (_users.TryRemove(guid, out ows))
            {
                if (ows != socket)
                {
                    // whops! user reconnected too fast and you are removing
                    // the new connection, put it back
                    _users.AddOrUpdate(guid, ows, (p, w) => ows);
                }
                else
                {
                    foreach (var userGroup in _userGroups)
                    {
                        if (userGroup.Value.Users.Contains(guid))
                        {
                            Guid og;
                            userGroup.Value.Users.TryTake(out og);
                        }
                    }
                }
            }
        }

        [HttpGet]
        public string SendMessageToAll(string message)
        {
            Log.Instance.Info("Connected Users:" + _users.Count);
            try
            {
                Parallel.ForEach(_users, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = 1000
                }, aspNetWebSocketContext =>
                {
                    try
                    {
                        if (aspNetWebSocketContext.Value != null)
                        {
                            WebSocket socket = aspNetWebSocketContext.Value;
                            if (socket != null)
                            {
                                if (socket.State == WebSocketState.Open)
                                {
                                    string userMessage = message;
                                    var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(userMessage));
                                    socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None).Wait(200);
                                }
                                else
                                {
                                    RemoveUser(aspNetWebSocketContext.Key);
                                }
                            }
                            else
                            {
                                RemoveUser(aspNetWebSocketContext.Key);
                            }
                        }
                        else
                        {
                            RemoveUser(aspNetWebSocketContext.Key);
                        }
                    }
                    catch (Exception)
                    {
                        RemoveUser(aspNetWebSocketContext.Key);
                    }
                });
            }
            catch (Exception exception)
            {
                Log.Instance.Info(exception, "Err On SendMessageToAll");
            }

            return "OK";
        }


        [HttpPost]
        public string SendMessage([FromBody]PushRequestModel model)
        {
            UserGroup userGroup;
            _userGroups.TryGetValue(model.GroupName, out userGroup);            
            if (userGroup != null)
            {
                Log.Instance.Info("GroupName:" + model.GroupName + "Connected Users:" + userGroup.Users.Count);
                try
                {
                    Parallel.ForEach(userGroup.Users, new ParallelOptions()
                    {
                        MaxDegreeOfParallelism = 1000
                    }, userId =>
                    {
                        try
                        {
                            var user = _users.FirstOrDefault(x => x.Key == userId);
                            if (user.Value != null)
                            {

                                if (user.Value.State == WebSocketState.Open)
                                {                                    
                                    var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(model.Message));
                                    user.Value.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None)
                                        .Wait(200);
                                }
                                else
                                {
                                    RemoveUser(user.Key);
                                }
                            }
                            else
                            {
                                RemoveUser(user.Key);
                            }

                        }
                        catch (Exception)
                        {
                            RemoveUser(userId);
                        }
                    });
                    return "Message Sent To " + userGroup.Users.Count;
                }
                catch (Exception exception)
                {
                    Log.Instance.Info(exception, "Err On SendMessageToAll");
                }
            }
            return "UserGroup Is Null";


        }

        [HttpGet]
        public int JoinGroup(Guid userId, string groupName)
        {
            Log.Instance.Info("group Info: " + groupName);

            var userGroup = _userGroups.GetOrAdd(groupName, new UserGroup()
            {
                Users = new ConcurrentBag<Guid>()
            });
            if (!userGroup.Users.Contains(userId))
            {
                userGroup.Users.Add(userId);
                Log.Instance.Info(userId + " Added To " + groupName);

            }
            return 1;
        }

        [HttpPost]
        public int JoinGroups([FromBody]JoinData data)
        {
            Log.Instance.Error("groups: " + data.GroupNames);
            string[] groups = data.GroupNames.Split(',');
            foreach (var grp in groups)
            {
                Log.Instance.Info("group: " + grp);
            }
            foreach (var groupName in groups)
            {
                var userGroup = _userGroups.GetOrAdd(groupName, new UserGroup()
                {
                    Users = new ConcurrentBag<Guid>()
                });
                if (!userGroup.Users.Contains(data.UserId))
                {
                    userGroup.Users.Add(data.UserId);
                    Log.Instance.Info(data.UserId + " Added To " + groupName);
                }

            }
            return groups.Length;
        }

        [HttpGet]
        private void RemoveUser(Guid key)
        {
            if (_users.ContainsKey(key))
            {
                WebSocket ows;
                _users.TryRemove(key, out ows);
                foreach (var userGroup in _userGroups)
                {
                    if (userGroup.Value.Users.Contains(key))
                    {
                        Guid og;
                        userGroup.Value.Users.TryTake(out og);
                    }
                }
            }
        }


        [HttpGet]
        public string Test()
        {
            return DateTime.Now.ToString();
        }
    }
}