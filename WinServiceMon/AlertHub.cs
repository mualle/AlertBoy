using Microsoft.AspNet.SignalR;
using System;

namespace WinServiceMon
{
    public class AlertHub : Hub
    {
        public void Alert(string alert)
        {
            Clients.All.notify(alert);
        }
    }
}
