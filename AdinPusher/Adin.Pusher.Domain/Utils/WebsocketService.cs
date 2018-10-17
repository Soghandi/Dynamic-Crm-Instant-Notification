using System;
using System.Net.Http;
using System.Threading.Tasks;
using Adin.Pusher.Domain.Logger;
using Newtonsoft.Json;

namespace Adin.Pusher.Domain.Utils
{
    public class WebsocketService
    {
        readonly string _pusherAddress = System.Configuration.ConfigurationManager.AppSettings["pusherAddress"];

        public async Task Send(WebsocketData data)
        {
            try
            {
                var message = JsonConvert.SerializeObject(data);
                if (data.Clients == null || data.Clients.Count == 0)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var url = string.Format("http://{0}/api/WebSocket/sendmessageToAll?message={1}", _pusherAddress, message);
                        await client.GetAsync(url);

                    }
                }
                else
                {
                    foreach (var dataClient in data.Clients)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            var url = string.Format("http://{0}/api/WebSocket/sendmessage?groupName={1}&message={2}", _pusherAddress, dataClient, message);
                            await client.GetAsync(url);

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "pusher Error");

            }
        }


    }
}