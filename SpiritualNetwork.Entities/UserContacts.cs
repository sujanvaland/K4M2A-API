using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.Entities
{
    public class UserContacts : BaseEntity
    {
        public int UserId { get; set; }
        [MaxLength(200)]
        public string FirstName { get; set; }
        [MaxLength(200)]
        public string LastName { get; set; }
        [MaxLength(200)]
        public string Email { get; set; }
        [MaxLength(20)]
        public string PhoneNumber { get; set; }
        [MaxLength(10)]
        public string Gender { get; set; }
        [MaxLength(10)]
        public string ContactType { get; set; }
    }
}
