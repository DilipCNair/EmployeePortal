using AutoMapper;
using Identity.Controllers;
using Identity.Data;
using Identity.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace UnitTestProject;

[TestFixture]
public class Tests
{
    private Mock<ApplicationDBContext> mockDBContext;
    private Mock<HttpContextAccessor> mockHttpContextAccessor;
    private Mock<IWebHostEnvironment> mockWebHostEnvironment;
    private Mock<IMapper> mockMapper;
    private Mock<UserManager<Employee>> mockUserManager;
    private Mock<SignInManager<Employee>> mockSignInManager;

    private AccountController accountController;

    [SetUp]
    public void Setup()
    {
        // Arrange
        var options = new DbContextOptions<ApplicationDBContext>();
        mockDBContext = new Mock<ApplicationDBContext>(options);

        mockHttpContextAccessor = new Mock<HttpContextAccessor>();
        mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
        mockMapper = new Mock<IMapper>();

        mockUserManager = new Mock<UserManager<Employee>>(
            new Mock<IUserStore<Employee>>().Object,
            null, null, null, null, null, null, null, null
        );

        mockSignInManager = new Mock<SignInManager<Employee>>(
            mockUserManager.Object,
            mockHttpContextAccessor.Object,
            new Mock<IUserClaimsPrincipalFactory<Employee>>().Object,
            null, null, null, null
        );

        accountController = new AccountController(
            mockDBContext.Object,
            mockHttpContextAccessor.Object,
            mockWebHostEnvironment.Object,
            mockMapper.Object,
            mockUserManager.Object,
            mockSignInManager.Object
        );
    }


    [Test]
    public void ConfirmDelete_ReturnsConfirmDeleteView()
    {
        // Act
        IActionResult result = accountController.ConfirmDelete();

        
        // Assert
        Assert.That(result, Is.InstanceOf<ViewResult>(), "Expected a ViewResult");

        var viewResult = result as ViewResult;
        Assert.That(viewResult?.ViewName, Is.EqualTo("ConfirmDelete"),
                    "Expected the view name to be 'ConfirmDelete'");
    }


    [TearDown]
    public void Teardown()
    {
        // Cleanup resources
        accountController?.Dispose();
    }
}