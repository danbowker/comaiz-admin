﻿using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comaiz.Pages.WorkRecords
{
    public class EditModel : ContractRateWorkerNamePageViewModel
    {
        private readonly ComaizContext _context;

        public EditModel(ComaizContext context)
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
            PopulateWorkerNameSelectList(_context, workrecord.WorkerId);
            PopulateRateSelectList(_context, workrecord.ContractRateId);
            PopulateContractNameSelectList(_context,workrecord.ContractId);

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
