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
    public class DeleteModel : PageModel
    {
        private readonly ComaizContext _context;

        public DeleteModel(ComaizContext context)
        {
            _context = context;
        }

        [BindProperty]
      public ContractRate ContractRate { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.ContractRates == null)
            {
                return NotFound();
            }

            var contractrate = await _context.ContractRates.FirstOrDefaultAsync(m => m.Id == id);

            if (contractrate == null)
            {
                return NotFound();
            }
            else 
            {
                ContractRate = contractrate;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.ContractRates == null)
            {
                return NotFound();
            }
            var contractrate = await _context.ContractRates.FindAsync(id);

            if (contractrate != null)
            {
                ContractRate = contractrate;
                _context.ContractRates.Remove(ContractRate);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
