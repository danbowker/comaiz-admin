using Microsoft.AspNetCore.Mvc;
using comaiz.Models;

namespace comaiz.Pages.Costs
{
    public class CreateModel : ContractNamePageModel
    {
        private readonly comaiz.Data.ComaizContext _context;

        public CreateModel(comaiz.Data.ComaizContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            PopulateContractNameSelectList(_context);
            return Page();
        }

        [BindProperty]
        public Cost Cost { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Costs == null)
            {
                PopulateContractNameSelectList(_context, Cost.ContractId);
                return Page();
            }

            _context.Costs.Add(Cost);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
