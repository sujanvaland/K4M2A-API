using Microsoft.AspNetCore.SignalR;

namespace SpiritualNetwork.API.Helper
{
    public class NotificationHubs : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
