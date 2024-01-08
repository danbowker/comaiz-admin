using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace comaiz.Pages.WorkRecords
{
    public class IndexModel : PageModel
    {
        private readonly ComaizContext _context;

        public IndexModel(ComaizContext context)
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
