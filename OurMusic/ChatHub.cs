using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace OurMusic
{
    public class ChatHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
        public void Send(String name, String message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }
    }
}