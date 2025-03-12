
using Microsoft.AspNetCore.SignalR;

namespace SpiritualNetwork.API.Helper
{
    public class SpirttualHub : Hub,ISpirtual
    {
        public Task SendMessage(string message, List<string> channels)
        {
            throw new NotImplementedException();
        }
    }
}
