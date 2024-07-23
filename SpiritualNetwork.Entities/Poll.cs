using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class Poll : BaseEntity
    {
        public string PollTitle { get; set; }
        public string Choice1 { get; set; }
        public string Choice2 { get; set; }
        public string? Choice3 { get; set; }
        public string? Choice4 { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
    }

    public class PollVote : BaseEntity
    {
        public int PollId { get; set; }
        public int UserId { get; set; }
        public string Choice { get; set; }
    }
}
