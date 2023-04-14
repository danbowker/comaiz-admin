using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using comaiz.Models;

namespace comaiz.Data
{
    public class ComaizContext : DbContext
    {
        public ComaizContext (DbContextOptions<ComaizContext> options)
            : base(options)
        {
        }

        public DbSet<comaiz.Models.Client> Clients { get; set; } = default!;

        public DbSet<comaiz.Models.Contract>? Contracts { get; set; }

        public DbSet<comaiz.Models.Worker>? Workers { get; set; }

        public DbSet<comaiz.Models.WorkRecord>? WorkRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Contract>().ToTable("Contract");
            modelBuilder.Entity<Worker>().ToTable("Worker");
            modelBuilder.Entity<WorkRecord>().ToTable("WorkRecord");
        }
    }
}
