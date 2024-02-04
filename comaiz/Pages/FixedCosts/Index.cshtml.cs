using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace comaiz.Pages.FixedCosts
{
    public class IndexModel : PageModel
    {
        private readonly ComaizContext _context;

        public IndexModel(ComaizContext context)
        {
            _context = context;
        }

        public IList<FixedCost> FixedCosts { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.FixedCosts != null)
            {
                FixedCosts = await _context.FixedCosts.ToListAsync();
            }
        }
    }
}
