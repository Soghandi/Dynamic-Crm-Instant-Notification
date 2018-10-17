using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adin.Pusher.Api.Model
{
    public class PushRequestModel
    {
        public string Message { get; set; }

        public string GroupName { get; set; }
        
    }
}