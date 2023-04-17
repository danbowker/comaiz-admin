using Microsoft.AspNetCore.Mvc;
using comaiz.Models;

namespace comaiz.Pages.Contracts
{
    public class CreateModel : ClientNamePageModel
    {
        private readonly Data.ComaizContext _context;

        public CreateModel(Data.ComaizContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            PopulateClientNameSelectList(_context);
            return Page();
        }

        [BindProperty]
        public Contract Contract { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Contracts == null)
            {
                PopulateClientNameSelectList(_context, Contract.ClientId);
                return Page();
            }

            _context.Contracts.Add(Contract);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");

        }
    }
}
