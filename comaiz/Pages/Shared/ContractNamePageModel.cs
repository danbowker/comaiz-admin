using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace comaiz.Pages.Shared
{
    public class ContractNamePageModel : PageModel
    {
        public SelectList? ContractsNameSelectList { get; set; }

        public void PopulateContractNameSelectList(ComaizContext context, object? selectedContract = null)
        {
            if (context.Contracts != null)
            {
                var contractQuery = context.Contracts.OrderBy(c => c.Id);

                ContractsNameSelectList = new SelectList(contractQuery.AsNoTracking(),
                    nameof(Contract.Id),
                    nameof(Contract.Description),
                    selectedContract);
            }
        }

    }
}
