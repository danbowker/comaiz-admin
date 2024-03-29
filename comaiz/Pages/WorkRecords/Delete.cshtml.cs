﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace comaiz.Pages.WorkRecords
{
    public class DeleteModel : PageModel
    {
        private readonly ComaizContext _context;

        public DeleteModel(ComaizContext context)
        {
            _context = context;
        }

        [BindProperty]
      public WorkRecord WorkRecord { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.WorkRecords == null)
            {
                return NotFound();
            }

            var workrecord = await _context.WorkRecords.FirstOrDefaultAsync(m => m.Id == id);

            if (workrecord == null)
            {
                return NotFound();
            }
            else 
            {
                WorkRecord = workrecord;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.WorkRecords == null)
            {
                return NotFound();
            }
            var workrecord = await _context.WorkRecords.FindAsync(id);

            if (workrecord != null)
            {
                WorkRecord = workrecord;
                _context.WorkRecords.Remove(WorkRecord);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
