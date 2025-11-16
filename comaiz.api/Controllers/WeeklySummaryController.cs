using Microsoft.AspNetCore.Mvc;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace comaiz.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeeklySummaryController : ControllerBase
    {
        private readonly ComaizContext dbContext;

        public WeeklySummaryController(ComaizContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        public async Task<ActionResult<WeeklySummaryResponse>> GetWeeklySummary(
            [FromQuery] string? userId,
            [FromQuery] DateOnly? weekStartDate)
        {
            if (dbContext.WorkRecords == null) 
                return StatusCode(StatusCodes.Status500InternalServerError);

            // Default to current week if no date provided
            var startDate = weekStartDate ?? GetStartOfWeek(DateOnly.FromDateTime(DateTime.Today));
            var endDate = startDate.AddDays(6);

            // Build query
            var query = dbContext.WorkRecords
                .Include(wr => wr.Task)
                .Include(wr => wr.ApplicationUser)
                .Where(wr => wr.StartDate >= startDate && wr.StartDate <= endDate);

            // Filter by user if provided
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(wr => wr.ApplicationUserId == userId);
            }

            var workRecords = await query.ToListAsync();

            // Group and aggregate data
            var summary = new WeeklySummaryResponse
            {
                WeekStartDate = startDate,
                WeekEndDate = endDate,
                UserId = userId,
                DailySummaries = new List<DailySummary>()
            };

            // Create summaries for each day of the week
            for (int i = 0; i < 7; i++)
            {
                var currentDate = startDate.AddDays(i);
                var dayRecords = workRecords.Where(wr => wr.StartDate == currentDate).ToList();

                var dailySummary = new DailySummary
                {
                    Date = currentDate,
                    TaskHours = dayRecords
                        .GroupBy(wr => new { wr.TaskId, TaskName = wr.Task?.Name ?? "No Task" })
                        .Select(g => new TaskHours
                        {
                            TaskId = g.Key.TaskId,
                            TaskName = g.Key.TaskName,
                            Hours = g.Sum(wr => wr.Hours)
                        })
                        .ToList(),
                    TotalHours = dayRecords.Sum(wr => wr.Hours)
                };

                summary.DailySummaries.Add(dailySummary);
            }

            // Calculate task totals per week
            summary.TaskWeeklyTotals = workRecords
                .GroupBy(wr => new { wr.TaskId, TaskName = wr.Task?.Name ?? "No Task" })
                .Select(g => new TaskHours
                {
                    TaskId = g.Key.TaskId,
                    TaskName = g.Key.TaskName,
                    Hours = g.Sum(wr => wr.Hours)
                })
                .ToList();

            // Calculate total hours per week
            summary.WeekTotalHours = workRecords.Sum(wr => wr.Hours);

            return summary;
        }

        private static DateOnly GetStartOfWeek(DateOnly date)
        {
            // Get Monday as start of week
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff);
        }
    }

    // Response models
    public class WeeklySummaryResponse
    {
        public DateOnly WeekStartDate { get; set; }
        public DateOnly WeekEndDate { get; set; }
        public string? UserId { get; set; }
        public List<DailySummary> DailySummaries { get; set; } = new();
        public List<TaskHours> TaskWeeklyTotals { get; set; } = new();
        public decimal WeekTotalHours { get; set; }
    }

    public class DailySummary
    {
        public DateOnly Date { get; set; }
        public List<TaskHours> TaskHours { get; set; } = new();
        public decimal TotalHours { get; set; }
    }

    public class TaskHours
    {
        public int? TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public decimal Hours { get; set; }
    }
}
