using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace comaiz.Pages.Costs
{
    public class EditModel : ContractNamePageModel
    {
        private readonly ComaizContext _context;

        public EditModel(ComaizContext context)
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

            var cost =  await _context.Costs.FirstOrDefaultAsync(m => m.Id == id);
            if (cost == null)
            {
                return NotFound();
            }
            Cost = cost;
            PopulateContractNameSelectList(_context);
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

            _context.Attach(Cost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CostExists(Cost.Id))
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

        private bool CostExists(int id)
        {
          return (_context.Costs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
