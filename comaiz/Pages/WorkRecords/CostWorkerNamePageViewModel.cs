using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace comaiz.Pages.WorkRecords
{
    public class CostWorkerNamePageViewModel : PageModel
    {
        public SelectList? CostsNameSelectList { get; set; }
        public SelectList? WorkerNameSelectList { get; set; }

        public void PopulateCostNameSelectList(ComaizContext context, object? selectedContract = null)
        {
            if (context.Costs != null)
            {
                var costsQuery = context.Costs.OrderBy(c => c.Id);

                CostsNameSelectList = new SelectList(costsQuery.AsNoTracking(),
                    nameof(Cost.Id),
                    nameof(Cost.Name),
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
