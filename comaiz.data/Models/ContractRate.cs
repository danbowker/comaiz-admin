namespace comaiz.data.Models
{
    public class ContractRate
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string InvoiceDescription { get; set; } = string.Empty;
        public Contract? Contract { get; set; }
        public decimal? Rate { get; set; }
        public ICollection<UserContractRate>? UserContractRates { get; set; }
    }
}
