namespace comaiz.data.Models
{
    public class UserContractRate
    {
        public int Id { get; set; }
        public int ContractRateId { get; set; }
        public ContractRate? ContractRate { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
