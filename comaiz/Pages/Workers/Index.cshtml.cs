using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using comaiz.Data;
using comaiz.Models;

namespace comaiz.Pages.Workers
{
    public class IndexModel : PageModel
    {
        private readonly comaiz.Data.ComaizContext _context;

        public IndexModel(comaiz.Data.ComaizContext context)
        {
            _context = context;
        }

        public IList<Worker> Worker { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Workers != null)
            {
                Worker = await _context.Workers.ToListAsync();
            }
        }
    }
}
