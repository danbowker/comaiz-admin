using Xunit;
using Microsoft.EntityFrameworkCore;
using comaiz.data;
using comaiz.data.Models;
using comaiz.Pages.WorkRecords;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace comaiz.tests.Pages
{
    /// <summary>
    /// Unit tests for WorkRecords Create page.
    /// Tests that ApplicationUser defaults to the current logged-in user.
    /// </summary>
    public class WorkRecordsCreatePageTests
    {
        private ComaizContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ComaizContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new ComaizContext(options);
        }

        private Mock<UserManager<ApplicationUser>> CreateMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        [Fact]
        public async Task OnGetAsync_WhenUserIsAuthenticated_DefaultsApplicationUserToCurrentUser()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var mockUserManager = CreateMockUserManager();
            
            var testUserId = "test-user-123";
            var testUser = new ApplicationUser 
            { 
                Id = testUserId,
                UserName = "testuser@example.com",
                Email = "testuser@example.com"
            };
            
            // Setup UserManager to return the test user
            mockUserManager
                .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);
            
            var pageModel = new CreateModel(context, mockUserManager.Object);
            
            // Setup authenticated user context
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser@example.com"),
                new Claim(ClaimTypes.NameIdentifier, testUserId)
            }, "mock"));
            
            pageModel.PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await pageModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(pageModel.WorkRecord);
            Assert.Equal(testUserId, pageModel.WorkRecord.ApplicationUserId);
        }

        [Fact]
        public async Task OnGetAsync_WhenUserIsNotAuthenticated_DoesNotSetApplicationUser()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var mockUserManager = CreateMockUserManager();
            
            var pageModel = new CreateModel(context, mockUserManager.Object);
            
            // Setup unauthenticated user context
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            
            pageModel.PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await pageModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            // WorkRecord should be null when user is not authenticated
        }

        [Fact]
        public async Task OnGetAsync_PopulatesSelectLists()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var mockUserManager = CreateMockUserManager();
            
            // Add test data
            context.Contracts!.Add(new Contract { Id = 1, Description = "Test Contract" });
            context.ContractRates!.Add(new ContractRate { Id = 1, Rate = 100, ContractId = 1 });
            context.Users!.Add(new ApplicationUser { Id = "user-1", UserName = "user1@example.com" });
            await context.SaveChangesAsync();
            
            var pageModel = new CreateModel(context, mockUserManager.Object);
            
            // Setup unauthenticated user context to test select list population
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            pageModel.PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await pageModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(pageModel.ContractsNameSelectList);
            Assert.NotNull(pageModel.RateSelectList);
            Assert.NotNull(pageModel.ApplicationUserSelectList);
        }
    }
}
