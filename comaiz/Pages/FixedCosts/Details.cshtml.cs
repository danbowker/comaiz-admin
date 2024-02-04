using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace comaiz.Pages.FixedCosts
{
    public class DetailsModel : PageModel
    {
        private readonly ComaizContext _context;

        public DetailsModel(ComaizContext context)
        {
            _context = context;
        }

      public FixedCost FixedCost { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.FixedCosts == null)
            {
                return NotFound();
            }

            var cost = await _context.FixedCosts.FirstOrDefaultAsync(m => m.Id == id);
            if (cost == null)
            {
                return NotFound();
            }
            else 
            {
                FixedCost = cost;
            }
            return Page();
        }
    }
}
