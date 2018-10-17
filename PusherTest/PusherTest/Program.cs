using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PusherTest
{
    class Program
    {
        static void Main(string[] args)
        {
            WebSocketService webSocketService = new WebSocketService();
            //  webSocketService.Connect();
            var body = Console.ReadLine();

            while (body != "exit")
            {
                var link = "https://crm.iranian.cards/IranCardDEV/main.aspx?etc=10041&id=6572F425-F263-E811-8431-005056A81EBC&pagetype=entityrecord";
                var userId = "cdf10765-8926-e811-8416-005056871d68";
                var pushModel = new PushModel
                {
                    Code = 100,
                    Message = body,
                    Data = new { },
                    Link = HttpUtility.UrlEncode(link),
                    ReceiverId = userId,
                    Title="ایران کارت"

                };
                webSocketService.SendMessage(pushModel, userId).Wait();
                body = Console.ReadLine();

            }
        }



    }
}