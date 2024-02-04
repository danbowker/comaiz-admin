namespace comaiz.data.Models
{
    public class WorkRecord : Cost
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal Hours { get; set; }
        public int WorkerId { get; set; }
        public Worker? Worker { get; set; }
        public int? ContractRateId { get; set; }
        public ContractRate? ContractRate { get; set; }
    }
}
