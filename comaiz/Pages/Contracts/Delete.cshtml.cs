using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using comaiz.Data;
using comaiz.Models;

namespace comaiz.Pages.Contracts
{
    public class DeleteModel : PageModel
    {
        private readonly comaiz.Data.comaizContext _context;

        public DeleteModel(comaiz.Data.comaizContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Contract Contract { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Contracts == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null)
            {
                return NotFound();
            }
            else 
            {
                Contract = contract;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Contracts == null)
            {
                return NotFound();
            }
            var contract = await _context.Contracts.FindAsync(id);

            if (contract != null)
            {
                Contract = contract;
                _context.Contracts.Remove(Contract);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
