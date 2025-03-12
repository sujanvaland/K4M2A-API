using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IPollService
    {
        public Task<Poll> SavePoll(Poll poll);

        public Task<JsonResponse> SavePollVote(PollVote vote);

        public Task<JsonResponse> GetPollDetails(int PollId, int UserId);
    }
}
