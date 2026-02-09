using Xunit;
using Microsoft.EntityFrameworkCore;
using comaiz.api.Controllers;
using comaiz.api.Models;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;

namespace comaiz.tests.Controllers
{
    /// <summary>
    /// Unit tests for InvoiceItemsController using In-Memory Database.
    /// Tests all CRUD operations with success and failure scenarios.
    /// </summary>
    public class InvoiceItemsControllerTests
    {
        private ComaizContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ComaizContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new ComaizContext(options);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetInvoiceItems_ReturnsAllInvoiceItems()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.InvoiceItems!.Add(new InvoiceItem { Id = 1, InvoiceId = 1, TaskId = 1, Quantity = 10, Rate = 50m, VATRate = 0.20m, Price = 600m });
            context.InvoiceItems.Add(new InvoiceItem { Id = 2, InvoiceId = 1, TaskId = 2, Quantity = 5, Rate = 75m, VATRate = 0.20m, Price = 450m });
            await context.SaveChangesAsync();
            
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.GetInvoiceItems(null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<InvoiceItem>>>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<InvoiceItem>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetInvoiceItem_WithValidId_ReturnsInvoiceItem()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.InvoiceItems!.Add(new InvoiceItem { Id = 1, InvoiceId = 1, TaskId = 1, Quantity = 10, Rate = 50m, VATRate = 0.20m, Price = 600m });
            await context.SaveChangesAsync();
            
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.GetInvoiceItem(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<InvoiceItem>>(result);
            var returnValue = Assert.IsType<InvoiceItem>(actionResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal(10, returnValue.Quantity);
            Assert.Equal(50m, returnValue.Rate);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetInvoiceItem_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.GetInvoiceItem(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostInvoiceItem_WithValidInvoiceItem_ReturnsCreatedAtAction()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var invoiceItem = new InvoiceItem { InvoiceId = 1, TaskId = 1, Quantity = 8, Rate = 60m, VATRate = 0.20m, Price = 576m };
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.PostInvoiceItem(invoiceItem);

            // Assert
            var actionResult = Assert.IsType<ActionResult<InvoiceItem>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetInvoiceItem", createdAtActionResult.ActionName);
            
            var savedInvoiceItem = await context.InvoiceItems!.FindAsync(invoiceItem.Id);
            Assert.NotNull(savedInvoiceItem);
            Assert.Equal(8, savedInvoiceItem.Quantity);
            Assert.Equal(60m, savedInvoiceItem.Rate);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutInvoiceItem_WithValidInvoiceItem_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var invoiceItem = new InvoiceItem { Id = 1, InvoiceId = 1, TaskId = 1, Quantity = 10, Rate = 50m, VATRate = 0.20m, Price = 600m };
            context.InvoiceItems!.Add(invoiceItem);
            await context.SaveChangesAsync();
            
            context.Entry(invoiceItem).State = EntityState.Detached;
            
            var updatedInvoiceItem = new InvoiceItem { Id = 1, InvoiceId = 1, TaskId = 1, Quantity = 15, Rate = 55m, VATRate = 0.20m, Price = 990m };
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.PutInvoiceItem(updatedInvoiceItem);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var savedInvoiceItem = await context.InvoiceItems.FindAsync(1);
            Assert.Equal(15, savedInvoiceItem!.Quantity);
            Assert.Equal(55m, savedInvoiceItem.Rate);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutInvoiceItem_WhenInvoiceItemNotExists_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var invoiceItem = new InvoiceItem { Id = 999, InvoiceId = 1, TaskId = 1, Quantity = 10, Rate = 50m, VATRate = 0.20m, Price = 600m };
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.PutInvoiceItem(invoiceItem);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteInvoiceItem_WithValidId_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var invoiceItem = new InvoiceItem { Id = 1, InvoiceId = 1, TaskId = 1, Quantity = 10, Rate = 50m, VATRate = 0.20m, Price = 600m };
            context.InvoiceItems!.Add(invoiceItem);
            await context.SaveChangesAsync();
            
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.DeleteInvoiceItem(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var deletedInvoiceItem = await context.InvoiceItems.FindAsync(1);
            Assert.Null(deletedInvoiceItem);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteInvoiceItem_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.DeleteInvoiceItem(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostInvoiceItem_WithDates_SavesDatesCorrectly()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var startDate = new DateOnly(2025, 1, 1);
            var endDate = new DateOnly(2025, 1, 31);
            var invoiceItem = new InvoiceItem 
            { 
                InvoiceId = 1, 
                TaskId = 1, 
                Quantity = 8, 
                Rate = 60m, 
                VATRate = 0.20m, 
                Price = 576m,
                StartDate = startDate,
                EndDate = endDate
            };
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.PostInvoiceItem(invoiceItem);

            // Assert
            var savedInvoiceItem = await context.InvoiceItems!.FindAsync(invoiceItem.Id);
            Assert.NotNull(savedInvoiceItem);
            Assert.Equal(startDate, savedInvoiceItem.StartDate);
            Assert.Equal(endDate, savedInvoiceItem.EndDate);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutInvoiceItem_UpdatesDates()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var invoiceItem = new InvoiceItem { Id = 1, InvoiceId = 1, TaskId = 1, Quantity = 10, Rate = 50m, VATRate = 0.20m, Price = 600m };
            context.InvoiceItems!.Add(invoiceItem);
            await context.SaveChangesAsync();
            
            context.Entry(invoiceItem).State = EntityState.Detached;
            
            var startDate = new DateOnly(2025, 2, 1);
            var endDate = new DateOnly(2025, 2, 28);
            var updatedInvoiceItem = new InvoiceItem 
            { 
                Id = 1, 
                InvoiceId = 1, 
                TaskId = 1, 
                Quantity = 15, 
                Rate = 55m, 
                VATRate = 0.20m, 
                Price = 990m,
                StartDate = startDate,
                EndDate = endDate
            };
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.PutInvoiceItem(updatedInvoiceItem);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var savedInvoiceItem = await context.InvoiceItems.FindAsync(1);
            Assert.Equal(startDate, savedInvoiceItem!.StartDate);
            Assert.Equal(endDate, savedInvoiceItem.EndDate);
        }

        [Fact]
        public async System.Threading.Tasks.Task DuplicateInvoiceItem_CopiesDates()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var startDate = new DateOnly(2025, 3, 1);
            var endDate = new DateOnly(2025, 3, 31);
            var invoiceItem = new InvoiceItem 
            { 
                Id = 1, 
                InvoiceId = 1, 
                TaskId = 1, 
                Quantity = 10, 
                Rate = 50m, 
                VATRate = 0.20m, 
                Price = 600m,
                StartDate = startDate,
                EndDate = endDate
            };
            context.InvoiceItems!.Add(invoiceItem);
            await context.SaveChangesAsync();
            
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.DuplicateInvoiceItem(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<InvoiceItem>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var duplicatedItem = Assert.IsType<InvoiceItem>(createdAtActionResult.Value);
            
            Assert.NotEqual(1, duplicatedItem.Id);
            Assert.Equal(startDate, duplicatedItem.StartDate);
            Assert.Equal(endDate, duplicatedItem.EndDate);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostInvoiceItem_WithDescriptionAndPriceIncVAT_SavesCorrectly()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var invoiceItem = new InvoiceItem 
            { 
                InvoiceId = 1, 
                TaskId = 1, 
                Quantity = 8m, 
                Rate = 60m, 
                VATRate = 0.20m, 
                Price = 480m,
                PriceIncVAT = 576m,
                Description = "Test description",
                Unit = Unit.Hours
            };
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.PostInvoiceItem(invoiceItem);

            // Assert
            var savedInvoiceItem = await context.InvoiceItems!.FindAsync(invoiceItem.Id);
            Assert.NotNull(savedInvoiceItem);
            Assert.Equal("Test description", savedInvoiceItem.Description);
            Assert.Equal(576m, savedInvoiceItem.PriceIncVAT);
            Assert.Equal(Unit.Hours, savedInvoiceItem.Unit);
        }

        [Fact]
        public async System.Threading.Tasks.Task DuplicateInvoiceItem_CopiesDescriptionAndPriceIncVAT()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var invoiceItem = new InvoiceItem 
            { 
                Id = 1, 
                InvoiceId = 1, 
                TaskId = 1, 
                Quantity = 10m, 
                Rate = 50m, 
                VATRate = 0.20m, 
                Price = 500m,
                PriceIncVAT = 600m,
                Description = "Original description",
                Unit = Unit.Miles
            };
            context.InvoiceItems!.Add(invoiceItem);
            await context.SaveChangesAsync();
            
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.DuplicateInvoiceItem(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<InvoiceItem>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var duplicatedItem = Assert.IsType<InvoiceItem>(createdAtActionResult.Value);
            
            Assert.Equal("Original description", duplicatedItem.Description);
            Assert.Equal(600m, duplicatedItem.PriceIncVAT);
            Assert.Equal(Unit.Miles, duplicatedItem.Unit);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateFixedCostInvoiceItem_WithValidData_CreatesCorrectly()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var fixedCost = new FixedCost { Id = 1, ContractId = 1, Name = "Setup Fee", Amount = 500m };
            context.FixedCosts!.Add(fixedCost);
            await context.SaveChangesAsync();
            
            var dto = new CreateFixedCostInvoiceItemDto
            {
                InvoiceId = 1,
                FixedCostId = 1,
                VATRate = 0.20m
            };
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.CreateFixedCostInvoiceItem(dto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<InvoiceItem>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var invoiceItem = Assert.IsType<InvoiceItem>(createdAtActionResult.Value);
            
            Assert.Equal(1, invoiceItem.InvoiceId);
            Assert.Equal(1, invoiceItem.FixedCostId);
            Assert.Equal(1m, invoiceItem.Quantity);
            Assert.Equal(500m, invoiceItem.Rate);
            Assert.Equal(500m, invoiceItem.Price);
            Assert.Equal(600m, invoiceItem.PriceIncVAT); // 500 * 1.20
            Assert.Equal("Setup Fee", invoiceItem.Description);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateMileageCostInvoiceItem_WithValidData_CreatesCorrectly()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var dto = new CreateMileageCostInvoiceItemDto
            {
                InvoiceId = 1,
                Quantity = 100.5m,
                Rate = 0.45m,
                VATRate = 0m,
                Description = "Travel to client site"
            };
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.CreateMileageCostInvoiceItem(dto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<InvoiceItem>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var invoiceItem = Assert.IsType<InvoiceItem>(createdAtActionResult.Value);
            
            Assert.Equal(1, invoiceItem.InvoiceId);
            Assert.Equal(100.5m, invoiceItem.Quantity);
            Assert.Equal(0.45m, invoiceItem.Rate);
            Assert.Equal(Unit.Miles, invoiceItem.Unit);
            Assert.Equal(45.225m, invoiceItem.Price);
            Assert.Equal(45.225m, invoiceItem.PriceIncVAT); // No VAT
            Assert.Equal("Travel to client site", invoiceItem.Description);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateLabourCostInvoiceItem_WithManualQuantityAndRate_CreatesCorrectly()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var task = new comaiz.data.Models.Task { Id = 1, Name = "Development", ContractId = 1 };
            context.Tasks!.Add(task);
            await context.SaveChangesAsync();
            
            var dto = new CreateLabourCostInvoiceItemDto
            {
                InvoiceId = 1,
                TaskId = 1,
                Quantity = 40m,
                Rate = 75m,
                VATRate = 0.20m,
                Description = "Custom description"
            };
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.CreateLabourCostInvoiceItem(dto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<InvoiceItem>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var invoiceItem = Assert.IsType<InvoiceItem>(createdAtActionResult.Value);
            
            Assert.Equal(1, invoiceItem.InvoiceId);
            Assert.Equal(1, invoiceItem.TaskId);
            Assert.Equal(40m, invoiceItem.Quantity);
            Assert.Equal(75m, invoiceItem.Rate);
            Assert.Equal(Unit.Hours, invoiceItem.Unit);
            Assert.Equal(3000m, invoiceItem.Price);
            Assert.Equal(3600m, invoiceItem.PriceIncVAT); // 3000 * 1.20
            Assert.Equal("Custom description", invoiceItem.Description);
        }
    }
}
