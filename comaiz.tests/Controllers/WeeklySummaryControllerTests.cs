using Xunit;
using Microsoft.EntityFrameworkCore;
using comaiz.api.Controllers;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;

namespace comaiz.tests.Controllers
{
    /// <summary>
    /// Unit tests for WeeklySummaryController using In-Memory Database.
    /// Tests weekly summary data aggregation and calculations.
    /// </summary>
    public class WeeklySummaryControllerTests
    {
        private ComaizContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ComaizContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new ComaizContext(options);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetWeeklySummary_ReturnsValidResponse()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var user = new ApplicationUser { Id = "test-user-id", UserName = "testuser", Email = "test@example.com" };
            context.Users!.Add(user);
            
            var task = new comaiz.data.Models.Task { Id = 1, Name = "Development", ContractId = 1 };
            context.Tasks!.Add(task);
            
            var today = DateOnly.FromDateTime(DateTime.Today);
            var workRecord = new WorkRecord
            {
                ApplicationUserId = user.Id,
                TaskId = task.Id,
                StartDate = today,
                EndDate = today,
                Hours = 8.0m
            };
            context.WorkRecords!.Add(workRecord);
            await context.SaveChangesAsync();
            
            var controller = new WeeklySummaryController(context);

            // Act
            var result = await controller.GetWeeklySummary(user.Id, null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<WeeklySummaryResponse>>(result);
            var summary = Assert.IsType<WeeklySummaryResponse>(actionResult.Value);
            Assert.NotNull(summary);
            Assert.NotNull(summary.DailySummaries);
            Assert.Equal(7, summary.DailySummaries.Count); // Should have 7 days
            Assert.NotNull(summary.TaskWeeklyTotals);
            Assert.Equal(user.Id, summary.UserId);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetWeeklySummary_WithNoData_ReturnsEmptySummary()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new WeeklySummaryController(context);
            var nonExistentUserId = "non-existent-user-id";

            // Act
            var result = await controller.GetWeeklySummary(nonExistentUserId, null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<WeeklySummaryResponse>>(result);
            var summary = Assert.IsType<WeeklySummaryResponse>(actionResult.Value);
            Assert.NotNull(summary);
            Assert.Equal(0, summary.WeekTotalHours);
            Assert.Equal(7, summary.DailySummaries.Count); // Still 7 days even with no data
        }

        [Fact]
        public async System.Threading.Tasks.Task GetWeeklySummary_CalculatesTotalsCorrectly()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var user = new ApplicationUser { Id = "test-user-id", UserName = "testuser", Email = "test@example.com" };
            context.Users!.Add(user);
            
            var task = new comaiz.data.Models.Task { Id = 1, Name = "Development", ContractId = 1 };
            context.Tasks!.Add(task);
            
            // Use a fixed Monday to ensure all days are in the same week
            var monday = new DateOnly(2024, 1, 1); // Jan 1, 2024 is a Monday
            
            // Add records across multiple days in the same week
            var records = new[]
            {
                new WorkRecord { ApplicationUserId = user.Id, TaskId = task.Id, StartDate = monday, EndDate = monday, Hours = 4.5m },
                new WorkRecord { ApplicationUserId = user.Id, TaskId = task.Id, StartDate = monday.AddDays(1), EndDate = monday.AddDays(1), Hours = 5.0m },
                new WorkRecord { ApplicationUserId = user.Id, TaskId = task.Id, StartDate = monday.AddDays(2), EndDate = monday.AddDays(2), Hours = 6.5m }
            };
            
            context.WorkRecords!.AddRange(records);
            await context.SaveChangesAsync();
            
            var controller = new WeeklySummaryController(context);

            // Act - Pass the same week start date
            var result = await controller.GetWeeklySummary(user.Id, monday);

            // Assert
            var actionResult = Assert.IsType<ActionResult<WeeklySummaryResponse>>(result);
            var summary = Assert.IsType<WeeklySummaryResponse>(actionResult.Value);
            Assert.NotNull(summary);
            Assert.Equal(16.0m, summary.WeekTotalHours); // 4.5 + 5.0 + 6.5 = 16.0
        }

        [Fact]
        public async System.Threading.Tasks.Task GetWeeklySummary_WithMultipleTasks_AggregatesCorrectly()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var user = new ApplicationUser { Id = "test-user-id", UserName = "testuser", Email = "test@example.com" };
            context.Users!.Add(user);
            
            var task1 = new comaiz.data.Models.Task { Id = 1, Name = "Development", ContractId = 1 };
            var task2 = new comaiz.data.Models.Task { Id = 2, Name = "Testing", ContractId = 1 };
            context.Tasks!.AddRange(task1, task2);
            
            var today = DateOnly.FromDateTime(DateTime.Today);
            
            // Add records for different tasks on the same day
            var records = new[]
            {
                new WorkRecord { ApplicationUserId = user.Id, TaskId = task1.Id, StartDate = today, EndDate = today, Hours = 4.0m },
                new WorkRecord { ApplicationUserId = user.Id, TaskId = task2.Id, StartDate = today, EndDate = today, Hours = 3.0m }
            };
            
            context.WorkRecords!.AddRange(records);
            await context.SaveChangesAsync();
            
            var controller = new WeeklySummaryController(context);

            // Act
            var result = await controller.GetWeeklySummary(user.Id, null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<WeeklySummaryResponse>>(result);
            var summary = Assert.IsType<WeeklySummaryResponse>(actionResult.Value);
            Assert.NotNull(summary);
            Assert.Equal(7.0m, summary.WeekTotalHours); // 4.0 + 3.0 = 7.0
            Assert.Equal(2, summary.TaskWeeklyTotals.Count); // Two different tasks
        }
    }
}
