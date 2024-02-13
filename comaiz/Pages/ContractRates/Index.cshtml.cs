using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using comaiz.data.Models;
using comaiz.data;

namespace comaiz.Pages.ContractRates
{
    public class IndexModel : PageModel
    {
        private readonly ComaizContext _context;

        public IndexModel(ComaizContext context)
        {
            _context = context;
        }

        public IList<ContractRate> ContractRate { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.ContractRates != null)
            {
                ContractRate = await _context
                    .ContractRates.Include(c => c.Contract)
                    .AsNoTracking()
                    .ToListAsync();
            }
        }
    }
}
