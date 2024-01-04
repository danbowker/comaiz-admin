namespace comaiz.data.Models
{
    public class WorkRecord
    {
        public int Id { get; set; }
        public decimal Hours { get; set; }
        public int WorkerId { get; set; }
        public Worker? Worker { get; set; }
        public int CostId { get; set; }
        public Cost? Cost{ get; set; }
    }
}
