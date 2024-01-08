using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace comaiz.Pages.Costs
{
    public class IndexModel : PageModel
    {
        private readonly ComaizContext _context;

        public IndexModel(ComaizContext context)
        {
            _context = context;
        }

        public IList<Cost> Cost { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Costs != null)
            {
                Cost = await _context.Costs.ToListAsync();
            }
        }
    }
}
