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
    public class DetailsModel : PageModel
    {
        private readonly comaiz.Data.comaizContext _context;

        public DetailsModel(comaiz.Data.comaizContext context)
        {
            _context = context;
        }

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
    }
}
