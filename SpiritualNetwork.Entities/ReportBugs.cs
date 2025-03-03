using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class ReportBugs : BaseEntity
    {
        public string? BugTitle { get; set; }
        public string? BugDescription { get; set; }
        public string? Priority { get; set; }
        public List<string>? Files { get; set; }
    }
}
