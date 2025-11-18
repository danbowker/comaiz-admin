namespace comaiz.data.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public string? Description { get; set; }
        // TODO Remove rate and use the contract rates instead
        //public decimal Rate { get; set; }
        public decimal? Price { get; set; }
        public ICollection<Cost>? Costs { get; set; }
        public ICollection<ContractRate>? ContractRates { get; set; }
        public string? Schedule { get; set; }
        public ChargeType ChargeType { get; set; }
        public RecordState State { get; set; } = RecordState.Active;
        public DateOnly? PlannedStart { get; set; }
        public DateOnly? PlannedEnd { get; set; }
    }
    public enum ChargeType
    {
        Fixed = 0,
        TimeAndMaterials = 1,
    }
}
