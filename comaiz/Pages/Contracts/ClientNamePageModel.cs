using comaiz.Data;
using comaiz.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace comaiz.Pages.Contracts
{
    public class ClientNamePageModel : PageModel
    {
        public SelectList ClientNameSelectList { get; set; }

        public void PopulateClientNameSelectList(ComaizContext context, object selectedClient = null)
        {
            var clientQuery = context.Clients.OrderBy(c => c.Name);

            ClientNameSelectList = new SelectList(clientQuery.AsNoTracking(),
                nameof(Client.Id),
                nameof(Client.ShortName),
                selectedClient);
        }
    }
}
