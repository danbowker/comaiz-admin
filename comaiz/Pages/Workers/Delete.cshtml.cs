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
    public class DeleteModel : PageModel
    {
        private readonly comaiz.Data.ComaizContext _context;

        public DeleteModel(comaiz.Data.ComaizContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Worker Worker { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Workers == null)
            {
                return NotFound();
            }

            var worker = await _context.Workers.FirstOrDefaultAsync(m => m.Id == id);

            if (worker == null)
            {
                return NotFound();
            }
            else 
            {
                Worker = worker;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Workers == null)
            {
                return NotFound();
            }
            var worker = await _context.Workers.FindAsync(id);

            if (worker != null)
            {
                Worker = worker;
                _context.Workers.Remove(Worker);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
