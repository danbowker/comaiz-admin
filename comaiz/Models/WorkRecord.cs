namespace comaiz.Models
{
    public class WorkRecord
    {
        public int Id { get; set; }
        public decimal Hours { get; set; }
        public Worker Worker { get; set; }
    }
}
