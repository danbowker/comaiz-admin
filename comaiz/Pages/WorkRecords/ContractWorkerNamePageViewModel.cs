using comaiz.Data;
using comaiz.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace comaiz.Pages.WorkRecords
{
    public class ContractWorkerNamePageViewModel : PageModel
    {
        public SelectList? ContractNameSelectList { get; set; }
        public SelectList? WorkerNameSelectList { get; set; }

        public void PopulateContractNameSelectList(ComaizContext context, object? selectedContract = null)
        {
            if (context.Contracts != null)
            {
                var contractQuery = context.Contracts.OrderBy(c => c.Id);

                ContractNameSelectList = new SelectList(contractQuery.AsNoTracking(),
                    nameof(Contract.Id),
                    nameof(Contract.Description),
                    selectedContract);
            }
        }

        public void PopulateWorkerNameSelectList(ComaizContext context, object? selectedWorker = null)
        {
            if (context.Workers != null)
            {
                var workerQuery = context.Workers.OrderBy(c => c.Name);

                WorkerNameSelectList = new SelectList(workerQuery.AsNoTracking(),
                    nameof(Worker.Id),
                    nameof(Worker.Name),
                    selectedWorker);
            }
        }
    }
}
