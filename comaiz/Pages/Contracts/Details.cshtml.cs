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
    public class DetailsModel : PageModel
    {
        private readonly comaiz.Data.comaizContext _context;

        public DetailsModel(comaiz.Data.comaizContext context)
        {
            _context = context;
        }

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
    }
}
