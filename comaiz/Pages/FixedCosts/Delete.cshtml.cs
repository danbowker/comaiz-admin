using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace comaiz.Pages.FixedCosts
{
    public class DeleteModel : PageModel
    {
        private readonly ComaizContext _context;

        public DeleteModel(ComaizContext context)
        {
            _context = context;
        }

        [BindProperty]
      public FixedCost FixedCost { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.FixedCosts == null)
            {
                return NotFound();
            }

            var fixedCost = await _context.FixedCosts.FirstOrDefaultAsync(m => m.Id == id);

            if (fixedCost == null)
            {
                return NotFound();
            }
            else 
            {
                FixedCost = fixedCost;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.FixedCosts == null)
            {
                return NotFound();
            }
            var fixedCost = await _context.FixedCosts.FindAsync(id);

            if (fixedCost != null)
            {
                FixedCost = fixedCost;
                _context.FixedCosts.Remove(fixedCost);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
