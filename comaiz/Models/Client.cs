namespace comaiz.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string? ShortName { get; set; }
        public string? Name { get; set; }
        public ICollection<Contract>? Contracts { get; set; }
    }
}
