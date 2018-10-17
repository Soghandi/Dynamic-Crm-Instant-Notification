using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace PusherTest
{
    public class WebSocketService
    {
        string pusherUrl = "http://pusher.iranian.cards/api/";
        public void Connect()
        {

            var wsUrl = "ws://pusher.iranian.cards/api/";
            using (var ws = new WebSocket(wsUrl + @"WebSocket/Get"))
            {
                ws.OnOpen += (sender, e) =>
                {
                    Console.WriteLine("Console open");
                };
                ws.OnClose += (sender, e) =>
                {
                    Console.WriteLine("Console Close");
                };
                ws.OnMessage += (sender, e) =>
                  Console.WriteLine("Laputa says: " + e.Data);

                ws.Connect();
                //  ws.Send("BALUS");
                Console.ReadKey(true);
            }
        }
        public async Task SendMessage(PushModel pushModel, string groupName)
        {
            try
            {
                var message = JsonConvert.SerializeObject(pushModel);
                var model = new PushRequestModel()
                {
                    Message = message,
                    GroupName = groupName
                };
                var requestBody = JsonConvert.SerializeObject(model);
                using (var client = new HttpClient())
                {
                    string url = pusherUrl + "websocket/sendMessage";


                    var result = await client.PostAsync(new Uri(url), new StringContent(requestBody, Encoding.UTF8, "application/json"));
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        string resultContent = await result.Content.ReadAsStringAsync();
                        Console.WriteLine(resultContent);
                    }
                    else
                    {
                        Console.WriteLine("Send FCM Error,Status Code:" + result.StatusCode);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message, "Error on Push On Web ");
            }
        }
    }
    public class PushModel
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        public string Link { get; set; }

        public string ReceiverId { get; set; }

        public string Title { get; set; }

    }

    public class PushRequestModel
    {
        public string Message { get; set; }

        public string GroupName { get; set; }

    }
}
