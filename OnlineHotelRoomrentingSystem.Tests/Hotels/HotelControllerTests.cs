namespace OnlineHotelRoomrentingSystem.Tests.Hotels;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Hotel;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Review;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Room;
using OnlineHotelRoomrentingSystem.Controllers;
using OnlineHotelRoomrentingSystem.Extensions.Contacts;
using System.Security.Claims;
using System.Text;
using static OnlineHotelRoomrentingSystem.Commons.MessagesConstants;
using static OnlineHotelRoomrentingSystem.Commons.NotificationMessagesConstants;

public class HotelControllerTests
{
    private Mock<IHotelService> mockHotelService;
    private Mock<IAgentService> mockAgentService;
    private Mock<IWebHostEnvironment> mockWebHostEncironment;
    private Mock<ICategoryService> mockCategoryService;
    private Mock<IReviewService> mockReviewService;
    private  Mock<IRoomService> mockRoomService;
    private Mock<IFileService> mockFileService;
    private HotelController controller;
    private ClaimsPrincipal user;

    private const string userId = "testUserId";

    [SetUp]
    public void SetUp()
    {
        mockHotelService = new Mock<IHotelService>();
        mockAgentService = new Mock<IAgentService>();
        mockWebHostEncironment = new Mock<IWebHostEnvironment>();
        mockCategoryService = new Mock<ICategoryService>();
        mockReviewService = new Mock<IReviewService>();
        mockRoomService = new Mock<IRoomService>();
        mockFileService = new Mock<IFileService>();

      user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
      {
            new Claim(ClaimTypes.NameIdentifier,"testUserId"),
      }, "mock"));

        controller = new HotelController(mockHotelService.Object, mockAgentService.Object,
                                 mockWebHostEncironment.Object, mockCategoryService.Object,
                                 mockReviewService.Object, mockRoomService.Object, mockFileService.Object);

        controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };

        controller.TempData = new TempDataDictionary(
         controller.HttpContext,
         Mock.Of<ITempDataProvider>()
         );
    }

    [Test]
    public async Task All_ShouldReturnViewResult_WithCorrectModel()
    {
        var queryModel = new AllHotelsQueryModel();
        var expectedServiceModel = new HotelQueryServiceModel
        {
            Hotels = new List<HotelServiceModel>
        {
            new HotelServiceModel
            {
                Id = "1",
                Name = "Test Hotel",
                Address = "Test Address",
                City = "Test City",
                Country = "Test Country",
                imageFile = "/images/test.jpg"
            }
        },
            TotalHotelsCount = 1
        };

        var expectedCategories = new List<string> { "Luxury", "Economy" };

        mockHotelService.Setup(service => service.AllHotelsAsync(queryModel))
                        .ReturnsAsync(expectedServiceModel);
        mockCategoryService.Setup(service => service.AllCategoriesNamesAsync())
                           .ReturnsAsync(expectedCategories);

        var result = await controller.All(queryModel);

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);

        var model = viewResult.Model as AllHotelsQueryModel;
        Assert.IsNotNull(model);
        Assert.That(model.Hotels.Count(), Is.EqualTo(expectedServiceModel.Hotels.Count()));
        Assert.That(model.TotalHotels, Is.EqualTo(expectedServiceModel.TotalHotelsCount));
        Assert.That(model.Categories.Count(), Is.EqualTo(expectedCategories.Count));
    }

    [Test]
    public async Task Mine_UserIsAgent_ShouldReturnViewWithHotels()
    {
        user.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "Agent") }));

        var expectedHotels = new List<HotelServiceModel>
        {
           new HotelServiceModel
           {
                Id = "1",
                Name = "Test Hotel",
                Address = "Test Address",
                City = "Test City",
                Country = "Test Country",
                imageFile = "/images/test.jpg"
           }
        };

        mockAgentService.Setup(service => service.ExistsByIdAsync(It.IsAny<string>())).ReturnsAsync(true);
        mockAgentService.Setup(service => service.GetAgentIdAsync(It.IsAny<string>())).ReturnsAsync("testUserId");
        mockHotelService.Setup(service => service.AllHotelsByAgentIdAsync("testUserId")).ReturnsAsync(expectedHotels);

        var result = await controller.Mine();

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as IEnumerable<HotelServiceModel>;
        Assert.IsNotNull(model);
        Assert.That(model.Count(), Is.EqualTo(expectedHotels.Count));
    }

    [Test]
    public async Task Mine_UserIsNotAgent_ShouldReturnViewWithNoHotels()
    {
        mockAgentService.Setup(service => service.ExistsByIdAsync(It.IsAny<string>())).ReturnsAsync(false);

        var result = await controller.Mine();

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);

        var model = viewResult.Model as IEnumerable<HotelServiceModel>;
        Assert.IsNull(model);
    }

    [Test]
    public async Task Mine_ShouldRedirectToAdminArea_WhenUserIsAdmin()
    {
        user.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "Administrator") }));

        mockAgentService.Setup(service => service.ExistsByIdAsync(It.IsAny<string>())).ReturnsAsync(true);

        var result = await controller.Mine();

        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.That(redirectResult.ActionName, Is.EqualTo("Mine"));
        Assert.That(redirectResult.ControllerName, Is.EqualTo("Hotel"));
        Assert.That(redirectResult.RouteValues["area"], Is.EqualTo("Admin"));
    }

    [Test]
    public async Task AddReview_ValidModel_ReturnsDetailsViewWithCorrectModel()
    {
        var hotelId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();

        var model = new HotelReviewViewModel
        {
            Rating = 3,
            Content = "Test content for review",
            ReviewDate = DateTime.Now,
            HotelId = hotelId,
        };

        controller.ModelState.Clear();

        var expectedHotelModel = new HotelDetailsViewModel
        {
            Id = hotelId,
            Name = "Test Hotel",
            Description = "Test description",
        };

        mockHotelService.Setup(x => x.HotelDetailsByIdAsync(hotelId))
                    .ReturnsAsync(expectedHotelModel);
        mockReviewService.Setup(service => service.CreateHotelReviewAsync(model, userId)).ReturnsAsync(expectedHotelModel.Id);

        var result = await controller.AddReview(model);

        Assert.IsInstanceOf<ViewResult>(result); 

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);

        Assert.That(viewResult.ViewName, Is.EqualTo("Details"));

        var returnedHotelModel = viewResult.Model as HotelDetailsViewModel;
        Assert.IsNotNull(returnedHotelModel);

        Assert.That(returnedHotelModel.Id, Is.EqualTo(expectedHotelModel.Id));
        Assert.That(returnedHotelModel.Name, Is.EqualTo(expectedHotelModel.Name));
        Assert.That(returnedHotelModel.Description, Is.EqualTo(expectedHotelModel.Description));
    }


    [Test]
    public async Task AddReview_ValidModel_SetsSuccessMessageInTempData()
    {
        string hotelId = Guid.NewGuid().ToString();

        var model = new HotelReviewViewModel
        {
            Rating = 3,
            Content = "Test content for review",
            ReviewDate = DateTime.Now,
            HotelId = hotelId,
        };

        controller.ModelState.Clear();


        var result = await controller.AddReview(model);

        Assert.IsInstanceOf<ViewResult>(result);

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);

        var tempData = controller.TempData[SuccessMessage] as string;
        Assert.IsNotNull(tempData);
        Assert.That(tempData, Is.EqualTo(SuccessfullyAddedReview));
    }

    [Test]
    public async Task Details_HotelExists_ReturnsViewWithHotelModel()
    {
        string hotelId = Guid.NewGuid().ToString();

        var hotelModel = new HotelDetailsViewModel
        {
            Id = hotelId,
            Address = "Test address",
            City = "Test city",
            Country = "Test country",
            Rooms = new List<RoomServiceModel>
          {
            new RoomServiceModel{Id = "1",PricePerNight = 100.00M},
            new RoomServiceModel{Id = "2",PricePerNight = 150.00M}
          }
        };

        mockHotelService.Setup(service => service.HotelExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        mockHotelService.Setup(service => service.HotelDetailsByIdAsync(It.IsAny<string>())).ReturnsAsync(hotelModel);
        mockRoomService.Setup(service => service.GetRoomsByHotelIdAsync(hotelId)).ReturnsAsync(hotelModel.Rooms);

        var result = await controller.Details(hotelId);

        Assert.IsInstanceOf<ViewResult>(result);
        var viewResult = (ViewResult)result;

        Assert.IsAssignableFrom<HotelDetailsViewModel>(viewResult.Model);

        var resultModel = (HotelDetailsViewModel)viewResult.Model;

        Assert.That(resultModel.Id, Is.EqualTo(hotelId));
        Assert.That(resultModel.Address, Is.EqualTo("Test address"));
        Assert.That(resultModel.City, Is.EqualTo("Test city"));
        Assert.That(resultModel.Country, Is.EqualTo("Test country"));
        Assert.That(resultModel.Rooms.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task Details_HotelDoesNotExist_RedirectsToAllHotels()
    {
        string hotelId = Guid.NewGuid().ToString();

        mockHotelService.Setup(service => service.HotelExistsAsync(hotelId)).ReturnsAsync(false);

        var result = await controller.Details(hotelId);

        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirectResult = (RedirectToActionResult)result;

        Assert.That(redirectResult.ActionName, Is.EqualTo("All"));
        Assert.That(redirectResult.ControllerName, Is.EqualTo("Hotel"));
        Assert.That(HotelIdDoesNotExist, Is.EqualTo(controller.TempData[ErrorMessage]));
    }

    [Test]
    public async Task Details_HotelModelIsNull_RedirectsToAllHotels()
    {
        string hotelId = Guid.NewGuid().ToString();

        mockHotelService.Setup(service => service.HotelExistsAsync(hotelId)).ReturnsAsync(true);
        mockHotelService.Setup(service => service.HotelDetailsByIdAsync(hotelId)).ReturnsAsync((HotelDetailsViewModel)null);

        var result = await controller.Details(hotelId);

        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirectResult = (RedirectToActionResult)result;

        Assert.That(redirectResult.ActionName, Is.EqualTo("All"));
        Assert.That(redirectResult.ControllerName, Is.EqualTo("Hotel"));
        Assert.That(HotelFormModelIsNotFound, Is.EqualTo(controller.TempData[ErrorMessage]));
    }

    [Test]
    public async Task Add_WhenAgentDoesNotExist_RedirectsToBecomeAgent()
    {
        mockAgentService.Setup(service => service.ExistsByIdAsync(userId)).ReturnsAsync(false);

        var result = await controller.Add();

        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirectToActionResult = (RedirectToActionResult)result;
        Assert.That(redirectToActionResult.ActionName, Is.EqualTo("Become"));
        Assert.That(redirectToActionResult.ControllerName, Is.EqualTo("Agent"));
    }

    [Test]
    public async Task Add_WhenImageFileIsNull_AddsModelError()
    {
        mockAgentService.Setup(service => service.ExistsByIdAsync(userId)).ReturnsAsync(true);
        var model = new HotelFormModel();
        var imageFile = (IFormFile)null;

        var result = await controller.Add(model, imageFile);

        Assert.IsInstanceOf<ViewResult>(result);
        var viewResult = (ViewResult)result;
        Assert.IsFalse(viewResult.ViewData.ModelState.IsValid);
        Assert.IsTrue(viewResult.ViewData.ModelState.ContainsKey(nameof(model.imageFile)));
        Assert.That(viewResult.ViewData.ModelState[nameof(model.imageFile)].Errors[0].ErrorMessage, Is.EqualTo(NoPhotoSelected));
    }

    [Test]
    public async Task Add_WhenCategoryDoesNotExist_AddsModelError()
    {
        mockAgentService.Setup(service => service.ExistsByIdAsync(userId)).ReturnsAsync(true);
        var model = new HotelFormModel { CategoryId = 1 };
        mockHotelService.Setup(service => service.CategoryExistAsync(model.CategoryId)).ReturnsAsync(false);

        var result = await controller.Add(model, null);

        Assert.IsInstanceOf<ViewResult>(result);
        var viewResult = (ViewResult)result;
        Assert.IsFalse(viewResult.ViewData.ModelState.IsValid);
        Assert.IsTrue(viewResult.ViewData.ModelState.ContainsKey(nameof(model.CategoryId)));
        Assert.That(viewResult.ViewData.ModelState[nameof(model.CategoryId)].Errors[0].ErrorMessage, Is.EqualTo(CategoryDoesNotExists));
    }

    [Test]
    public async Task Add_WhenDuplicateEmail_AddsModelError()
    {
        mockAgentService.Setup(service => service.ExistsByIdAsync(userId)).ReturnsAsync(true);
        var model = new HotelFormModel { Email = "duplicateEmail@com" };
        mockHotelService.Setup(service => service.HotelWithTheSameEmailExistAsync(model.Email)).ReturnsAsync(true);

        var result = await controller.Add(model, null);

        Assert.IsInstanceOf<ViewResult>(result);
        var viewResult = (ViewResult)result;
        Assert.IsFalse(viewResult.ViewData.ModelState.IsValid);
        Assert.IsTrue(viewResult.ViewData.ModelState.ContainsKey(nameof(model.Email)));
        Assert.That(viewResult.ViewData.ModelState[nameof(model.Email)].Errors[0].ErrorMessage, Is.EqualTo(SameHotelEmail));
    }

    [Test]
    public async Task Add_WhenDuplicatePhoneNumber_AddsModelError()
    {
        mockAgentService.Setup(service => service.ExistsByIdAsync(userId)).ReturnsAsync(true);
        var model = new HotelFormModel { PhoneNumber = "0856655665" };
        mockHotelService.Setup(service => service.HotelWithTheSamePhoneNumberExist(model.PhoneNumber)).ReturnsAsync(true);

        var result = await controller.Add(model, null);

        Assert.IsInstanceOf<ViewResult>(result);
        var viewResult = (ViewResult)result;
        Assert.IsFalse(viewResult.ViewData.ModelState.IsValid);
        Assert.IsTrue(viewResult.ViewData.ModelState.ContainsKey(nameof(model.PhoneNumber)));
        Assert.That(viewResult.ViewData.ModelState[nameof(model.PhoneNumber)].Errors[0].ErrorMessage, Is.EqualTo(SameHotelPhoneNumbers));
    }

    [Test]
    public async Task AddHotel_WhenHotelIsSuccessfullyCreated_ShouldReturnSuccessMessage()
    {
        var mockImageFile = CreateMockFormFile();
        var expectedHotelId = Guid.NewGuid().ToString();

        mockAgentService.Setup(service => service.ExistsByIdAsync(userId)).ReturnsAsync(true);
        mockHotelService.Setup(service => service.CategoryExistAsync(It.IsAny<int>())).ReturnsAsync(true);
        mockHotelService.Setup(service => service.HotelWithTheSameEmailExistAsync(It.IsAny<string>())).ReturnsAsync(false);
        mockHotelService.Setup(service => service.HotelWithTheSamePhoneNumberExist(It.IsAny<string>())).ReturnsAsync(false);
        mockHotelService.Setup(service => service.CreateHotelAsync(It.IsAny<HotelFormModel>(), It.IsAny<string>(),
                                         It.IsAny<IFormFile>())).ReturnsAsync(expectedHotelId);
        mockFileService.Setup(service => service.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<IWebHostEnvironment>(), "images"))
                                       .ReturnsAsync("path/to/uploaded/file");

        var model = new HotelFormModel { PhoneNumber = "085555555", Email = "testEmial.com" };

        var result = await controller.Add(model, mockImageFile);

        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var resultAsRedirect = (RedirectToActionResult)result;
        Assert.IsNotNull(resultAsRedirect);

        Assert.That(resultAsRedirect.ActionName, Is.EqualTo("Details"));
        Assert.IsTrue(resultAsRedirect.RouteValues.ContainsKey("id"));
        Assert.That(resultAsRedirect.RouteValues["id"], Is.EqualTo(expectedHotelId));

        Assert.That(SuccessfullyCreatedHotel, Is.EqualTo(controller.TempData[SuccessMessage]));
    }

    [Test]
    public async Task EditHotel_HotelDoestNotExistShouldRedirectToAllHotel()
    {
        string nonExistingHotelId = "nonExistingId";

        mockHotelService.Setup(service => service.HotelExistsAsync(nonExistingHotelId)).ReturnsAsync(false);

        var result = await controller.Edit(nonExistingHotelId);

        Assert.IsInstanceOf<RedirectToActionResult>(result);

        var redirectToActionResult = (RedirectToActionResult)result;
        Assert.That(redirectToActionResult.ActionName, Is.EqualTo("All"));
        Assert.That(redirectToActionResult.ControllerName, Is.EqualTo("Hotel"));
        Assert.That(controller.TempData[ErrorMessage], Is.EqualTo(HotelDoesNotExists));
    }

    [Test]
    public async Task Edit_WhenUserIsNotAuthorized_ReturnsUnauthorized()
    {
        string existingHotelId = "existingId";
        string agentId = "agentId";

        mockHotelService.Setup(service => service.HotelExistsAsync(existingHotelId)).ReturnsAsync(true);
        mockAgentService.Setup(service => service.GetAgentIdAsync(It.IsAny<string>())).ReturnsAsync(agentId);
        mockHotelService.Setup(service => service.HasHotelManagerWithIdAsync(existingHotelId, agentId)).ReturnsAsync(false);

        var result = await controller.Edit(existingHotelId);

        Assert.IsInstanceOf<UnauthorizedResult>(result);

        Assert.That(controller.TempData[ErrorMessage], Is.EqualTo(NotAuthorizedHotelManager));
    }

    [Test]
    public async Task Edit_WhenHotelExistsAndUserIsAuthorized_ReturnsViewWithHotel()
    {
        string existingHotelId = "existingId";
        string agentId = "agentId";

        var hotelEditModel = new HotelFormModel
        {
            Name = "Test Hotel",
            Address = "123 Test Street",
            City = "Test City",
            Country = "Test Country",
            Email = "test@example.com",
            PhoneNumber = "1234567890",
            Description = "A hotel for testing purposes.",
            NumberOfRooms = 10,
            MaximumCapacity = 20,
            CategoryId = 1
        };

        mockHotelService.Setup(service => service.HotelExistsAsync(existingHotelId)).ReturnsAsync(true);
        mockAgentService.Setup(service => service.GetAgentIdAsync(It.IsAny<string>())).ReturnsAsync(agentId);
        mockHotelService.Setup(service => service.HasHotelManagerWithIdAsync(existingHotelId, agentId)).ReturnsAsync(true);
        mockHotelService.Setup(service => service.EditHotelByIdAsync(existingHotelId)).ReturnsAsync(hotelEditModel);

        var result = await controller.Edit(existingHotelId);

        Assert.IsInstanceOf<ViewResult>(result);
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.That(hotelEditModel, Is.EqualTo(viewResult.Model));
    }

    [Test]
    public async Task DeleteHotel_WhenAgentIdDoesNotExists()
    {
        string nonExistingAgentId = null;
        string hotelId = "hotelId";

        mockAgentService.Setup(service => service.GetAgentIdAsync(It.IsAny<string>())).ReturnsAsync(nonExistingAgentId);
        mockHotelService.Setup(service => service.HotelExistsAsync(hotelId)).ReturnsAsync(true);

        var result = await controller.Delete(hotelId);

        Assert.IsInstanceOf<RedirectToActionResult>(result);

        var resultAsRedirectToAction = result as RedirectToActionResult;

        Assert.That(resultAsRedirectToAction.ActionName, Is.EqualTo("Become"));
        Assert.That(resultAsRedirectToAction.ControllerName, Is.EqualTo("Agent"));
        Assert.That(controller.TempData[ErrorMessage], Is.EqualTo(AgenIdDoesNotExists));
    }

    [Test]
    public async Task DeleteHotel_ShouldSuccessfullyReturnTheHotelForDeletion()
    {
        string existingHotelId = "existingId";
        string agentId = "agentId";

        var hotelToDelete = new HotelPreDeleteDetailsViewModel
        {
            Address = "TestAdress123",
            Name = "Test Name",
            City = "Test city",
            Image = "test image"
        };

        mockAgentService.Setup(service => service.GetAgentIdAsync(It.IsAny<string>())).ReturnsAsync(agentId);
        mockHotelService.Setup(service => service.HotelExistsAsync(existingHotelId)).ReturnsAsync(true);
        mockHotelService.Setup(service => service.HasHotelManagerWithIdAsync(existingHotelId,agentId)).ReturnsAsync(true);
        mockHotelService.Setup(service => service.GetHotelForDeleteByIdAsync(existingHotelId)).ReturnsAsync(hotelToDelete);

        var result = await controller.Delete(existingHotelId);

        Assert.IsInstanceOf<ViewResult>(result);

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);

        var model = viewResult.Model as HotelPreDeleteDetailsViewModel;
        Assert.IsNotNull(model);

        Assert.That(hotelToDelete.Address, Is.EqualTo(model.Address));
        Assert.That(hotelToDelete.Name, Is.EqualTo(model.Name));
        Assert.That(hotelToDelete.City, Is.EqualTo(model.City));
        Assert.That(hotelToDelete.Image, Is.EqualTo(model.Image));

    }

    [Test]
    public async Task DeleteHotel_ShouldSuccessfullyDeleteHotel()
    {
        string existingHotelId = "existingHotelId";
        string agentId = "agentId";

        mockAgentService.Setup(service => service.GetAgentIdAsync(It.IsAny<string>())).ReturnsAsync(agentId);
        mockHotelService.Setup(service => service.HotelExistsAsync(existingHotelId)).ReturnsAsync(true);
        mockHotelService.Setup(service => service.HasHotelManagerWithIdAsync(existingHotelId, agentId)).ReturnsAsync(true);

        var result = await controller.Delete(new HotelDetailsViewModel(), existingHotelId);

        Assert.IsInstanceOf<RedirectToActionResult>(result);

        var resultAsRedirectToAction = result as RedirectToActionResult;

        Assert.IsNotNull(resultAsRedirectToAction);
        Assert.That(controller.TempData[WarningMessage], Is.EqualTo(SuccessfullyDeletedHotel));
        Assert.That(resultAsRedirectToAction.ActionName, Is.EqualTo("All"));
        Assert.That(resultAsRedirectToAction.ControllerName, Is.EqualTo("Hotel"));

        mockHotelService.Verify(service => service.DeleteHotelByIdAsync(existingHotelId), Times.Once);
    }

    [Test]
    public async Task DeleteHotel_ShouldDeleteHotelAndRoomsInHotel()
    {
        string existingHotelId = "existingHotelId";
        string agentId = "agentId";

        mockAgentService.Setup(service => service.GetAgentIdAsync(It.IsAny<string>())).ReturnsAsync(agentId);
        mockHotelService.Setup(service => service.HotelExistsAsync(existingHotelId)).ReturnsAsync(true);
        mockHotelService.Setup(service => service.HasHotelManagerWithIdAsync(existingHotelId, agentId)).ReturnsAsync(true);
        mockHotelService.Setup(service => service.HasRoomsAsync(existingHotelId)).ReturnsAsync(true);
        mockHotelService.Setup(service => service.DeleteRoomsByHotelIdAsync(existingHotelId)).Returns(Task.CompletedTask);
        mockHotelService.Setup(service => service.HotelHasReviewsAsync(existingHotelId)).ReturnsAsync(true);
        mockReviewService.Setup(service => service.DeleteReviewsByHotelIdAsync(existingHotelId)).Returns(Task.CompletedTask);
        mockHotelService.Setup(service => service.DeleteHotelByIdAsync(existingHotelId)).Returns(Task.CompletedTask);

        var result = await controller.Delete(new HotelDetailsViewModel(), existingHotelId);

        Assert.IsInstanceOf<RedirectToActionResult>(result);

        var resultAsRedirectToAction = result as RedirectToActionResult;

        mockHotelService.Verify(service => service.DeleteRoomsByHotelIdAsync(existingHotelId), Times.Once);
        mockHotelService.Verify(service => service.DeleteHotelByIdAsync(existingHotelId), Times.Once);

        Assert.IsNotNull(resultAsRedirectToAction);
        Assert.That(controller.TempData[WarningMessage], Is.EqualTo(SuccessfullyDeletedHotel));
        Assert.That(resultAsRedirectToAction.ActionName, Is.EqualTo("All"));
        Assert.That(resultAsRedirectToAction.ControllerName, Is.EqualTo("Hotel"));
    }

    [TearDown]
    public void Dispose()
    {
        controller.Dispose();
    }

    private IFormFile CreateMockFormFile()
    {
        var content = "Fake file content";
        var fileName = "test.jpg";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        return new FormFile(stream, 0, stream.Length, "id_from_form", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
    }
}
