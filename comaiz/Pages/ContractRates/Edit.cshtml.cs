using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using comaiz.data.Models;
using comaiz.data;
using comaiz.Pages.Shared;

namespace comaiz.Pages.ContractRates
{
    public class EditModel : ClientNamePageModel
    {
        private readonly ComaizContext _context;

        public EditModel(ComaizContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ContractRate ContractRate { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.ContractRates == null)
            {
                return NotFound();
            }

            var contractrate =  await _context.ContractRates.Include(c => c.Contract)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contractrate == null)
            {
                return NotFound();
            }
            ContractRate = contractrate;
            PopulateClientNameSelectList(_context);
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

            _context.Attach(ContractRate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContractExists(ContractRate.Id))
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

        private bool ContractExists(int id)
        {
          return (_context.ContractRates?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
