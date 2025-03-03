using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class SchedulePost : BaseEntity
    {
        public int UserId { get; set; }
        [MaxLength(4000)]
        public string PostMessage { get; set; }
        [MaxLength(10)]
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public bool? IsVideo { get; set; }
        public DateTime? ScheduleTime { get; set; }
        public bool? IsScheduled { get; set; }

    }
}
