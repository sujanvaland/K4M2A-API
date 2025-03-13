using K4M2A.Entities;
using System.ComponentModel.DataAnnotations;

namespace K4M2A.Entities.Model
{
    public class UserAttributeRequest
    {
        public int UserId { get; set; }
        [MaxLength(100)]
        public string KeyName { get; set; }
        [MaxLength(500)]
        public string Value { get; set; }
    }
}
