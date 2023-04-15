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
            if (!ModelState.IsValid || _context.Contracts == null || Contract == null)
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
