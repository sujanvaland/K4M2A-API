namespace SpiritualNetwork.API.Model
{
    public class PollModal
    {
        public string PollTitle { get; set; }
        public string Choice1 { get; set; }
        public string Choice2 { get; set; }
        public string? Choice3 { get; set; }
        public string? Choice4 { get; set; }
        public decimal Choice1Per { get; set; }
        public decimal Choice2Per { get; set; }
        public decimal Choice3Per { get; set; }
        public decimal Choice4Per { get; set; }
        public int TotalVote { get; set; }
        public string? IsVoted { get; set; }
        public DateTime ValidTill { get; set; }
        public bool IsValid { get; set; }
    }
}
