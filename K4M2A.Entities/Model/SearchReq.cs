namespace K4M2A.Entities.Model
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
