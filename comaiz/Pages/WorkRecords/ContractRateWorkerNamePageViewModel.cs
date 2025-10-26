using comaiz.data;
using comaiz.data.Models;
using comaiz.Pages.FixedCosts;
using comaiz.Pages.Shared;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace comaiz.Pages.WorkRecords
{
    public class ContractRateApplicationUserPageViewModel : ContractNamePageModel
    {
        public SelectList? RateSelectList { get; set; }
        public SelectList? ApplicationUserSelectList { get; set; }

        public void PopulateRateSelectList(ComaizContext context, object? selectedRate = null)
        {
            if (context.ContractRates != null)
            {
                var rateQuery = context.ContractRates.OrderBy(c => c.Contract);

                RateSelectList = new SelectList(rateQuery.AsNoTracking(),
                    nameof(ContractRate.Id),
                    nameof(ContractRate.Rate),
                    selectedRate);
            }
        }

        public void PopulateApplicationUserSelectList(ComaizContext context, object? selectedUser = null)
        {
            if (context.Users != null)
            {
                var userQuery = context.Users.OrderBy(u => u.UserName);

                ApplicationUserSelectList = new SelectList(userQuery.AsNoTracking(),
                    nameof(ApplicationUser.Id),
                    nameof(ApplicationUser.UserName),
                    selectedUser);
            }
        }
    }
}
