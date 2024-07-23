/*using InnFlow.CoreAPI.Hubs.Dtos;
using InnFlow.CoreAPI.SignalR.Clients;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InnFlow.CoreAPI.SignalR.Hubs {

    public class UserManagementHub : Hub<IUserManagementClient> {
        public async Task NotifySaveUserDetails(NotificationDataDto data)
        {
            await Clients.AllExcept(new List<string>() { Context.ConnectionId}).NotifySaveUserDetails(data);
        }

        
    }

}

*/