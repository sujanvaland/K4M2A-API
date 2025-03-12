using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class ReportEntity : BaseEntity
    {
        public int ActionUserId { get; set; }
        public string ReportType { get; set; }
        public int ReportId { get; set; }
        public string? Value { get; set; }
        public string? Description {  get; set; }

    }
}
