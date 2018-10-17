using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace SendNotification
{
    public class WebSocketService : IDisposable
    {
        readonly string pusherUrl = "http://pusher.adin.ir/api/";

        public void Dispose()
        {

        }

        public async Task SendMessage(PushModel pushModel, string groupName)
        {
            try
            {
                string message = string.Empty;
            using (var ms = new MemoryStream())
            {
                var js = new DataContractJsonSerializer(typeof(PushModel));
                js.WriteObject(ms, pushModel);
                ms.Position = 0;
                var sr = new StreamReader(ms);
                message =await sr.ReadToEndAsync();
            }

            
            var model = new PushRequestModel()
            {
                Message = message,
                GroupName = groupName
            };
            var requestBody =string.Empty;
            using (var ms = new MemoryStream())
            {
                var js = new DataContractJsonSerializer(typeof(PushRequestModel));
                js.WriteObject(ms, model);
                ms.Position = 0;
                var sr = new StreamReader(ms);
                requestBody = await sr.ReadToEndAsync();
            }


            using (var client = new HttpClient())
            {
                string url =
                    pusherUrl + "websocket/sendMessage";
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(new Uri(url),content);
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
