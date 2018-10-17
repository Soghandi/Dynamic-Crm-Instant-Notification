using Microsoft.Xrm.Sdk;
using System;


namespace SendNotification
{
    public class Core : IPlugin
    {
        readonly string baseUrl = "https://crm.adin.ir/dynamic365";
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if ((context.MessageName == "Update" || context.MessageName == "Create") &&
                context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                switch (((Entity)context.InputParameters["Target"]).LogicalName)
                {
                    case "task":
                        var taskEntity = (Entity)context.InputParameters["Target"];
                        var taskPreImg = context.PreEntityImages["Img"];
                        if (context.MessageName == "Update" && taskEntity.Attributes.Contains("ownerid"))
                        {
                            var userId = ((EntityReference)taskEntity.Attributes["ownerid"]).Id;
                            var taskId = taskEntity.Attributes["activityid"].ToString();
			    //Task objectTypeId = 4212
                            var link = string.Format("{0}/main.aspx?etc={1}&id={2}&pagetype=entityrecord", baseUrl, 4212, taskId);
                            var subject = taskPreImg.Attributes["subject"].ToString();
                            SendMessage(userId.ToString(), taskId, link, subject, "New Task");
                        }
                        break;


                }
            }
        }
        private void SendMessage(string userId, string entityId, string link, string subject, string title = "Default Title")
        {
            using (var webSocketService = new WebSocketService())
            {
                try
                {
                    var pusherModel = new PushModel()
                    {
                        Link = (link),
                        ReceiverId = userId.ToString(),
                        Message = subject,
                        Code = 100,
                        Title = title
                    };
                    webSocketService.SendMessage(pusherModel, userId.ToString()).Wait();
                }
                catch (Exception EX)
                {
                    Console.WriteLine(EX.Message);
                }

            }
        }
    }
}
