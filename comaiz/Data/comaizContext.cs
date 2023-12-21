﻿using System;
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

        public DbSet<Client> Clients { get; set; } = default!;
        public DbSet<Contract>? Contracts { get; set; }
        public DbSet<Worker>? Workers { get; set; }
        public DbSet<Cost>? Costs { get; set; }
        public DbSet<WorkRecord>? WorkRecords { get; set; }
        public DbSet<ContractRate>? ContractRates { get; set; }
        public DbSet<Invoice>? Invoices { get; set; }
        public DbSet<InvoiceItem>? InvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Contract>().ToTable("Contract");
            modelBuilder.Entity<Worker>().ToTable("Worker");
            modelBuilder.Entity<WorkRecord>().ToTable("WorkRecord");
            modelBuilder.Entity<ContractRate>().ToTable("ContractRate");
        }
    }
}
