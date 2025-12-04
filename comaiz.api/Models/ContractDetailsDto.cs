namespace comaiz.api.Models
{
    public class ContractDetailsDto
    {
        public int ContractId { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal TotalInvoiced { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal? Remaining { get; set; }
        public DateOnly? LastInvoiceEndDate { get; set; }
        public List<TaskDetailsDto> Tasks { get; set; } = new();
    }

    public class TaskDetailsDto
    {
        public int TaskId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal TotalInvoiced { get; set; }
        public decimal TotalPaid { get; set; }
        public DateOnly? LastInvoiceEndDate { get; set; }
    }
}
