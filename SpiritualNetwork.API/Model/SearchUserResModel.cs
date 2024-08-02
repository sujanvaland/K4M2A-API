using Microsoft.EntityFrameworkCore.Metadata;

namespace SpiritualNetwork.API.Model
{
    /*FullName = user.FullName,
                                            Email = user.Email,
                                            PhoneNumber = user.PhoneNumber,
                                            Id = user.Id,
                                            FirstName = user.FirstName,
                                            LastName = user.LastName,
                                            UserName = "",
                                            ProfileImg = user.Photo,
                                            Online = false,
                                            UniqueId = user.UniqueId*/
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

    }
}
