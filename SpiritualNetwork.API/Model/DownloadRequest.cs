namespace SpiritualNetwork.API.Model
{
    public class DownloadRequest
    {
        public string Url { get; set; }
        public string Content { get; set; }
    }

    public class deleteFileReq
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }

}
