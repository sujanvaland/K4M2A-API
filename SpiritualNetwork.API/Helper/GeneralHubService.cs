/*using InnFlow.CoreAPI.Hubs.Dtos;
using InnFlow.CoreAPI.SignalR.Clients;
using InnFlow.CoreAPI.SignalR.Constants;
using InnFlow.CoreAPI.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InnFlow.CoreAPI.SignalR.Services {

    public class GeneralHubService :IGeneralHubService 
  {
        private IHubContext<GeneralHub, IGeneralClient> _generalHub;
        private IHubContext<UserManagementHub, IUserManagementClient> _userManagementHub;
        public GeneralHubService(IHubContext<GeneralHub, IGeneralClient> generalHub, IHubContext<UserManagementHub, IUserManagementClient> userManagementHub)
        {
            _generalHub = generalHub;
            _userManagementHub = userManagementHub;
        }

        public async Task PublishMessage(NotificationDataDto data,  List<string> channels )
        {
            foreach (var channel in channels)
            {
                switch (channel)
                {
                    case NotificationHubModuleNames.GENERAL:
                         await _generalHub.Clients.All.NotifyUsersAboutSiteDown(data);
                        break;
                    case NotificationHubModuleNames.USER_MANAGEMENT:
                        await _userManagementHub.Clients.All.NotifySaveUserDetails(data);
                        break;
                    default:
                        break;
                }

            }
        }
    }
}
*/