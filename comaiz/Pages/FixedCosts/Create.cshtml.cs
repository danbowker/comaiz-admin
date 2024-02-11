using comaiz.data;
using comaiz.data.Models;
using comaiz.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace comaiz.Pages.FixedCosts
{
    public class CreateModel : ContractNamePageModel
    {
        private readonly ComaizContext _context;

        public CreateModel(ComaizContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            PopulateContractNameSelectList(_context);
            return Page();
        }

        [BindProperty]
        public FixedCost FixedCost { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.FixedCosts == null)
            {
                PopulateContractNameSelectList(_context, FixedCost.ContractId);
                return Page();
            }

            _context.FixedCosts.Add(FixedCost);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
