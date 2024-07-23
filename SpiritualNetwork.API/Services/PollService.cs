using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services
{
    public class PollService : IPollService
    {
        private readonly IRepository<Poll> _pollRepository;
        private readonly IRepository<PollVote> _pollVoteRepository;
        public PollService(IRepository<Poll> pollRepository,
            IRepository<PollVote> pollVoteRepository) {
            _pollRepository = pollRepository;
            _pollVoteRepository = pollVoteRepository;
        }

        public async Task<JsonResponse> GetPollDetails(int PollId,int userId)
        {
            var poll = await _pollRepository.Table.Where(x=>x.Id == PollId).FirstOrDefaultAsync();
            var pollVote = await _pollVoteRepository.Table.Where(x => x.PollId == poll.Id).ToListAsync();
            var uservote = pollVote.Where(x => x.UserId == userId).FirstOrDefault();

            TimeSpan duration = new TimeSpan(poll.Day,poll.Hour,poll.Minute,0);
            PollModal result = new PollModal();
            result.PollTitle = poll.PollTitle;
            result.Choice1 = poll.Choice1;
            result.Choice2 = poll.Choice2;
            result.Choice3 = poll.Choice3;
            result.Choice4 = poll.Choice4;
            result.ValidTill = poll.CreatedDate.Add(duration);
            if(result.ValidTill > DateTime.UtcNow)
            {
                result.IsValid = true;
            }
            else
            {
                result.IsValid = false;
            }
            result.TotalVote = pollVote.Count;
            result.IsVoted = uservote == null ? "" : uservote.Choice;
            var choice1VoteCount = pollVote.Where(x => x.Choice == result.Choice1).Count();
            if(choice1VoteCount > 0 && pollVote.Count > 0)
            {
                result.Choice1Per = Math.Round((Convert.ToDecimal(choice1VoteCount) / Convert.ToDecimal(pollVote.Count)) * 100,2);
            }
            else
            {
                result.Choice1Per = 0;
            }

            var choice2VoteCount = pollVote.Where(x => x.Choice == result.Choice2).Count();
            if (choice2VoteCount > 0 && pollVote.Count > 0)
            {
                result.Choice2Per = Math.Round((Convert.ToDecimal(choice2VoteCount) / Convert.ToDecimal(pollVote.Count)) * 100,2);
            }
            else
            {
                result.Choice2Per = 0;
            }

            var choice3VoteCount = pollVote.Where(x => x.Choice == result.Choice3).Count();
            if (choice3VoteCount > 0 && pollVote.Count > 0)
            {
                result.Choice3Per = Math.Round((Convert.ToDecimal(choice3VoteCount) / Convert.ToDecimal(pollVote.Count)) * 100,2);
            }
            else
            {
                result.Choice3Per = 0;
            }

            var choice4VoteCount = pollVote.Where(x => x.Choice == result.Choice4).Count();
            if (choice4VoteCount > 0 && pollVote.Count > 0)
            {
                result.Choice4Per = Math.Round((Convert.ToDecimal(choice4VoteCount) / Convert.ToDecimal(pollVote.Count)) * 100,2);
            }
            else
            {
                result.Choice4Per = 0;
            }
            return new JsonResponse(200, true, "Success", result);
        }

        public async Task<Poll> SavePoll(Poll poll)
        {
            if(poll.Id == 0)
            {
                await _pollRepository.InsertAsync(poll);
            }

            return poll;
        }

        public async Task<JsonResponse> SavePollVote(PollVote vote)
        {
            var voteexist = _pollVoteRepository.Table.Where(x=>x.UserId == vote.UserId && x.PollId == vote.PollId).FirstOrDefault();

            if (voteexist == null)
            {
                vote.CreatedDate = DateTime.Now;
                vote.ModifiedDate = DateTime.Now;
                await _pollVoteRepository.InsertAsync(vote);
                return new JsonResponse(200, true, "Poll Created Successfully", vote);
            }
            else
            {
                voteexist.Choice = vote.Choice;
                voteexist.ModifiedDate = DateTime.Now;
                await _pollVoteRepository.UpdateAsync(voteexist);
                return new JsonResponse(200, true, "Poll Created Successfully", vote);
            }
        }


    }
}
