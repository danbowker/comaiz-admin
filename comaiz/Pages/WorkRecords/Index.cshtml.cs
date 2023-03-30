using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using comaiz.Data;
using comaiz.Models;

namespace comaiz.Pages.WorkRecords
{
    public class IndexModel : PageModel
    {
        private readonly comaiz.Data.comaizContext _context;

        public IndexModel(comaiz.Data.comaizContext context)
        {
            _context = context;
        }

        public IList<WorkRecord> WorkRecord { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.WorkRecords != null)
            {
                WorkRecord = await _context.WorkRecords.ToListAsync();
            }
        }
    }
}
