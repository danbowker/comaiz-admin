﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using comaiz.Data;
using comaiz.Models;

namespace comaiz.Pages.Clients
{
    public class CreateModel : PageModel
    {
        private readonly comaiz.Data.ComaizContext _context;

        public CreateModel(comaiz.Data.ComaizContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Client Client { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Clients == null || Client == null)
          {
              return Page();
          }

          _context.Clients.Add(Client);
          await _context.SaveChangesAsync();

          return RedirectToPage("./Index");
        }
    }
}
