using SpiritualNetwork.Entities;
using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.API.Model
{
    public class ProfileModel : User
    {
        public bool IsPremium { get; set; }
        public OnlineUsers ConnectionDetail { get; set; }
        public int NoOfFollowers { get; set; }
        public int NoOfFollowing { get; set; }
        public int IsFollowedByLoginUser { get; set; }
        public int IsFollowingLoginUser { get; set; }
    }

    public class ProfileReqest 
    {
		public string? Name { get; set; }
		public string? Username { get; set; }
		public string? Email { get; set; }
		public string? Password { get; set; }
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
        public int ModifiedBy { get; set; }
    }

    public class UserFollowersModel
    {
        public UserFollowersModel()
        {
            Followers = new List<ProfileModel>();
            Following = new List<ProfileModel>();
            Mutual = new List<ProfileModel>();
        }
        public List<ProfileModel>? Followers { get; set; }
        public List<ProfileModel>? Following { get; set; }
        public List<ProfileModel>? Mutual { get; set; }

    }

    public class Mentions
    {
        public int userId { get; set; }
        public string name { get; set; }
        public string link { get; set; }
        public string avatar { get; set; }
        public bool? IsBusinessAccount { get; set; }

    }

    public class TagUser
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string userName { get; set; }

    }
}
