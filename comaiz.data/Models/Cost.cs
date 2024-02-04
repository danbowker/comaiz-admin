using System.ComponentModel.DataAnnotations.Schema;

namespace comaiz.data.Models
{
    public abstract class Cost
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public Contract? Contract { get; set; }
        public int ? InvoiceItemId { get; set; }
    }

    public class FixedCost : Cost
    {
        public string? Name { get; set; }
        public decimal? Amount { get; set; }
    }

    public class CarJourney : Cost
    {
        public DateOnly? Date { get; set; }
        public string? Description { get; set; }
        public decimal? Miles { get; set; }
        public decimal? Rate { get; set; }
    }
}
