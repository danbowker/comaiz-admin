namespace comaiz.data.Models
{
    public class WorkRecord
    {
        public int Id { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal Hours { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public int? TaskId { get; set; }
        public Task? Task { get; set; }
        public int? InvoiceItemId { get; set; }
    }
}
