﻿using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace comaiz.Pages.Shared
{
    public class ClientNamePageModel : PageModel
    {
        public SelectList? ClientNameSelectList { get; set; }

        public void PopulateClientNameSelectList(ComaizContext context, object? selectedClient = null)
        {
            var clientQuery = context?.Clients?.OrderBy(c => c.Name);

            ClientNameSelectList = new SelectList(clientQuery?.AsNoTracking(),
                nameof(Client.Id),
                nameof(Client.ShortName),
                selectedClient);
        }
    }
}
