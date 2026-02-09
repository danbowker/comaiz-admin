namespace comaiz.data.Models
{
    // Can either reference a Task or a FixedCost
    public class InvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
        public int? TaskId { get; set; }
        public Task? Task { get; set; }
        public int? FixedCostId { get; set; }
        public FixedCost? FixedCost { get; set; }
        public decimal Quantity { get; set; }
        public Unit Unit { get; set; }
        public decimal Rate { get; set; }
        public decimal VATRate { get; set; }
        public decimal Price { get; set; }
        public decimal PriceIncVAT { get; set; }
        public string? Description { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
