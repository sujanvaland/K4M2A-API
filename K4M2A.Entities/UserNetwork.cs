using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class UserNetwork : BaseEntity
    {
        public int? InviterId { get; set; }
        public string? UniqueId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set;}
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Photo {  get; set; }
        public string? PhoneNumber { get; set; }
        public bool? IsInvited { get; set; } = false;
    }
}
