using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Adin.Pusher.Api.Utils;
using Adin.Pusher.Domain.Logger;
using FireSharp.Interfaces;

namespace Adin.Pusher.Api.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            return "OK";
        }
    }
        
}
