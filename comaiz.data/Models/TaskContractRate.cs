namespace comaiz.data.Models
{
    public class TaskContractRate
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public Task? Task { get; set; }
        public int UserContractRateId { get; set; }
        public UserContractRate? UserContractRate { get; set; }
    }
}
