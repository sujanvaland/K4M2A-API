using SpiritualNetwork.Entities;

namespace SpiritualNetwork.API.Model
{
    public class PreSignupRequest
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string IpAddress { get; set; }

        public List<AnswerModel> Answers { get; set; }
    }
}
