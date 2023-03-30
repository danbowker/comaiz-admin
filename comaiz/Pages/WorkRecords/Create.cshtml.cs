using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using comaiz.Data;
using comaiz.Models;

namespace comaiz.Pages.WorkRecords
{
    public class CreateModel : PageModel
    {
        private readonly comaiz.Data.comaizContext _context;

        public CreateModel(comaiz.Data.comaizContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public WorkRecord WorkRecord { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.WorkRecords == null || WorkRecord == null)
            {
                return Page();
            }

            _context.WorkRecords.Add(WorkRecord);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
