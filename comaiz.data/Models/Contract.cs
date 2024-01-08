namespace comaiz.data.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public string? Description { get; set; }
        public decimal Rate { get; set; }
        public decimal Price { get; set; }
        public ICollection<Cost>? Costs { get; set; }
        public ICollection<ContractRate>? ContractRates { get; set; }
        public string? Assignment { get; set; }
    }
}
