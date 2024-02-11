using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;

namespace comaiz.Pages.WorkRecords
{
    public class CreateModel : ContractRateWorkerNamePageViewModel
    {
        private readonly ComaizContext _context;

        public CreateModel(ComaizContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            PopulateContractNameSelectList(_context);
            PopulateRateSelectList(_context);
            PopulateWorkerNameSelectList(_context);
            return Page();
        }

        [BindProperty]
        public WorkRecord WorkRecord { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.WorkRecords == null)
          {
              PopulateWorkerNameSelectList(_context, WorkRecord.WorkerId);
              return Page();
          }

          _context.WorkRecords.Add(WorkRecord);
          await _context.SaveChangesAsync();

          return RedirectToPage("./Index");
        }
    }
}
