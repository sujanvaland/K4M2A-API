using Microsoft.AspNetCore.SignalR;

namespace K4M2A.API.Helper
{
    public class NotificationHubs : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
