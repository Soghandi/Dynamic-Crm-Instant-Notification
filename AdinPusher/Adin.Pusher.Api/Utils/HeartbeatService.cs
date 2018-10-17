using System;
using System.Timers;
using Adin.Pusher.Domain.Logger;
using Adin.Pusher.Domain.Utils;

namespace Adin.Pusher.Api.Utils
{
    public class HeartbeatService
    {
        private HeartbeatService()
        {

        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                WebsocketService websocketService = new WebsocketService();
                websocketService.Send(new WebsocketData()
                {
                    Code = 1,
                    Clients = null,
                    Data = new
                    {
                        CurrentDate = DateTime.Now.ToString("yyyyMMddhhmmssff")
                    }
                }).Wait(2 * 1000);
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Error on heartbeat");
            }

        }

        private static readonly Timer _timer = new Timer(5 * 1000 * 60);

        public static HeartbeatService Instance { get; } = new HeartbeatService();

        public void Configure()
        {
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
        }
    }
}