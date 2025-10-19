using Xunit;
using Microsoft.EntityFrameworkCore;
using comaiz.api.Controllers;
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
        public async Task GetInvoiceItems_ReturnsAllInvoiceItems()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.InvoiceItems!.Add(new InvoiceItem { Id = 1, InvoiceId = 1, CostId = 1, Quantity = 10, Rate = 50m, VATRate = 0.20m, Price = 600m });
            context.InvoiceItems.Add(new InvoiceItem { Id = 2, InvoiceId = 1, CostId = 2, Quantity = 5, Rate = 75m, VATRate = 0.20m, Price = 450m });
            await context.SaveChangesAsync();
            
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.GetInvoiceItems();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<InvoiceItem>>>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<InvoiceItem>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetInvoiceItem_WithValidId_ReturnsInvoiceItem()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.InvoiceItems!.Add(new InvoiceItem { Id = 1, InvoiceId = 1, CostId = 1, Quantity = 10, Rate = 50m, VATRate = 0.20m, Price = 600m });
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
        public async Task GetInvoiceItem_WithInvalidId_ReturnsNotFound()
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
        public async Task PostInvoiceItem_WithValidInvoiceItem_ReturnsCreatedAtAction()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var invoiceItem = new InvoiceItem { InvoiceId = 1, CostId = 1, Quantity = 8, Rate = 60m, VATRate = 0.20m, Price = 576m };
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
        public async Task PutInvoiceItem_WithValidInvoiceItem_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var invoiceItem = new InvoiceItem { Id = 1, InvoiceId = 1, CostId = 1, Quantity = 10, Rate = 50m, VATRate = 0.20m, Price = 600m };
            context.InvoiceItems!.Add(invoiceItem);
            await context.SaveChangesAsync();
            
            context.Entry(invoiceItem).State = EntityState.Detached;
            
            var updatedInvoiceItem = new InvoiceItem { Id = 1, InvoiceId = 1, CostId = 1, Quantity = 15, Rate = 55m, VATRate = 0.20m, Price = 990m };
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
        public async Task PutInvoiceItem_WhenInvoiceItemNotExists_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var invoiceItem = new InvoiceItem { Id = 999, InvoiceId = 1, CostId = 1, Quantity = 10, Rate = 50m, VATRate = 0.20m, Price = 600m };
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.PutInvoiceItem(invoiceItem);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteInvoiceItem_WithValidId_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var invoiceItem = new InvoiceItem { Id = 1, InvoiceId = 1, CostId = 1, Quantity = 10, Rate = 50m, VATRate = 0.20m, Price = 600m };
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
        public async Task DeleteInvoiceItem_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new InvoiceItemsController(context);

            // Act
            var result = await controller.DeleteInvoiceItem(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
