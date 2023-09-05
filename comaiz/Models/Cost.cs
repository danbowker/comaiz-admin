namespace comaiz.Models
{
    public class Cost
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public Contract? Contract { get; set; }
        public string? Name { get; set; }
        public ChargeType ChargeType { get; set; }
    }
    public enum ChargeType
    {
        Fixed = 0,
        TimeAndMaterials = 1,
    }
}
