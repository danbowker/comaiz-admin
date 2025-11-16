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
        public async System.Threading.Tasks.Task GetWorkRecords_ReturnsAllWorkRecords()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            
            context.WorkRecords!.Add(new WorkRecord { Id = 1, StartDate = startDate, EndDate = endDate, Hours = 8m, ApplicationUserId = "user-1" });
            context.WorkRecords.Add(new WorkRecord { Id = 2, StartDate = startDate, EndDate = endDate, Hours = 6m, ApplicationUserId = "user-2" });
            await context.SaveChangesAsync();
            
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.GetWorkRecords(null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<WorkRecord>>>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<WorkRecord>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetWorkRecord_WithValidId_ReturnsWorkRecord()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            
            context.WorkRecords!.Add(new WorkRecord { Id = 1, StartDate = startDate, EndDate = endDate, Hours = 8m, ApplicationUserId = "user-1" });
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
        public async System.Threading.Tasks.Task GetWorkRecord_WithInvalidId_ReturnsNotFound()
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
        public async System.Threading.Tasks.Task PostWorkRecord_WithValidWorkRecord_ReturnsCreatedAtAction()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            
            var workRecord = new WorkRecord { StartDate = startDate, EndDate = endDate, Hours = 8m, ApplicationUserId = "user-1" };
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
        public async System.Threading.Tasks.Task PostWorkRecord_WithApplicationUser_SavesApplicationUserId()
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
                ApplicationUserId = testUserId,
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
        public async System.Threading.Tasks.Task PutWorkRecord_WithValidWorkRecord_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            
            var workRecord = new WorkRecord { Id = 1, StartDate = startDate, EndDate = endDate, Hours = 8m, ApplicationUserId = "user-1" };
            context.WorkRecords!.Add(workRecord);
            await context.SaveChangesAsync();
            
            context.Entry(workRecord).State = EntityState.Detached;
            
            var updatedWorkRecord = new WorkRecord { Id = 1, StartDate = startDate, EndDate = endDate, Hours = 10m, ApplicationUserId = "user-1" };
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.PutWorkRecord(updatedWorkRecord);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var savedWorkRecord = await context.WorkRecords.FindAsync(1);
            Assert.Equal(10m, savedWorkRecord!.Hours);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutWorkRecord_CanUpdateApplicationUserId()
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
                ApplicationUserId = originalUserId,
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
                ApplicationUserId = newUserId,
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
        public async System.Threading.Tasks.Task PutWorkRecord_WhenWorkRecordNotExists_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            
            var workRecord = new WorkRecord { Id = 999, StartDate = startDate, EndDate = endDate, Hours = 8m, ApplicationUserId = "user-1" };
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.PutWorkRecord(workRecord);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteWorkRecord_WithValidId_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            
            var workRecord = new WorkRecord { Id = 1, StartDate = startDate, EndDate = endDate, Hours = 8m, ApplicationUserId = "user-1" };
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
        public async System.Threading.Tasks.Task DeleteWorkRecord_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.DeleteWorkRecord(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostWorkRecord_ToCompleteTask_ReturnsBadRequest()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var task = new comaiz.data.Models.Task { Id = 1, Name = "Complete Task", State = RecordState.Complete };
            context.Tasks!.Add(task);
            await context.SaveChangesAsync();

            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var workRecord = new WorkRecord { StartDate = startDate, EndDate = endDate, Hours = 8m, ApplicationUserId = "user-1", TaskId = 1 };
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.PostWorkRecord(workRecord);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Cannot add work records to a complete task.", badRequestResult.Value);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostWorkRecord_ToTaskWithCompleteContract_ReturnsBadRequest()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contract = new Contract { Id = 1, ClientId = 1, Description = "Complete Contract", State = RecordState.Complete };
            context.Contracts!.Add(contract);
            
            var task = new comaiz.data.Models.Task { Id = 1, Name = "Active Task", ContractId = 1, State = RecordState.Active };
            context.Tasks!.Add(task);
            await context.SaveChangesAsync();

            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var workRecord = new WorkRecord { StartDate = startDate, EndDate = endDate, Hours = 8m, ApplicationUserId = "user-1", TaskId = 1 };
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.PostWorkRecord(workRecord);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Cannot add work records to a task whose contract is complete.", badRequestResult.Value);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostWorkRecord_ToActiveTask_Succeeds()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contract = new Contract { Id = 1, ClientId = 1, Description = "Active Contract", State = RecordState.Active };
            context.Contracts!.Add(contract);
            
            var task = new comaiz.data.Models.Task { Id = 1, Name = "Active Task", ContractId = 1, State = RecordState.Active };
            context.Tasks!.Add(task);
            await context.SaveChangesAsync();

            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var workRecord = new WorkRecord { StartDate = startDate, EndDate = endDate, Hours = 8m, ApplicationUserId = "user-1", TaskId = 1 };
            var controller = new WorkRecordsController(context);

            // Act
            var result = await controller.PostWorkRecord(workRecord);

            // Assert
            var actionResult = Assert.IsType<ActionResult<WorkRecord>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetWorkRecord", createdAtActionResult.ActionName);
        }
    }
}
