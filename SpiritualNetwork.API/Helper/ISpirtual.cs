namespace SpiritualNetwork.API.Helper
{
    public interface ISpirtual
    {
        public Task SendMessage(string message, List<string> channels);
    }
}
