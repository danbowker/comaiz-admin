using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace comaiz.Pages.WorkRecords
{
    public class CreateModel : ContractRateWorkerNamePageViewModel
    {
        private readonly ComaizContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(ComaizContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            PopulateContractNameSelectList(_context);
            PopulateRateSelectList(_context);
            PopulateWorkerNameSelectList(_context);
            PopulateApplicationUserSelectList(_context);
            
            // Set default ApplicationUser to current logged-in user
            if (User.Identity?.IsAuthenticated == true)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    WorkRecord = new WorkRecord
                    {
                        ApplicationUserId = currentUser.Id
                    };
                }
            }
            
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
              PopulateApplicationUserSelectList(_context, WorkRecord.ApplicationUserId);
              return Page();
          }

          _context.WorkRecords.Add(WorkRecord);
          await _context.SaveChangesAsync();

          return RedirectToPage("./Index");
        }
    }
}
