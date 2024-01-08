namespace comaiz.data.Models
{
    public class InvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
        public int CostId { get; set; }
        public Cost? Cost{ get; set; }
        public int Quantity { get; set; }
        //TODO, Unit table or enum
        public int Unit { get; set; }
        public decimal Rate { get; set; }
        public decimal VATRate { get; set; }
        public decimal Price { get; set; }
    }
}
