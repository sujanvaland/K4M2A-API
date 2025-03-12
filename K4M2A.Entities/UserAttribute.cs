using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class UserAttribute : BaseEntity
    {
        public int UserId { get; set; }
        [MaxLength(100)]
        public string KeyName { get; set; }
        [MaxLength(500)]
        public string Value { get; set; }
    }
}
