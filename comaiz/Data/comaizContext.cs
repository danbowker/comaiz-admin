using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using comaiz.Models;

namespace comaiz.Data
{
    public class comaizContext : DbContext
    {
        public comaizContext (DbContextOptions<comaizContext> options)
            : base(options)
        {
        }

        public DbSet<comaiz.Models.Client> Client { get; set; } = default!;
    }
}
