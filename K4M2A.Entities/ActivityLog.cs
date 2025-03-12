using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class ActivityLog : BaseEntity
    {
        public int UserId { get; set; }
        public string? ActivityType { get; set; }
        public string? Type { get; set; }
        public int? RefId1 { get; set; }
        public int? RefId2 { get; set; }
        public string? Message { get; set; }
        public string? IPAddress {  get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }

    }
}
