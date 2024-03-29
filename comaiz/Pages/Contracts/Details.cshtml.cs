﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using comaiz.data.Models;
using comaiz.data;

namespace comaiz.Pages.Contracts
{
    public class DetailsModel : PageModel
    {
        private readonly ComaizContext _context;

        public DetailsModel(ComaizContext context)
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

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
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
