namespace comaiz.data.Models
{
    public class ContractRate
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public Contract? Contract { get; set; }
        public decimal? Rate { get; set; }
    }
}
