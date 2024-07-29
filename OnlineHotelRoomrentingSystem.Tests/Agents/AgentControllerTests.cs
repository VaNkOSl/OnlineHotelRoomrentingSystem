namespace OnlineHotelRoomrentingSystem.Tests.Agents;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Agent;
using OnlineHotelRoomrentingSystem.Controllers;
using System.Security.Claims;
using static OnlineHotelRoomrentingSystem.Commons.MessagesConstants;
using static OnlineHotelRoomrentingSystem.Commons.NotificationMessagesConstants;


public class AgentControllerTests
{
    private readonly Mock<IAgentService> mockAgentService;
    private AgentController controller;

    public AgentControllerTests()
    {
        mockAgentService = new Mock<IAgentService>();
    }

    [OneTimeSetUp]
    public void Setup()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "testUserId")
        }, "mock"));

        controller = new AgentController(mockAgentService.Object);
        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };

        controller.TempData = new TempDataDictionary(
            controller.HttpContext,
            Mock.Of<ITempDataProvider>()
        );
    }

    [Test]
    public async Task Become_Get_ShouldReturnViewIfUserNotExists()
    {
        mockAgentService.Setup(s => s.ExistsByIdAsync(It.IsAny<string>())).ReturnsAsync(false);

        var result = await controller.Become();

        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        Xunit.Assert.Null(viewResult.ViewName);
    }

    [Test]
    public async Task Become_Get_ShouldReturnBadRequestIfUserExists()
    {
        var model = new BecomeAgentFormModel();
        controller.ModelState.AddModelError("FakeError", "FakeMessage");

        var result = await controller.Become(model);

        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        Xunit.Assert.Equal(model, viewResult.Model);
    }

    [Test]
    public async Task Become_Post_ShouldReturnViewWithErrorIfModelIsInvalid()
    {
        var model = new BecomeAgentFormModel();
        controller.ModelState.AddModelError("FakeError", "FakeMessage");

        var result = await controller.Become(model);

        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        Xunit.Assert.Equal(model, viewResult.Model);
    }

    [Test]
    public async Task Become_Post_ShouldAddModelErrorifEgnExists()
    {
        var model = new BecomeAgentFormModel();
        mockAgentService.Setup(s => s.UserWithEgnExistAsync(It.IsAny<string>())).ReturnsAsync(true);

        var result = await controller.Become(model);

        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        Xunit.Assert.True(controller.ModelState.ContainsKey(nameof(model.EGN)));
    }

    [Test]
    public async Task Become_Post_ShouldAddModelErrorIfPhoneNumberExists()
    {
        var model = new BecomeAgentFormModel();
        mockAgentService.Setup(s => s.UserWithPhoneNumberExistAsync(It.IsAny<string>())).ReturnsAsync(true);

        var result = await controller.Become(model);

        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        Xunit.Assert.True(controller.ModelState.ContainsKey(nameof(model.PhoneNumber)));
    }

    [Test]
    public async Task Become_Post_ShouldRedirectToHootelControllerAllActionIfUserExists()
    {
        var model = new BecomeAgentFormModel();
        var userId = "testUserId";

        mockAgentService.Setup(s => s.UserWithEgnExistAsync(model.EGN)).ReturnsAsync(false);
        mockAgentService.Setup(s => s.UserWithPhoneNumberExistAsync(model.PhoneNumber)).ReturnsAsync(false);
        mockAgentService.Setup(s => s.ExistsByIdAsync(userId)).ReturnsAsync(false);

        var result = await controller.Become(model);

        if (result is RedirectToActionResult redirectResult)
        {
            Xunit.Assert.Equal("All", redirectResult.ActionName);
            Xunit.Assert.Equal("Hotel", redirectResult.ControllerName);
            Xunit.Assert.Equal(SuccessfullyBecomeAHotelManager, controller.TempData[SuccessMessage]);
        }
        else if (result is ViewResult viewResult)
        {
            Xunit.Assert.Null(viewResult.ViewName);
            Xunit.Assert.Equal(model, viewResult.Model); 
        }
    }

    [OneTimeTearDown]
    public void Dispose()
    {
        controller.Dispose();
    }
}
