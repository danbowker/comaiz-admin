using comaiz.data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace comaiz.data
{
    public class ComaizContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ComaizContext(DbContextOptions<ComaizContext> options)
            : base(options)
        {
        }

        public DbSet<Client>? Clients { get; set; } = default!;
        public DbSet<Contract>? Contracts { get; set; }
        public DbSet<Worker>? Workers { get; set; }
        public DbSet<FixedCost>? FixedCosts { get; set; }
        public DbSet<CarJourney>? CarJourneys { get; set; }
        public DbSet<WorkRecord>? WorkRecords { get; set; }
        public DbSet<ContractRate>? ContractRates { get; set; }
        public DbSet<Invoice>? Invoices { get; set; }
        public DbSet<InvoiceItem>? InvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Important for Identity

            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Contract>().ToTable("Contract");
            modelBuilder.Entity<Worker>().ToTable("Worker");
            modelBuilder.Entity<FixedCost>().ToTable("FixedCost");
            modelBuilder.Entity<CarJourney>().ToTable("CarJourney");
            modelBuilder.Entity<WorkRecord>().ToTable("WorkRecord");
            modelBuilder.Entity<ContractRate>().ToTable("ContractRate");
            modelBuilder.Entity<Invoice>().ToTable("Invoice");
            modelBuilder.Entity<InvoiceItem>().ToTable("InvoiceItem");
            modelBuilder.Ignore<Cost>();
        }
    }
}
