namespace comaiz.data.Models
{
    public class WorkRecord : Cost
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal Hours { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public int? ContractRateId { get; set; }
        public ContractRate? ContractRate { get; set; }
    }
}
