using SpiritualNetwork.Entities;

namespace SpiritualNetwork.API.Model
{
    public class LikesCountModel
    {
        public Reaction reaction { get; set; }
        public int LikesCount { get; set; }
        public UserPost? Poster { get; set; }
        public User? User { get; set; }
    }
}
