using K4M2A.Entities;
using K4M2A.Entities.CommonModel;

namespace K4M2A.API.Services.Interface
{
    public interface IPollService
    {
        public Task<Poll> SavePoll(Poll poll);

        public Task<JsonResponse> SavePollVote(PollVote vote);

        public Task<JsonResponse> GetPollDetails(int PollId, int UserId);
    }
}
