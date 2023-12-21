﻿namespace comaiz.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public string? PurchaseOrder { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public ICollection<InvoiceItem>? InvoiceItems { get; set; }
    }
}
