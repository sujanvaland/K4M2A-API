using Microsoft.AspNetCore.SignalR;
using K4M2A.API.Hubs;

namespace K4M2A.API.Hubs
{
    public class NotificationHub : Hub<INotificationHub>
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Client(Context.ConnectionId).SendMessage("connected-"+Context.ConnectionId);
        }
        public async Task SendMessage(object obj)
        {
            await Clients.All.SendMessage(obj);
        }
        public async Task SendChatMessage(object obj)
        {
            await Clients.Client(Context.ConnectionId).SendChatMessage(obj);
        }
        public async Task OnNewPost(object obj)
        {
            await Clients.Client(Context.ConnectionId).OnNewPost(obj);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Clients.Client(Context.ConnectionId).SendMessage("disconnected-"+Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
