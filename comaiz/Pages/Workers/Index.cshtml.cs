using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace comaiz.Pages.Workers
{
    public class IndexModel : PageModel
    {
        private readonly ComaizContext _context;

        public IndexModel(ComaizContext context)
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
