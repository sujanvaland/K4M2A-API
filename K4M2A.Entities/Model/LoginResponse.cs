using K4M2A.Entities;

namespace K4M2A.Entities.Model
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public ProfileModel? Profile { get; set; }
    }
}
