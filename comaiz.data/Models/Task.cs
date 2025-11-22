namespace comaiz.data.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ContractId { get; set; }
        public Contract? Contract { get; set; }
        public int? ContractRateId { get; set; }
        public ContractRate? ContractRate { get; set; }
        public ICollection<TaskContractRate>? TaskContractRates { get; set; }
        public RecordState State { get; set; } = RecordState.Active;
    }
}
