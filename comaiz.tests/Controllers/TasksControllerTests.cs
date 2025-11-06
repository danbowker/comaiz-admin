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
            var result = await controller.GetTasks(null);

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
            
            // Create a contract and contract rates
            var contract = new Contract { Id = 1, ClientId = 1, Description = "Test Contract", ChargeType = ChargeType.TimeAndMaterials };
            context.Contracts!.Add(contract);
            
            var contractRate1 = new ContractRate { Id = 1, ContractId = 1, Description = "Developer Rate", Rate = 100 };
            var contractRate2 = new ContractRate { Id = 2, ContractId = 1, Description = "Tester Rate", Rate = 80 };
            context.ContractRates!.Add(contractRate1);
            context.ContractRates.Add(contractRate2);
            await context.SaveChangesAsync();

            var task = new comaiz.data.Models.Task 
            { 
                Name = "Project Task",
                ContractId = 1,
                TaskContractRates = new List<TaskContractRate>
                {
                    new TaskContractRate { ContractRateId = 1 },
                    new TaskContractRate { ContractRateId = 2 }
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
            
            // Create contract and contract rates
            var contract = new Contract { Id = 1, ClientId = 1, Description = "Test Contract", ChargeType = ChargeType.TimeAndMaterials };
            context.Contracts!.Add(contract);
            
            var contractRate1 = new ContractRate { Id = 1, ContractId = 1, Description = "Developer Rate", Rate = 100 };
            var contractRate2 = new ContractRate { Id = 2, ContractId = 1, Description = "Tester Rate", Rate = 80 };
            var contractRate3 = new ContractRate { Id = 3, ContractId = 1, Description = "Designer Rate", Rate = 90 };
            context.ContractRates!.Add(contractRate1);
            context.ContractRates.Add(contractRate2);
            context.ContractRates.Add(contractRate3);
            
            var task = new comaiz.data.Models.Task 
            { 
                Id = 1, 
                Name = "Development",
                ContractId = 1,
                TaskContractRates = new List<TaskContractRate>
                {
                    new TaskContractRate { TaskId = 1, ContractRateId = 1 }
                }
            };
            context.Tasks!.Add(task);
            await context.SaveChangesAsync();
            
            // Update task with new contract rates
            var updatedTask = new comaiz.data.Models.Task 
            { 
                Id = 1, 
                Name = "Development Updated",
                ContractId = 1,
                TaskContractRates = new List<TaskContractRate>
                {
                    new TaskContractRate { TaskId = 1, ContractRateId = 2 },
                    new TaskContractRate { TaskId = 1, ContractRateId = 3 }
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
            Assert.Contains(savedTask.TaskContractRates, tcr => tcr.ContractRateId == 2);
            Assert.Contains(savedTask.TaskContractRates, tcr => tcr.ContractRateId == 3);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetTask_ReturnsTaskWithContractRates()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            
            var contract = new Contract { Id = 1, ClientId = 1, Description = "Test Contract", ChargeType = ChargeType.TimeAndMaterials };
            context.Contracts!.Add(contract);
            
            var contractRate = new ContractRate { Id = 1, ContractId = 1, Description = "Developer Rate", Rate = 100 };
            context.ContractRates!.Add(contractRate);
            
            var task = new comaiz.data.Models.Task 
            { 
                Id = 1, 
                Name = "Development",
                ContractId = 1,
                TaskContractRates = new List<TaskContractRate>
                {
                    new TaskContractRate { TaskId = 1, ContractRateId = 1 }
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
    }
}
