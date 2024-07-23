using SpiritualNetwork.Entities;

namespace SpiritualNetwork.API.Model
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public ProfileModel? Profile { get; set; }
    }
}
