namespace K4M2A.API.Hubs
{
    public interface INotificationHub
    {
        public Task SendMessage(object obj);
        public Task SendChatMessage(object obj);
        public Task OnNewPost(object obj);
        public Task SendMessageToGroup(string groupName, string message);
    }
}
