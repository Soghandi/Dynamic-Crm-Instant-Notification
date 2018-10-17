using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Adin.Pusher.Api.Utils;
using Adin.Pusher.Domain.Logger;

namespace Adin.Pusher.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //  AreaRegistration.RegisterAllAreas();
            // GlobalConfiguration.Configure(WebApiConfig.Register);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            Bootstrapper.Start();

            HeartbeatService.Instance.Configure();
            Log.Instance.Info("Project Started");
        }
    }
}
