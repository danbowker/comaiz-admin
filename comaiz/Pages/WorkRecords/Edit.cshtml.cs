﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using comaiz.Data;
using comaiz.Models;

namespace comaiz.Pages.WorkRecords
{
    public class EditModel : PageModel
    {
        private readonly comaiz.Data.ComaizContext _context;

        public EditModel(comaiz.Data.ComaizContext context)
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

            var workrecord =  await _context.WorkRecords.FirstOrDefaultAsync(m => m.Id == id);
            if (workrecord == null)
            {
                return NotFound();
            }
            WorkRecord = workrecord;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(WorkRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkRecordExists(WorkRecord.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool WorkRecordExists(int id)
        {
          return (_context.WorkRecords?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
