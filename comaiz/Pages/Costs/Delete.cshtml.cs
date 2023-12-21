﻿using System;
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
    public class DeleteModel : PageModel
    {
        private readonly comaiz.Data.ComaizContext _context;

        public DeleteModel(comaiz.Data.ComaizContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Costs == null)
            {
                return NotFound();
            }
            var cost = await _context.Costs.FindAsync(id);

            if (cost != null)
            {
                Cost = cost;
                _context.Costs.Remove(Cost);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}