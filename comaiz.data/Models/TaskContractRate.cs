namespace comaiz.data.Models
{
    public class TaskContractRate
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public Task? Task { get; set; }
        public int ContractRateId { get; set; }
        public ContractRate? ContractRate { get; set; }
    }
}
