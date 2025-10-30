using Xunit;
using Microsoft.EntityFrameworkCore;
using comaiz.api.Controllers;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;

namespace comaiz.tests.Controllers
{
    /// <summary>
    /// Unit tests for InvoicesController using In-Memory Database.
    /// Tests all CRUD operations with success and failure scenarios.
    /// </summary>
    public class InvoicesControllerTests
    {
        private ComaizContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ComaizContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new ComaizContext(options);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetInvoices_ReturnsAllInvoices()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.Invoices!.Add(new Invoice { Id = 1, Date = DateOnly.FromDateTime(DateTime.Now), ClientId = 1 });
            context.Invoices.Add(new Invoice { Id = 2, Date = DateOnly.FromDateTime(DateTime.Now), ClientId = 2 });
            await context.SaveChangesAsync();
            
            var controller = new InvoicesController(context);

            // Act
            var result = await controller.GetInvoices();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Invoice>>>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Invoice>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetInvoice_WithValidId_ReturnsInvoice()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var date = DateOnly.FromDateTime(DateTime.Now);
            context.Invoices!.Add(new Invoice { Id = 1, Date = date, ClientId = 1, PurchaseOrder = "PO-123" });
            await context.SaveChangesAsync();
            
            var controller = new InvoicesController(context);

            // Act
            var result = await controller.GetInvoice(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Invoice>>(result);
            var returnValue = Assert.IsType<Invoice>(actionResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("PO-123", returnValue.PurchaseOrder);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetInvoice_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new InvoicesController(context);

            // Act
            var result = await controller.GetInvoice(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostInvoice_WithValidInvoice_ReturnsCreatedAtAction()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var invoice = new Invoice { Date = DateOnly.FromDateTime(DateTime.Now), ClientId = 1, PurchaseOrder = "PO-123" };
            var controller = new InvoicesController(context);

            // Act
            var result = await controller.PostInvoice(invoice);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Invoice>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetInvoice", createdAtActionResult.ActionName);
            
            var savedInvoice = await context.Invoices!.FindAsync(invoice.Id);
            Assert.NotNull(savedInvoice);
            Assert.Equal("PO-123", savedInvoice.PurchaseOrder);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutInvoice_WithValidInvoice_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var invoice = new Invoice { Id = 1, Date = DateOnly.FromDateTime(DateTime.Now), ClientId = 1, PurchaseOrder = "PO-123" };
            context.Invoices!.Add(invoice);
            await context.SaveChangesAsync();
            
            context.Entry(invoice).State = EntityState.Detached;
            
            var updatedInvoice = new Invoice { Id = 1, Date = DateOnly.FromDateTime(DateTime.Now), ClientId = 1, PurchaseOrder = "PO-456" };
            var controller = new InvoicesController(context);

            // Act
            var result = await controller.PutInvoice(updatedInvoice);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var savedInvoice = await context.Invoices.FindAsync(1);
            Assert.Equal("PO-456", savedInvoice!.PurchaseOrder);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutInvoice_WhenInvoiceNotExists_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var invoice = new Invoice { Id = 999, Date = DateOnly.FromDateTime(DateTime.Now), ClientId = 1 };
            var controller = new InvoicesController(context);

            // Act
            var result = await controller.PutInvoice(invoice);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteInvoice_WithValidId_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var invoice = new Invoice { Id = 1, Date = DateOnly.FromDateTime(DateTime.Now), ClientId = 1 };
            context.Invoices!.Add(invoice);
            await context.SaveChangesAsync();
            
            var controller = new InvoicesController(context);

            // Act
            var result = await controller.DeleteInvoice(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var deletedInvoice = await context.Invoices.FindAsync(1);
            Assert.Null(deletedInvoice);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteInvoice_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new InvoicesController(context);

            // Act
            var result = await controller.DeleteInvoice(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
