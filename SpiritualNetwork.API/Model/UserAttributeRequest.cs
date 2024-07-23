using SpiritualNetwork.Entities;
using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.API.Model
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
