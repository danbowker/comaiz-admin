using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using comaiz.Data;
using comaiz.Models;

namespace comaiz.Pages.Contracts
{
    public class CreateModel : ClientNamePageModel
    {
        private readonly comaiz.Data.ComaizContext _context;

        public CreateModel(comaiz.Data.ComaizContext context)
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
            var contract = new Contract();

            if (await TryUpdateModelAsync<Contract>(
                    contract,
                    "contract",   // Prefix for form value.
                    s => s.Id, s => s.ClientId, s => s.Description, s => s.ChargeType, s => s.Rate, s => s.Price))
            {
                _context.Contracts.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            // Select DepartmentID if TryUpdateModelAsync fails.
            PopulateClientNameSelectList(_context, contract.ClientId);
            return Page();
        }
    }
}
