using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using comaiz.Data;
using comaiz.Models;

namespace comaiz.Pages.Costs
{
    public class DetailsModel : PageModel
    {
        private readonly comaiz.Data.ComaizContext _context;

        public DetailsModel(comaiz.Data.ComaizContext context)
        {
            _context = context;
        }

      public Cost Cost { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Costs == null)
            {
                return NotFound();
            }

            var cost = await _context.Costs.FirstOrDefaultAsync(m => m.Id == id);
            if (cost == null)
            {
                return NotFound();
            }
            else 
            {
                Cost = cost;
            }
            return Page();
        }
    }
}
