using SpiritualNetwork.Entities;

namespace SpiritualNetwork.Entities.Model
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public ProfileModel? Profile { get; set; }
    }
}
