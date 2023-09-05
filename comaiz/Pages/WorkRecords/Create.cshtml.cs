﻿using Microsoft.AspNetCore.Mvc;
using comaiz.Models;

namespace comaiz.Pages.WorkRecords
{
    public class CreateModel : CostWorkerNamePageViewModel
    {
        private readonly Data.ComaizContext _context;

        public CreateModel(Data.ComaizContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            PopulateCostNameSelectList(_context);
            PopulateWorkerNameSelectList(_context);
            return Page();
        }

        [BindProperty]
        public WorkRecord WorkRecord { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.WorkRecords == null)
          {
              PopulateCostNameSelectList(_context, WorkRecord.CostId);
              PopulateWorkerNameSelectList(_context, WorkRecord.WorkerId);
              return Page();
          }

          _context.WorkRecords.Add(WorkRecord);
          await _context.SaveChangesAsync();

          return RedirectToPage("./Index");
        }
    }
}
