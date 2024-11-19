using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.Entities
{
    public class Invitation : BaseEntity
    {
        public string Name { get; set; }
    }
    public class User : BaseEntity
    {
        [MaxLength(100)]
        public string UserName { get; set; }
        public int InviterId { get; set; }
        [MaxLength(200)]
        public string Password { get; set; }
        [MaxLength(100)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        public string LastName { get; set; }
        [MaxLength(200)]
        public string Email { get; set; }
        public int? IsEmailVerified { get; set; }
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        public int? IsPhoneVerified { get; set; }
        [MaxLength(10)]
        public string Status { get; set; }
        [MaxLength(100)]
        public string PaymentStatus { get; set; }
        [MaxLength(100)]
        public string PaymentRef1 { get; set; }
        [MaxLength(100)]
        public string PaymentRef2 { get; set; }
        [MaxLength(200)]
        public string? About { get; set; }
        public DateTime? DOB { get; set; }
        [MaxLength(10)]
        public string? Gender { get; set; }
        [MaxLength(200)]
        public string? Location { get; set; }
        [MaxLength(200)]
        public string? Profession { get; set; }
        [MaxLength(200)]
        public string? Organization { get; set; }
        [MaxLength(100)]
        public string? Title { get; set; }
        [MaxLength(200)]
        public string? FacebookLink { get; set; }
        [MaxLength(200)]
        public string? LinkedinLink { get; set; }
        [MaxLength(200)]
        public string? Skills { get; set; }
        [MaxLength(1000)]
        public string? ProfileImg { get; set; }
        [MaxLength(1000)]
        public string? BackgroundImg { get; set; }
        [MaxLength(1000)]
        public string? Tags { get; set; }
        [MaxLength(200)]
        public string? LoginMethod { get; set;}
        public DateTime? ReActivationDate { get; set; }
        [MaxLength(200)]
        public string? Latitude { get; set; }
        [MaxLength(200)]
        public string? Longitude { get; set; }
        public bool? IsBusinessAccount { get; set; }
        public string? SecondaryPassword { get; set; }
        public string? GoogleId { get; set; }
    }
}