using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace singalR2
{
    
    public class ChatHub : Hub
    {
        //server
        public void Join(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
        }

        public void groupSend(string group, string message)
        {
            try
            {
                Clients.Group(group).addNewMessageToPage(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
      
        public void send(string name, string message)
        {
            try
            {
                Clients.All.addNewMessageToPage(name, message);
                //Clients.All.broadcastMessage(name, message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}