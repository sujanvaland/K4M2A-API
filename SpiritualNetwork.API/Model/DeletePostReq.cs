using System.Collections.Generic;

namespace SpiritualNetwork.API.Model
{
    public class DeletePostReq
    {
        public int Id { get; set; }
    }

    public class DeleteSchedulePostReq
    {
        public List<int> Id { get; set; }
    }
}
