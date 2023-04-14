namespace comaiz.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
        public decimal Price { get; set; }
        public ChargeType ChargeType { get; set; }
        public ICollection<WorkRecord>? WorkRecords { get; set; }
    }

    public enum ChargeType
    {
        Fixed = 0,
        TimeAndMaterials = 1,
    }
}
