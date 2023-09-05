using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using comaiz.Models;

namespace comaiz.Pages.WorkRecords
{
    public class IndexModel : PageModel
    {
        private readonly Data.ComaizContext _context;

        public IndexModel(Data.ComaizContext context)
        {
            _context = context;
        }

        public IList<WorkRecord> WorkRecord { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.WorkRecords != null)
            {
                WorkRecord = await _context.WorkRecords
                    .Include(w => w.Cost)
                    .Include(w => w.Worker)
                    .ToListAsync();
            }
        }
    }
}
