using System.Collections.Generic;

namespace Adin.Pusher.Domain.Utils
{
    public class WebsocketData
    {
        public int Code { get; set; }

        public List<string> Clients { get; set; }

        public object Data { get; set; }
    }
}