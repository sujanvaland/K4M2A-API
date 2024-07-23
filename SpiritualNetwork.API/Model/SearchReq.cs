namespace SpiritualNetwork.API.Model
{
    public class SearchReq
    {
        public string Name { get; set; }
    }


    public class SearchReqByPage
    {
        public string Name { get; set; }
        public int PageNo { get; set; }
        public int Records { get; set; }

    }

}
