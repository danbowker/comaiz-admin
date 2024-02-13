using comaiz.data;
using comaiz.data.Models;
using comaiz.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace comaiz.Pages.ContractRates
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
        public ContractRate ContractRate { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.ContractRates == null)
            {
                PopulateContractNameSelectList(_context, ContractRate.ContractId);
                return Page();
            }

            _context.ContractRates.Add(ContractRate);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");

        }
    }
}
