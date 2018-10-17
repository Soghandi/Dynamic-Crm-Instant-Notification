using NLog;
using NLog.Config;
using NLog.Targets;

namespace Adin.Pusher.Domain.Logger
{
    public static class Log
    {
        public static NLog.Logger Instance { get; private set; }
        /// <summary>
        /// reconfigure logger for udp target
        /// </summary>
        static Log()
        {
            var sentinalTarget = new NLogViewerTarget()
            {
                Name = "sentinal",
                Address = "udp://127.0.0.1:9999",
                IncludeNLogData = false
            };
            var sentinalRule = new LoggingRule("*", LogLevel.Trace, sentinalTarget);
            LogManager.Configuration.AddTarget("sentinal", sentinalTarget);
            LogManager.Configuration.LoggingRules.Add(sentinalRule);

            LogManager.ReconfigExistingLoggers();

            Instance = LogManager.GetCurrentClassLogger();
        }
    }
}
