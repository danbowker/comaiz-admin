using comaiz.data.Models;

namespace comaiz.api.Models
{
    public class CreateFixedCostInvoiceItemDto
    {
        public int InvoiceId { get; set; }
        public int FixedCostId { get; set; }
        public decimal VATRate { get; set; } = 0m;
    }

    public class CreateLabourCostInvoiceItemDto
    {
        public int InvoiceId { get; set; }
        public int? ContractId { get; set; }
        public int TaskId { get; set; }
        public string? ApplicationUserId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal VATRate { get; set; } = 0.20m;
        public string? Description { get; set; }
    }

    public class CreateMileageCostInvoiceItemDto
    {
        public int InvoiceId { get; set; }
        public int? ContractId { get; set; }
        public decimal Quantity { get; set; } // Distance in miles
        public decimal Rate { get; set; }
        public decimal VATRate { get; set; } = 0m;
        public string? Description { get; set; }
    }
}
