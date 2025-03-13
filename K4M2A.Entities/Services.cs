using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K4M2A.Entities
{
    public class Service : BaseEntity
    {
        [MaxLength(200)]
        public string Name { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string? Tags { get; set; }
        public string? Description { get; set; }
    }

    public class ServiceFAQ : BaseEntity
    {
        public int ServiceId { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
    }
    
    public class ServiceImages : BaseEntity
    {
        public int? ServiceId { get; set; }
        public string? ImageUrl { get; set;}
    }
}
