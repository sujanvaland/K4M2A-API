namespace SpiritualNetwork.Entities
{
    public class UserSubcription : BaseEntity
    {
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsOrganization { get; set; }
        public bool IsAnnual { get; set; }
        public bool IsMonthly { get; set; }
        public string PaymentStatus { get; set; }

    }
}
