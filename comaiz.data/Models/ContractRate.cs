namespace comaiz.data.Models
{
    public class ContractRate
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string Description { get; set; } = string.Empty;
        public Contract? Contract { get; set; }
        public decimal? Rate { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
