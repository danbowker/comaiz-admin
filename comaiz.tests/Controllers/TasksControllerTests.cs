using Xunit;
using Microsoft.EntityFrameworkCore;
using comaiz.api.Controllers;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;

namespace comaiz.tests.Controllers
{
    /// <summary>
    /// Unit tests for TasksController using In-Memory Database.
    /// Tests all CRUD operations with success and failure scenarios.
    /// </summary>
    public class TasksControllerTests
    {
        private ComaizContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ComaizContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new ComaizContext(options);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetTasks_ReturnsAllTasks()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.Tasks!.Add(new comaiz.data.Models.Task { Id = 1, Name = "Development" });
            context.Tasks.Add(new comaiz.data.Models.Task { Id = 2, Name = "Testing" });
            await context.SaveChangesAsync();
            
            var controller = new TasksController(context);

            // Act
            var result = await controller.GetTasks(null, null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<comaiz.data.Models.Task>>>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<comaiz.data.Models.Task>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetTask_WithValidId_ReturnsTask()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.Tasks!.Add(new comaiz.data.Models.Task { Id = 1, Name = "Development" });
            await context.SaveChangesAsync();
            
            var controller = new TasksController(context);

            // Act
            var result = await controller.GetTask(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<comaiz.data.Models.Task>>(result);
            var returnValue = Assert.IsType<comaiz.data.Models.Task>(actionResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("Development", returnValue.Name);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetTask_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new TasksController(context);

            // Act
            var result = await controller.GetTask(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostTask_WithValidTask_ReturnsCreatedAtAction()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var task = new comaiz.data.Models.Task { Name = "Documentation" };
            var controller = new TasksController(context);

            // Act
            var result = await controller.PostTask(task);

            // Assert
            var actionResult = Assert.IsType<ActionResult<comaiz.data.Models.Task>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetTask", createdAtActionResult.ActionName);
            
            var savedTask = await context.Tasks!.FindAsync(task.Id);
            Assert.NotNull(savedTask);
            Assert.Equal("Documentation", savedTask.Name);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutTask_WithValidTask_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var task = new comaiz.data.Models.Task { Id = 1, Name = "Development" };
            context.Tasks!.Add(task);
            await context.SaveChangesAsync();
            
            context.Entry(task).State = EntityState.Detached;
            
            var updatedTask = new comaiz.data.Models.Task { Id = 1, Name = "Design" };
            var controller = new TasksController(context);

            // Act
            var result = await controller.PutTask(updatedTask);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var savedTask = await context.Tasks.FindAsync(1);
            Assert.Equal("Design", savedTask!.Name);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutTask_WhenTaskNotExists_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var task = new comaiz.data.Models.Task { Id = 999, Name = "NonExistent" };
            var controller = new TasksController(context);

            // Act
            var result = await controller.PutTask(task);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteTask_WithValidId_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var task = new comaiz.data.Models.Task { Id = 1, Name = "Development" };
            context.Tasks!.Add(task);
            await context.SaveChangesAsync();
            
            var controller = new TasksController(context);

            // Act
            var result = await controller.DeleteTask(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var deletedTask = await context.Tasks.FindAsync(1);
            Assert.Null(deletedTask);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteTask_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new TasksController(context);

            // Act
            var result = await controller.DeleteTask(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostTask_WithTaskContractRates_SavesCollection()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            
            // Create a contract, contract rates, and user contract rates
            var contract = new Contract { Id = 1, ClientId = 1, Description = "Test Contract", ChargeType = ChargeType.TimeAndMaterials };
            context.Contracts!.Add(contract);
            
            var user = new ApplicationUser { Id = "test-user-1", UserName = "testuser", Email = "test@test.com" };
            context.Users!.Add(user);
            
            var contractRate1 = new ContractRate { Id = 1, ContractId = 1, Description = "Developer Rate", InvoiceDescription = "Development", Rate = 100 };
            var contractRate2 = new ContractRate { Id = 2, ContractId = 1, Description = "Tester Rate", InvoiceDescription = "Testing", Rate = 80 };
            context.ContractRates!.Add(contractRate1);
            context.ContractRates.Add(contractRate2);
            await context.SaveChangesAsync();
            
            var userContractRate1 = new UserContractRate { Id = 1, ContractRateId = 1, ApplicationUserId = user.Id };
            var userContractRate2 = new UserContractRate { Id = 2, ContractRateId = 2, ApplicationUserId = user.Id };
            context.UserContractRates!.Add(userContractRate1);
            context.UserContractRates.Add(userContractRate2);
            await context.SaveChangesAsync();

            var task = new comaiz.data.Models.Task 
            { 
                Name = "Project Task",
                ContractId = 1,
                TaskContractRates = new List<TaskContractRate>
                {
                    new TaskContractRate { UserContractRateId = 1 },
                    new TaskContractRate { UserContractRateId = 2 }
                }
            };
            var controller = new TasksController(context);

            // Act
            var result = await controller.PostTask(task);

            // Assert
            var actionResult = Assert.IsType<ActionResult<comaiz.data.Models.Task>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            
            var savedTask = await context.Tasks!
                .Include(t => t.TaskContractRates)
                .FirstOrDefaultAsync(t => t.Id == task.Id);
            Assert.NotNull(savedTask);
            Assert.Equal("Project Task", savedTask.Name);
            Assert.NotNull(savedTask.TaskContractRates);
            Assert.Equal(2, savedTask.TaskContractRates.Count);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutTask_UpdatesTaskContractRates()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            
            // Create contract, contract rates, and user contract rates
            var contract = new Contract { Id = 1, ClientId = 1, Description = "Test Contract", ChargeType = ChargeType.TimeAndMaterials };
            context.Contracts!.Add(contract);
            
            var user = new ApplicationUser { Id = "test-user-2", UserName = "testuser2", Email = "test2@test.com" };
            context.Users!.Add(user);
            
            var contractRate1 = new ContractRate { Id = 1, ContractId = 1, Description = "Developer Rate", InvoiceDescription = "Development", Rate = 100 };
            var contractRate2 = new ContractRate { Id = 2, ContractId = 1, Description = "Tester Rate", InvoiceDescription = "Testing", Rate = 80 };
            var contractRate3 = new ContractRate { Id = 3, ContractId = 1, Description = "Designer Rate", InvoiceDescription = "Design", Rate = 90 };
            context.ContractRates!.Add(contractRate1);
            context.ContractRates.Add(contractRate2);
            context.ContractRates.Add(contractRate3);
            await context.SaveChangesAsync();
            
            var userContractRate1 = new UserContractRate { Id = 1, ContractRateId = 1, ApplicationUserId = user.Id };
            var userContractRate2 = new UserContractRate { Id = 2, ContractRateId = 2, ApplicationUserId = user.Id };
            var userContractRate3 = new UserContractRate { Id = 3, ContractRateId = 3, ApplicationUserId = user.Id };
            context.UserContractRates!.Add(userContractRate1);
            context.UserContractRates.Add(userContractRate2);
            context.UserContractRates.Add(userContractRate3);
            await context.SaveChangesAsync();
            
            var task = new comaiz.data.Models.Task 
            { 
                Id = 1, 
                Name = "Development",
                ContractId = 1,
                TaskContractRates = new List<TaskContractRate>
                {
                    new TaskContractRate { TaskId = 1, UserContractRateId = 1 }
                }
            };
            context.Tasks!.Add(task);
            await context.SaveChangesAsync();
            
            // Update task with new user contract rates
            var updatedTask = new comaiz.data.Models.Task 
            { 
                Id = 1, 
                Name = "Development Updated",
                ContractId = 1,
                TaskContractRates = new List<TaskContractRate>
                {
                    new TaskContractRate { TaskId = 1, UserContractRateId = 2 },
                    new TaskContractRate { TaskId = 1, UserContractRateId = 3 }
                }
            };
            var controller = new TasksController(context);

            // Act
            var result = await controller.PutTask(updatedTask);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var savedTask = await context.Tasks
                .Include(t => t.TaskContractRates)
                .FirstOrDefaultAsync(t => t.Id == 1);
            Assert.NotNull(savedTask);
            Assert.Equal("Development Updated", savedTask.Name);
            Assert.NotNull(savedTask.TaskContractRates);
            Assert.Equal(2, savedTask.TaskContractRates.Count);
            Assert.Contains(savedTask.TaskContractRates, tcr => tcr.UserContractRateId == 2);
            Assert.Contains(savedTask.TaskContractRates, tcr => tcr.UserContractRateId == 3);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetTask_ReturnsTaskWithContractRates()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            
            var contract = new Contract { Id = 1, ClientId = 1, Description = "Test Contract", ChargeType = ChargeType.TimeAndMaterials };
            context.Contracts!.Add(contract);
            
            var user = new ApplicationUser { Id = "test-user-3", UserName = "testuser3", Email = "test3@test.com" };
            context.Users!.Add(user);
            
            var contractRate = new ContractRate { Id = 1, ContractId = 1, Description = "Developer Rate", InvoiceDescription = "Development", Rate = 100 };
            context.ContractRates!.Add(contractRate);
            await context.SaveChangesAsync();
            
            var userContractRate = new UserContractRate { Id = 1, ContractRateId = 1, ApplicationUserId = user.Id };
            context.UserContractRates!.Add(userContractRate);
            
            var task = new comaiz.data.Models.Task 
            { 
                Id = 1, 
                Name = "Development",
                ContractId = 1,
                TaskContractRates = new List<TaskContractRate>
                {
                    new TaskContractRate { TaskId = 1, UserContractRateId = 1 }
                }
            };
            context.Tasks!.Add(task);
            await context.SaveChangesAsync();
            
            var controller = new TasksController(context);

            // Act
            var result = await controller.GetTask(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<comaiz.data.Models.Task>>(result);
            var returnValue = Assert.IsType<comaiz.data.Models.Task>(actionResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.NotNull(returnValue.TaskContractRates);
            Assert.Single(returnValue.TaskContractRates);
        }

        [Fact]
        public async System.Threading.Tasks.Task DuplicateTask_WithValidId_ReturnsCreatedAtAction()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var task = new comaiz.data.Models.Task
            {
                Id = 1,
                Name = "Development Task",
                ContractId = 1,
                ContractRateId = 1
            };
            context.Tasks!.Add(task);
            await context.SaveChangesAsync();

            var controller = new TasksController(context);

            // Act
            var result = await controller.DuplicateTask(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<comaiz.data.Models.Task>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetTask", createdAtActionResult.ActionName);

            var duplicatedTask = Assert.IsType<comaiz.data.Models.Task>(createdAtActionResult.Value);
            Assert.NotEqual(1, duplicatedTask.Id);
            Assert.Equal("Development Task (Copy)", duplicatedTask.Name);
            Assert.Equal(1, duplicatedTask.ContractId);
            Assert.Equal(1, duplicatedTask.ContractRateId);

            // Verify both tasks exist in the database
            var allTasks = await context.Tasks.ToListAsync();
            Assert.Equal(2, allTasks.Count);
        }

        [Fact]
        public async System.Threading.Tasks.Task DuplicateTask_WithTaskContractRates_CopiesCollection()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            var contract = new Contract { Id = 1, ClientId = 1, Description = "Test Contract", ChargeType = ChargeType.TimeAndMaterials };
            context.Contracts!.Add(contract);

            var user = new ApplicationUser { Id = "test-user-dup", UserName = "testuser", Email = "test@test.com" };
            context.Users!.Add(user);

            var contractRate1 = new ContractRate { Id = 1, ContractId = 1, Description = "Developer Rate", InvoiceDescription = "Development", Rate = 100 };
            var contractRate2 = new ContractRate { Id = 2, ContractId = 1, Description = "Tester Rate", InvoiceDescription = "Testing", Rate = 80 };
            context.ContractRates!.Add(contractRate1);
            context.ContractRates.Add(contractRate2);
            await context.SaveChangesAsync();

            var userContractRate1 = new UserContractRate { Id = 1, ContractRateId = 1, ApplicationUserId = user.Id };
            var userContractRate2 = new UserContractRate { Id = 2, ContractRateId = 2, ApplicationUserId = user.Id };
            context.UserContractRates!.Add(userContractRate1);
            context.UserContractRates.Add(userContractRate2);

            var task = new comaiz.data.Models.Task
            {
                Id = 1,
                Name = "Development",
                ContractId = 1,
                TaskContractRates = new List<TaskContractRate>
                {
                    new TaskContractRate { TaskId = 1, UserContractRateId = 1 },
                    new TaskContractRate { TaskId = 1, UserContractRateId = 2 }
                }
            };
            context.Tasks!.Add(task);
            await context.SaveChangesAsync();

            var controller = new TasksController(context);

            // Act
            var result = await controller.DuplicateTask(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<comaiz.data.Models.Task>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);

            var duplicatedTask = Assert.IsType<comaiz.data.Models.Task>(createdAtActionResult.Value);

            // Load the duplicated task with its TaskContractRates
            var savedTask = await context.Tasks
                .Include(t => t.TaskContractRates)
                .FirstOrDefaultAsync(t => t.Id == duplicatedTask.Id);

            Assert.NotNull(savedTask);
            Assert.Equal("Development (Copy)", savedTask.Name);
            Assert.NotNull(savedTask.TaskContractRates);
            Assert.Equal(2, savedTask.TaskContractRates.Count);
            Assert.Contains(savedTask.TaskContractRates, tcr => tcr.UserContractRateId == 1);
            Assert.Contains(savedTask.TaskContractRates, tcr => tcr.UserContractRateId == 2);
        }

        [Fact]
        public async System.Threading.Tasks.Task DuplicateTask_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new TasksController(context);

            // Act
            var result = await controller.DuplicateTask(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostTask_ToCompleteContract_ReturnsBadRequest()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contract = new Contract { Id = 1, ClientId = 1, Description = "Complete Contract", State = RecordState.Complete };
            context.Contracts!.Add(contract);
            await context.SaveChangesAsync();

            var task = new comaiz.data.Models.Task { Name = "New Task", ContractId = 1 };
            var controller = new TasksController(context);

            // Act
            var result = await controller.PostTask(task);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Cannot add tasks to a complete contract.", badRequestResult.Value);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostTask_ToActiveContract_Succeeds()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contract = new Contract { Id = 1, ClientId = 1, Description = "Active Contract", State = RecordState.Active };
            context.Contracts!.Add(contract);
            await context.SaveChangesAsync();

            var task = new comaiz.data.Models.Task { Name = "New Task", ContractId = 1 };
            var controller = new TasksController(context);

            // Act
            var result = await controller.PostTask(task);

            // Assert
            var actionResult = Assert.IsType<ActionResult<comaiz.data.Models.Task>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetTask", createdAtActionResult.ActionName);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetTasks_WithStateFilter_ReturnsFilteredTasks()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var activeContract = new Contract { Id = 1, ClientId = 1, Description = "Active Contract", State = RecordState.Active };
            var completeContract = new Contract { Id = 2, ClientId = 1, Description = "Complete Contract", State = RecordState.Complete };
            context.Contracts!.Add(activeContract);
            context.Contracts.Add(completeContract);
            
            context.Tasks!.Add(new comaiz.data.Models.Task { Id = 1, Name = "Active Task", ContractId = 1, State = RecordState.Active });
            context.Tasks.Add(new comaiz.data.Models.Task { Id = 2, Name = "Complete Task", State = RecordState.Complete });
            context.Tasks.Add(new comaiz.data.Models.Task { Id = 3, Name = "Task in Complete Contract", ContractId = 2, State = RecordState.Active });
            await context.SaveChangesAsync();
            
            var controller = new TasksController(context);

            // Act - Filter for active tasks
            var resultActive = await controller.GetTasks(null, RecordState.Active);
            var activeTasks = Assert.IsAssignableFrom<IEnumerable<comaiz.data.Models.Task>>(resultActive.Value);
            
            // Act - Filter for complete tasks
            var resultComplete = await controller.GetTasks(null, RecordState.Complete);
            var completeTasks = Assert.IsAssignableFrom<IEnumerable<comaiz.data.Models.Task>>(resultComplete.Value);

            // Assert - only truly active task should be returned (task 1)
            Assert.Single(activeTasks);
            Assert.Equal(1, activeTasks.First().Id);
            
            // Assert - complete tasks include task marked as complete and task in complete contract (tasks 2 and 3)
            Assert.Equal(2, completeTasks.Count());
            Assert.Contains(completeTasks, t => t.Id == 2);
            Assert.Contains(completeTasks, t => t.Id == 3);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostTask_DefaultStateIsActive()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var task = new comaiz.data.Models.Task { Name = "New Task" };
            var controller = new TasksController(context);

            // Act
            var result = await controller.PostTask(task);

            // Assert
            var savedTask = await context.Tasks!.FindAsync(task.Id);
            Assert.NotNull(savedTask);
            Assert.Equal(RecordState.Active, savedTask.State);
        }
    }
}
