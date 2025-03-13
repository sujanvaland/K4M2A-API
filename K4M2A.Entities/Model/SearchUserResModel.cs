
namespace K4M2A.Entities.Model
{
    public class SearchUserResModel
    {
        public string? FullName { get; set; }
        public string? Email { get; set;}
        public string? PhoneNumber { get; set; }
        public int? Id { get; set;}
        public string? FirstName { get; set;}
        public string? LastName { get; set;}
        public string? UserName { get; set; }
        public string? ProfileImg { get; set; }
        public bool? Online { get; set; }
        public string? UniqueId { get; set; }
        public bool? IsInvited { get; set; }
        public bool? IsHidden { get; set; }
        public DateTime? Created { get; set; }
        public bool? IsBusinessAccount { get; set; }
        public bool? IsFollowedByLoginUser { get; set; }

    }
}
