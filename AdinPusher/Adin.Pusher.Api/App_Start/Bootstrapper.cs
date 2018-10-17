using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;

namespace Adin.Pusher.Api
{
    public static class Bootstrapper
    {
        private const string BASE_PATH = "https://adin-c3c10.firebaseio.com/";
        private const string FIREBASE_SECRET = "nweVlAT9MitRjiO3epfLXDW57eIfN0ENlkRc3VQD";

        public static void Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            GlobalConfiguration.Configure(WebApiConfig.Register);

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var builder = new ContainerBuilder();

            // Register your MVC controllers.
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.Register(context => new FirebaseConfig
            {
                AuthSecret = FIREBASE_SECRET,
                BasePath = BASE_PATH
            }).As<IFirebaseConfig>().SingleInstance();

            builder.RegisterType<FirebaseClient>().As<IFirebaseClient>().SingleInstance();


            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver((IContainer)container); //Set the WebApi DependencyResolver

        }
    }
}