using Xunit;
using Microsoft.EntityFrameworkCore;
using comaiz.api.Controllers;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;

namespace comaiz.tests.Controllers
{
    /// <summary>
    /// Unit tests for WorkRecordsController using In-Memory Database.
    /// Tests all CRUD operations with success and failure scenarios.
    /// </summary>
    public class WorkRecordsControllerTests
    {
        private ComaizContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ComaizContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new ComaizContext(options);
        }

        [Fact]
        public async Task GetWorkRecords_ReturnsAllWorkRecords()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            
            context.WorkRecords!.Add(new WorkRecord { Id = 1, StartDate = startDate, EndDate = endDate, Hours = 8m, WorkerId = 1, ContractId = 1 });
            context.WorkRecords.Add(new WorkRecord { Id = 2, StartDate = startDate, EndDate = endDate, Hours = 6m, WorkerId = 2, ContractId = 1 });
            await context.SaveChangesAsync();
            
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.GetWorkRecords();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<WorkRecord>>>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<WorkRecord>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetWorkRecord_WithValidId_ReturnsWorkRecord()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            
            context.WorkRecords!.Add(new WorkRecord { Id = 1, StartDate = startDate, EndDate = endDate, Hours = 8m, WorkerId = 1, ContractId = 1 });
            await context.SaveChangesAsync();
            
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.GetWorkRecord(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<WorkRecord>>(result);
            var returnValue = Assert.IsType<WorkRecord>(actionResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal(8m, returnValue.Hours);
        }

        [Fact]
        public async Task GetWorkRecord_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.GetWorkRecord(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostWorkRecord_WithValidWorkRecord_ReturnsCreatedAtAction()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            
            var workRecord = new WorkRecord { StartDate = startDate, EndDate = endDate, Hours = 8m, WorkerId = 1, ContractId = 1 };
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.PostWorkRecord(workRecord);

            // Assert
            var actionResult = Assert.IsType<ActionResult<WorkRecord>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetWorkRecord", createdAtActionResult.ActionName);
            
            var savedWorkRecord = await context.WorkRecords!.FindAsync(workRecord.Id);
            Assert.NotNull(savedWorkRecord);
            Assert.Equal(8m, savedWorkRecord.Hours);
        }

        [Fact]
        public async Task PostWorkRecord_WithApplicationUser_SavesApplicationUserId()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var testUserId = "test-user-123";
            
            var workRecord = new WorkRecord 
            { 
                StartDate = startDate, 
                EndDate = endDate, 
                Hours = 8m, 
                WorkerId = 1, 
                ContractId = 1,
                ApplicationUserId = testUserId
            };
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.PostWorkRecord(workRecord);

            // Assert
            var actionResult = Assert.IsType<ActionResult<WorkRecord>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetWorkRecord", createdAtActionResult.ActionName);
            
            var savedWorkRecord = await context.WorkRecords!.FindAsync(workRecord.Id);
            Assert.NotNull(savedWorkRecord);
            Assert.Equal(testUserId, savedWorkRecord.ApplicationUserId);
        }

        [Fact]
        public async Task PutWorkRecord_WithValidWorkRecord_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            
            var workRecord = new WorkRecord { Id = 1, StartDate = startDate, EndDate = endDate, Hours = 8m, WorkerId = 1, ContractId = 1 };
            context.WorkRecords!.Add(workRecord);
            await context.SaveChangesAsync();
            
            context.Entry(workRecord).State = EntityState.Detached;
            
            var updatedWorkRecord = new WorkRecord { Id = 1, StartDate = startDate, EndDate = endDate, Hours = 10m, WorkerId = 1, ContractId = 1 };
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.PutWorkRecord(updatedWorkRecord);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var savedWorkRecord = await context.WorkRecords.FindAsync(1);
            Assert.Equal(10m, savedWorkRecord!.Hours);
        }

        [Fact]
        public async Task PutWorkRecord_CanUpdateApplicationUserId()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var originalUserId = "user-1";
            var newUserId = "user-2";
            
            var workRecord = new WorkRecord 
            { 
                Id = 1, 
                StartDate = startDate, 
                EndDate = endDate, 
                Hours = 8m, 
                WorkerId = 1, 
                ContractId = 1,
                ApplicationUserId = originalUserId
            };
            context.WorkRecords!.Add(workRecord);
            await context.SaveChangesAsync();
            
            context.Entry(workRecord).State = EntityState.Detached;
            
            var updatedWorkRecord = new WorkRecord 
            { 
                Id = 1, 
                StartDate = startDate, 
                EndDate = endDate, 
                Hours = 8m, 
                WorkerId = 1, 
                ContractId = 1,
                ApplicationUserId = newUserId
            };
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.PutWorkRecord(updatedWorkRecord);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var savedWorkRecord = await context.WorkRecords.FindAsync(1);
            Assert.Equal(newUserId, savedWorkRecord!.ApplicationUserId);
        }

        [Fact]
        public async Task PutWorkRecord_WhenWorkRecordNotExists_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            
            var workRecord = new WorkRecord { Id = 999, StartDate = startDate, EndDate = endDate, Hours = 8m, WorkerId = 1, ContractId = 1 };
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.PutWorkRecord(workRecord);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteWorkRecord_WithValidId_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            
            var workRecord = new WorkRecord { Id = 1, StartDate = startDate, EndDate = endDate, Hours = 8m, WorkerId = 1, ContractId = 1 };
            context.WorkRecords!.Add(workRecord);
            await context.SaveChangesAsync();
            
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.DeleteWorkRecord(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var deletedWorkRecord = await context.WorkRecords.FindAsync(1);
            Assert.Null(deletedWorkRecord);
        }

        [Fact]
        public async Task DeleteWorkRecord_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.DeleteWorkRecord(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
