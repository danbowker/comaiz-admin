using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comaiz.Pages.FixedCosts
{
    public class EditModel : ContractNamePageModel
    {
        private readonly ComaizContext _context;

        public EditModel(ComaizContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FixedCost FixedCost { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.FixedCosts == null)
            {
                return NotFound();
            }

            var fixedCost =  await _context.FixedCosts.FirstOrDefaultAsync(m => m.Id == id);
            if (fixedCost == null)
            {
                return NotFound();
            }
            FixedCost = fixedCost;
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

            _context.Attach(FixedCost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CostExists(FixedCost.Id))
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
          return (_context.FixedCosts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
