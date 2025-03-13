namespace K4M2A.API.Helper
{
    public interface ISpirtual
    {
        public Task SendMessage(string message, List<string> channels);
    }
}
