using System.Collections.Generic;

namespace SpiritualNetwork.Entities.Model
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
