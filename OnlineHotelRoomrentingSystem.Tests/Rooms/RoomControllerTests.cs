namespace OnlineHotelRoomrentingSystem.Tests.Rooms;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NuGet.Protocol.Core.Types;
using OnilineHotelRoomBookingSystem.Services.Data;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Review;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Room;
using OnlineHotelRoomrentingSystem.Controllers;
using OnlineHotelRoomrentingSystem.Data;
using OnlineHotelRoomrentingSystem.Data.Data.Common;
using OnlineHotelRoomrentingSystem.Extensions.Contacts;
using OnlineHotelRoomrentingSystem.Models;
using System.Security.Claims;
using System.Text;
using static OnlineHotelRoomrentingSystem.Commons.EntityValidationConstants;
using static OnlineHotelRoomrentingSystem.Commons.MessagesConstants;
using static OnlineHotelRoomrentingSystem.Commons.NotificationMessagesConstants;
using Agent = Models.Agent;

public class RoomControllerTests
{
    private Mock<IAgentService> mockAgentService;
    private Mock<IRoomService> mockRoomService;
    private Mock<IReviewService>  mockReviewService;
    private Mock<IHotelService> mockHotelService;
    private Mock<IWebHostEnvironment> mockWebHostEnvironment;
    private Mock<IMemoryCache> mockMemoryCache;
    private Mock<IFileService> mockFileHelper;

    private RoomController controller;
    private ClaimsPrincipal user;

    [SetUp]
    public void OneTimeSetUp()
    {
        mockAgentService = new Mock<IAgentService>();
        mockHotelService = new Mock<IHotelService>();
        mockRoomService = new Mock<IRoomService>();
        mockReviewService = new Mock<IReviewService>();
        mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
        mockMemoryCache = new Mock<IMemoryCache>();
        mockFileHelper = new Mock<IFileService>();

        user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier,"testUserId"),
        }, "mock" ));

        controller = new RoomController(mockRoomService.Object,mockAgentService.Object,mockHotelService.Object,
                                        mockWebHostEnvironment.Object, mockReviewService.Object, mockMemoryCache.Object,
                                        mockFileHelper.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };

        controller.TempData = new TempDataDictionary(
          controller.HttpContext,
          Mock.Of<ITempDataProvider>()
          );
    }

    [Test]
    public async Task All_ShouldReturnAllRooms()
    {
        var mockRooms = new List<RoomServiceModel>()
        {
          new RoomServiceModel{Id = "1",PricePerNight = 100.00M,IsAvaible = true},
          new RoomServiceModel{Id = "2",PricePerNight = 150.00M,IsAvaible = false},
        };

        mockRoomService.Setup(service => service.GetAllRoomsAsync()).ReturnsAsync(mockRooms);

        var result = await controller.All();

        var viewResult = result as ViewResult;

        Assert.IsNotNull(viewResult);
        Assert.IsInstanceOf<ViewResult>(viewResult);
        var model = viewResult.Model as List<RoomServiceModel>;
        Assert.IsNotNull(model);
        Assert.That(2, Is.EqualTo(model.Count));
    }

    [Test]
    public async Task All_ShouldReturnEmptyWhenThereAreNoRooms()
    {
        var mockRooms = new List<RoomServiceModel>();

        mockRoomService.Setup(service => service.GetAllRoomsAsync()).ReturnsAsync(mockRooms);

        var result = await controller.All();

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);

        Assert.IsInstanceOf<ViewResult> (viewResult);
        var model = viewResult.Model as List<RoomServiceModel>;

        Assert.IsEmpty(model);
    }


    [Test]
    public async Task Mine_UserIsAgent_ShouldReturnViewWithRooms()
    {
        user.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "Agent") }));

        var expectedRooms = new List<RoomServiceModel>()
        {
            new RoomServiceModel
            {
                Id = "1",
                HotelName = "Hotel name",
                imageFile = "img",
                IsAvaible = true,
                PricePerNight = 100.0M,
                HotelId = "1",
                RoomType = "Deluxe"
            }
        };

        mockAgentService.Setup(service => service.ExistsByIdAsync(It.IsAny<string>())).ReturnsAsync(true);
        mockAgentService.Setup(service => service.GetAgentIdAsync(It.IsAny<string>())).ReturnsAsync("testUserId");
        mockRoomService.Setup(service => service.AllRoomsByAgentIdAsync("testUserId")).ReturnsAsync(expectedRooms);

        var result = await controller.Mine();

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as IEnumerable<RoomServiceModel>;
        Assert.IsNotNull(model);
        Assert.That(model.Count(), Is.EqualTo(expectedRooms.Count));
    }

    [Test]
    public async Task Mine_UserIsNotAgent_ShouldReturnViewWithRooms()
    {
        mockAgentService.Setup(service => service.ExistsByIdAsync(It.IsAny<string>())).ReturnsAsync(false);

        var result = await controller.Mine();

        var viewResult = result as ViewResult;
        Assert.IsNotNull(result);

        var model = viewResult.Model as IEnumerable<RoomServiceModel>;
        Assert.IsEmpty(model);
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
        Assert.That(redirectResult.ControllerName, Is.EqualTo("Room"));
        Assert.That(redirectResult.RouteValues["area"], Is.EqualTo("Admin"));
    }

    [Test]
    public async Task Room_AddReview_ValidModel_SetsSuccessMessageInTempData()
    {
        string roomId = Guid.NewGuid().ToString();

        var reviewModel = new RoomReviewViewModel
        {
            Rating = 3,
            Content = "Test review content",
            ReviewDate = DateTime.Now,
            RoomId = roomId
        };

        var result = await controller.AddReview(reviewModel);

        Assert.IsInstanceOf<ViewResult>(result);

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);

        var tempData = controller.TempData[SuccessMessage] as string;
        Assert.IsNotNull(tempData);
        Assert.That(tempData, Is.EqualTo(SuccessfullyAddedReview));
    }

    [Test]
    public async Task Room_AddReview_ValidModel_ReturnsDetailsViewWithCorrectModel()
    {
        string roomId = Guid.NewGuid().ToString();
        string userId = Guid.NewGuid().ToString();

        var roomReviewModel = new RoomReviewViewModel
        {
            Rating = 3,
            Content = "Test review content",
            ReviewDate = DateTime.Now,
            RoomId = roomId
        };

        var expectedRoomModel = new RoomDetailsViewModel
        {
            Id = roomId,
            Description = "Test description",
            PricePerNight = 100.00M,
            IsAvaible = false
        };

        mockRoomService.Setup(service => service.RoomDetailsByIdAsync(roomId)).ReturnsAsync(expectedRoomModel);
        mockReviewService.Setup(service => service.CreateRoomReviewAsync(roomReviewModel, userId)).ReturnsAsync(expectedRoomModel.Id);

        var result = await controller.AddReview(roomReviewModel);

        Assert.IsInstanceOf<ViewResult>(result);
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);

        Assert.That("Details", Is.EqualTo(viewResult.ViewName));

        var returnedRoomModel = viewResult.Model as RoomDetailsViewModel;
        Assert.IsNotNull(returnedRoomModel);

        Assert.That(expectedRoomModel.Id, Is.EqualTo(returnedRoomModel.Id));
        Assert.That(expectedRoomModel.Description, Is.EqualTo(returnedRoomModel.Description));
        Assert.That(expectedRoomModel.PricePerNight, Is.EqualTo(returnedRoomModel.PricePerNight));
        Assert.That(expectedRoomModel.IsAvaible, Is.EqualTo(returnedRoomModel.IsAvaible));
    }

    [Test]
    public async Task RoomDetails_RoomIdDoesNotExists_ShouldRedirectToAllRooms()
    {
        string roomId = Guid.NewGuid().ToString();
        mockRoomService.Setup(service => service.RoomExistsAsync(roomId)).ReturnsAsync(false);

        var result = await controller.Details(roomId);

        Assert.IsInstanceOf<RedirectToActionResult>(result);

        var redirectResult = result as RedirectToActionResult;

        Assert.IsNotNull(redirectResult);
        Assert.That(redirectResult.ActionName, Is.EqualTo("All"));
        Assert.That(redirectResult.ControllerName, Is.EqualTo("Room"));
        Assert.That(RoomIdDoesNotExists, Is.EqualTo(controller.TempData[ErrorMessage]));
    }

    [Test]
    public async Task RoomDetails_WhenRoomModelIsNull_RedirectToAllRooms()
    {
        string roomId = Guid.NewGuid().ToString();
        mockRoomService.Setup(service => service.RoomExistsAsync(roomId)).ReturnsAsync(true);
        mockRoomService.Setup(service => service.RoomDetailsByIdAsync(roomId)).ReturnsAsync((RoomDetailsViewModel)null);

        var result = await controller.Details(roomId);

        Assert.IsInstanceOf<RedirectToActionResult>(result);

        var redirectResult = result as RedirectToActionResult;

        Assert.IsNotNull(redirectResult);
        Assert.IsNotNull(redirectResult);
        Assert.That(redirectResult.ActionName, Is.EqualTo("All"));
        Assert.That(redirectResult.ControllerName, Is.EqualTo("Room"));
        Assert.That(RoomFormModelIsNotFound, Is.EqualTo(controller.TempData[ErrorMessage]));
    }

    [Test]
    public async Task RoomDetails_RoomExists_ShouldRetunrViewWithRoomModel()
    {
        string roomId = Guid.NewGuid().ToString();

        var roomModel = new RoomDetailsViewModel
        {
            HotelId = Guid.NewGuid().ToString(),
            Id = roomId,
            RoomType = "Deluxe" ,
            Description = "test",
            HotelName = "Test hotel",
            IsAvaible = true,
            PricePerNight = 10,
            imageFile = "jhj",
            Agent = new OnlineHotelRoomBookingSystem.Web.ViewModels.Agent.AgentServiceModel
            {
                FullName = "Ivan Petrov Petrov",
                Email = "ivan@gamil.com",
                PhoneNumber = "0885555555",
            },
            Reviews = new List<RoomReviewViewModel>
            {
                new RoomReviewViewModel
                {
                    Content = "This is test review",
                    Rating = 5,
                    ReviewDate = DateTime.Now,
                    RoomId = roomId,
                }
            }
        };

        mockRoomService.Setup(service => service.RoomExistsAsync(roomId)).ReturnsAsync(true);
        mockRoomService.Setup(service => service.RoomDetailsByIdAsync(It.IsAny<string>())).ReturnsAsync(roomModel);

        var result = await controller.Details(roomId);

        Assert.IsInstanceOf<ViewResult>(result);
        var viewResult = result as ViewResult;

        Assert.IsAssignableFrom<RoomDetailsViewModel>(viewResult.Model);

        var resultModel = (RoomDetailsViewModel)viewResult.Model;


        Assert.That(resultModel.Id, Is.EqualTo(roomId));
        Assert.That(resultModel.RoomType, Is.EqualTo("Deluxe"));
        Assert.That(resultModel.Description, Is.EqualTo("test"));
        Assert.That(resultModel.HotelName, Is.EqualTo("Test hotel"));
        Assert.That(resultModel.IsAvaible, Is.True);
        Assert.That(resultModel.PricePerNight, Is.EqualTo(10));
        Assert.That(resultModel.imageFile, Is.EqualTo("jhj"));

        Assert.IsNotNull(resultModel.Agent);
        Assert.That(resultModel.Agent.FullName, Is.EqualTo("Ivan Petrov Petrov"));
        Assert.That(resultModel.Agent.Email, Is.EqualTo("ivan@gamil.com"));
        Assert.That(resultModel.Agent.PhoneNumber, Is.EqualTo("0885555555"));

        Assert.IsNotNull(resultModel.Reviews);
        Assert.That(resultModel.Reviews.Count, Is.EqualTo(1));
        var review = resultModel.Reviews[0];
        Assert.That(review.Content, Is.EqualTo("This is test review"));
        Assert.That(review.Rating, Is.EqualTo(5));
        Assert.That(review.RoomId, Is.EqualTo(roomId));
    }

    [Test]
    public async Task RoomAdd_WhenAgenDoesNotExisstShouldRedirectToBecomeAgengView()
    {
        mockAgentService.Setup(service => service.ExistsByIdAsync(It.IsAny<string>())).ReturnsAsync(false);

        var result = await controller.Add();

        Assert.IsInstanceOf<RedirectToActionResult>(result);

        var redirectToAdcionResult = (RedirectToActionResult)result;

        Assert.That(redirectToAdcionResult.ActionName,Is.EqualTo("Become"));
        Assert.That(redirectToAdcionResult.ControllerName, Is.EqualTo("Agent"));
        Assert.That(controller.TempData[ErrorMessage], Is.EqualTo(AgenDoesNotExists));
    }

    [Test]
    public async Task RoomAdd_ReturnsViewWithCorrectModel_WhenAgentExists()
    {
        mockAgentService.Setup(service => service.ExistsByIdAsync(It.IsAny<string>())).ReturnsAsync(true);
        mockRoomService.Setup(service => service.AllRoomTypesAsync()).ReturnsAsync(new List<RoomTypeServiceModel>());
        mockRoomService.Setup(service => service.GetAllHotelsAsync()).ReturnsAsync(new List<HotelRoomViewModel>());

        var result = await controller.Add();
        Assert.IsInstanceOf<ViewResult>(result);
        var viewResult = (ViewResult)result;

        Assert.IsInstanceOf<RoomFormModel>(viewResult.Model);
        var model = (RoomFormModel)viewResult.Model;

        Assert.IsNotNull(model.RoomTypes);
        Assert.IsNotNull(model.Hotels);

        mockAgentService.Verify(s => s.ExistsByIdAsync(It.IsAny<string>()), Times.Once);
        mockRoomService.Verify(s => s.AllRoomTypesAsync(), Times.Once);
        mockRoomService.Verify(s => s.GetAllHotelsAsync(), Times.Once);
    }

    [Test]
    public async Task AddWhen_ImageFileIsNull_AddsModelError()
    {
        mockAgentService.Setup(service => service.ExistsByIdAsync(It.IsAny<string>())).ReturnsAsync(true);
        mockRoomService.Setup(service => service.RoomTypeExistAsync(It.IsAny<int>())).ReturnsAsync(true);
        mockHotelService.Setup(service => service.HotelExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        mockAgentService.Setup(service => service.HasHotelWithIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var model = new RoomFormModel();
        var imageFile = (IFormFile)null;

        var result = await controller.Add(model, imageFile);

        Assert.IsInstanceOf<ViewResult>(result);
        var viewResult = (ViewResult)result;

        Assert.IsFalse(viewResult.ViewData.ModelState.IsValid);
        Assert.IsTrue(viewResult.ViewData.ModelState.ContainsKey(nameof(model.imageFile)));
        Assert.That(viewResult.ViewData.ModelState[nameof(model.imageFile)].Errors[0].ErrorMessage,Is.EqualTo(NoPhotoSelected));
    }

    [Test]
    public async Task AddRoom_WhenRoomTypeDoesNotExists()
    {
        mockAgentService.Setup(service => service.ExistsByIdAsync(It.IsAny<string>())).ReturnsAsync(true);
        mockRoomService.Setup(service => service.RoomTypeExistAsync(It.IsAny<int>())).ReturnsAsync(false);

        var model = new RoomFormModel();

        var result = await controller.Add(model, null);

        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirectResult = (RedirectToActionResult)result;

        Assert.IsNotNull(redirectResult);
        Assert.That(redirectResult.ActionName, Is.EqualTo("Add"));
        Assert.That(redirectResult.ControllerName, Is.EqualTo("Room"));
        Assert.That(controller.TempData[ErrorMessage], Is.EqualTo(RoomTypeDoesNotExists));
    }

    [Test]
    public async Task AddRoom_WhenRoomHotelNotExists()
    {
        mockAgentService.Setup(service => service.ExistsByIdAsync(It.IsAny<string>())).ReturnsAsync(true);
        mockRoomService.Setup(service => service.RoomTypeExistAsync(It.IsAny<int>())).ReturnsAsync(true);
        mockHotelService.Setup(service => service.HotelExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
     //   mockAgentService.Setup(service => service.HasHotelWithIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);


        var model = new RoomFormModel();

        var result = await controller.Add(model, null);

        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirectResult = (RedirectToActionResult)result;

        Assert.IsNotNull(redirectResult);
        Assert.That(redirectResult.ActionName, Is.EqualTo("All"));
        Assert.That(redirectResult.ControllerName, Is.EqualTo("Hotel"));
        Assert.That(controller.TempData[ErrorMessage], Is.EqualTo(HotelDoesNotExists));
    }

    [Test]
    public async Task AddRoom_WhenRoomNumberAlreadyExistsInTheHotel_ShouldReturnModelError()
    {
        mockAgentService.Setup(service => service.ExistsByIdAsync(It.IsAny<string>())).ReturnsAsync(true);
        mockRoomService.Setup(service => service.AllRoomTypesAsync()).ReturnsAsync(new List<RoomTypeServiceModel>());
        mockRoomService.Setup(service => service.GetAllHotelsAsync()).ReturnsAsync(new List<HotelRoomViewModel>());
        mockHotelService.Setup(service => service.HotelExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        mockRoomService.Setup(service => service.RoomTypeExistAsync(It.IsAny<int>())).ReturnsAsync(true);
        mockRoomService.Setup(service => service.RoomNumberAlreadyExistInTheHotelAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);
        mockAgentService.Setup(service => service.HasHotelWithIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var model = new RoomFormModel() { RoomNumber = 101, HotelId = "1" };

        var result = await controller.Add(model, null);

        Assert.IsInstanceOf<ViewResult>(result);
        var resultAsViewResult = (ViewResult)result;

        Assert.IsNotNull(resultAsViewResult);
        Assert.IsFalse(controller.ModelState.IsValid);
        Assert.IsTrue(controller.ModelState.ContainsKey(nameof(model.RoomNumber)));
        Assert.That(controller.ViewData.ModelState[nameof(model.RoomNumber)].Errors[0].ErrorMessage,
                    Is.EqualTo(RoomWithTheSameNumberExists));

        mockAgentService.Verify(s => s.ExistsByIdAsync(It.IsAny<string>()), Times.Once);
        mockRoomService.Verify(s => s.AllRoomTypesAsync(), Times.Once);
        mockRoomService.Verify(s => s.GetAllHotelsAsync(), Times.Once);
        mockRoomService.Verify(service => service.RoomNumberAlreadyExistInTheHotelAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        mockHotelService.Verify(service => service.HotelExistsAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task AddRoom_WhenRoomIsSuccessfullyCreated_ShouldReturnSuccessMessage()
    {
        var hotelName = "Test Hotel";
        var mockImageFile = CreateMockFormFile();

        mockAgentService.Setup(service => service.ExistsByIdAsync(It.IsAny<string>())).ReturnsAsync(true);
        mockRoomService.Setup(service => service.AllRoomTypesAsync()).ReturnsAsync(new List<RoomTypeServiceModel>());
        mockRoomService.Setup(service => service.GetAllHotelsAsync()).ReturnsAsync(new List<HotelRoomViewModel>());
        mockHotelService.Setup(service => service.HotelExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        mockRoomService.Setup(service => service.RoomTypeExistAsync(It.IsAny<int>())).ReturnsAsync(true);
        mockRoomService.Setup(service => service.RoomNumberAlreadyExistInTheHotelAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(false);
        mockHotelService.Setup(service => service.GetHotelNameByIdAsync(It.IsAny<string>())).ReturnsAsync(hotelName);
        mockRoomService.Setup(service => service.CreateRoomAsync(It.IsAny<RoomFormModel>(), It.IsAny<string>(), It.IsAny<IFormFile>())).ReturnsAsync(Guid.NewGuid().ToString());
        mockFileHelper.Setup(helper => helper.UploadFileRoomsAsync(It.IsAny<IFormFile>(), It.IsAny<IWebHostEnvironment>(),"rooms")).ReturnsAsync("path/to/uploaded/file");
        mockAgentService.Setup(service => service.HasHotelWithIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);


        var model = new RoomFormModel() { RoomNumber = 101, HotelId = "1" };

        var result = await controller.Add(model, mockImageFile);

        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var resultAsRedirect = (RedirectToActionResult)result;

        Assert.IsNotNull(resultAsRedirect);
        Assert.That(string.Format(SuccessfullyCreateRoom, hotelName), Is.EqualTo(controller.TempData["SuccessMessage"]));

        mockAgentService.Verify(s => s.ExistsByIdAsync(It.IsAny<string>()), Times.Once);
        mockRoomService.Verify(service => service.RoomNumberAlreadyExistInTheHotelAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        mockHotelService.Verify(service => service.HotelExistsAsync(It.IsAny<string>()), Times.Once);
        mockHotelService.Verify(service => service.GetHotelNameByIdAsync(It.IsAny<string>()), Times.Once);
        mockFileHelper.Verify(helper => helper.UploadFileRoomsAsync(It.IsAny<IFormFile>(), It.IsAny<IWebHostEnvironment>(),"rooms"), Times.Once);
    }

    [Test]
    public async Task EditRoom_WhenUserIsNotAuthorized_ReturnsUnauthorized()
    {
        string existingRoomId = "existingId";
        string agentId = "agentId";

        mockRoomService.Setup(service => service.RoomExistsAsync(existingRoomId)).ReturnsAsync(true);
        mockRoomService.Setup(service => service.HasRoomAgenWithIdAsync(existingRoomId, agentId)).ReturnsAsync(false);

        var result = await controller.Edit(existingRoomId);

        Assert.IsInstanceOf<UnauthorizedResult>(result);

        Assert.IsNotNull(result);
        Assert.That(controller.TempData[ErrorMessage], Is.EqualTo(NotAuthorizedHotelManager));

    }

    [Test]
    public async Task EditRoom_WhenRoomExistsAndUserIsAuthorized_ReturnsViewWithRooms()
    {
        string existingRoomId = "existingId";
        string agentId = "agentId";

        var roomToEditModel = new RoomFormModel
        {
            Description = "Test",
            HotelId = "1",
            imageFile = "test",
            PricePerNight = 10M,
            RoomNumber = 1,
            RoomTypeId = 1,
        };

        mockRoomService.Setup(service => service.RoomExistsAsync(existingRoomId)).ReturnsAsync(true);
        mockAgentService.Setup(service => service.GetAgentIdAsync(It.IsAny<string>())).ReturnsAsync(agentId);
        mockRoomService.Setup(service => service.HasRoomAgenWithIdAsync(agentId, existingRoomId)).ReturnsAsync(true);
        mockRoomService.Setup(service => service.EditRoomByIdAsync(existingRoomId)).ReturnsAsync(roomToEditModel);

        var result = await controller.Edit(existingRoomId);

        Assert.IsInstanceOf<ViewResult>(result);
        var viewResult = (ViewResult)result;
        Assert.IsNotNull(viewResult);
        Assert.That(viewResult.Model, Is.EqualTo(roomToEditModel));
    }

    [Test]
    public async Task EditRoom_WhenRoomDoesNotExist_ShouldRedirectToAllRooms()
    {
        string existingRoomId = "existingId";
        string agentId = "agentId";

        var model = new RoomFormModel
        {
            RoomNumber = 1,
            HotelId = "1",
            Description = "Test",
            PricePerNight = 100,
            RoomTypeId = 1
        };

        mockRoomService.Setup(service => service.RoomExistsAsync(existingRoomId)).ReturnsAsync(true);
        mockAgentService.Setup(service => service.GetAgentIdAsync(It.IsAny<string>())).ReturnsAsync(agentId);
        mockRoomService.Setup(service => service.HasRoomAgenWithIdAsync(existingRoomId, agentId)).ReturnsAsync(true);
        mockRoomService.Setup(service => service.AllRoomsByUsersIdAsync(It.IsAny<string>())).ReturnsAsync((IEnumerable<RoomServiceModel>)null);

        var result = await controller.Edit(existingRoomId, model, null);

        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirectResult = (RedirectToActionResult)result;
        Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(controller.All)));
        Assert.That(redirectResult.ControllerName, Is.EqualTo("Room"));
        Assert.That(controller.TempData[ErrorMessage], Is.EqualTo(UserDoesNotHaveRoom));

        mockRoomService.Verify(service => service.AllRoomsByUsersIdAsync(existingRoomId), Times.Once);
    }

    [Test]
    public async Task EditRoom_WhenEverythingIsCorrect_ShouldEditRoomAndRedirectToDetails()
    {
        string existingRoomId = "existingId";
        string agentId = "agentId";
        string imagePath = "path/to/uploaded/file";


        var model = new RoomFormModel
        {
            RoomNumber = 1,
            HotelId = "1",
            Description = "Test",
            PricePerNight = 100,
            RoomTypeId = 1,
            imageFile = imagePath
        };

        var mockImageFile = CreateMockFormFile();

        mockRoomService.Setup(service => service.RoomExistsAsync(existingRoomId)).ReturnsAsync(true);
        mockAgentService.Setup(service => service.GetAgentIdAsync(It.IsAny<string>())).ReturnsAsync(agentId);
        mockRoomService.Setup(service => service.HasRoomAgenWithIdAsync(agentId, existingRoomId)).ReturnsAsync(true);
        mockRoomService.Setup(service => service.AllRoomsByUsersIdAsync(It.IsAny<string>())).ReturnsAsync(new List<RoomServiceModel> { new RoomServiceModel { Id = existingRoomId } });
        mockRoomService.Setup(service => service.EditRoomByFormModel(It.IsAny<RoomFormModel>(), It.IsAny<string>(), It.IsAny<IFormFile>())).Returns(Task.CompletedTask);
        mockFileHelper.Setup(helper => helper.UploadFileRoomsAsync(It.IsAny<IFormFile>(), It.IsAny<IWebHostEnvironment>(),"rooms")).ReturnsAsync(imagePath);

        var result = await controller.Edit(existingRoomId, model, mockImageFile);

        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirectResult = (RedirectToActionResult)result;
        Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(controller.Details)));
        Assert.That(redirectResult.RouteValues["id"], Is.EqualTo(existingRoomId));
    }

    [Test]
    public async Task RoomDelete_ShouldReturnCorrectViewForDeleting()
    {
        string existingRoomId = "existingId";
        string agentId = "agentId";

        var roomToDeleteModel = new RoomPreDeleteViewModel
        {
            HotelName = "Luxory Hotel",
            PricePerNight = 10M
        };


        mockRoomService.Setup(service => service.RoomExistsAsync(existingRoomId)).ReturnsAsync(true);
        mockAgentService.Setup(service => service.GetAgentIdAsync(It.IsAny<string>())).ReturnsAsync(agentId);
        mockRoomService.Setup(service => service.HasRoomAgenWithIdAsync(agentId, existingRoomId)).ReturnsAsync(true);
        mockRoomService.Setup(service => service.DeteleRoomFormModelByIdAsync(existingRoomId)).ReturnsAsync(roomToDeleteModel);

        var result = await controller.Delete(existingRoomId);

        Assert.IsInstanceOf<ViewResult>(result);
        var viewResult = result as ViewResult;

        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as RoomPreDeleteViewModel;

        Assert.That(roomToDeleteModel.HotelName, Is.EqualTo(model.HotelName));
        Assert.That(roomToDeleteModel.PricePerNight, Is.EqualTo(model.PricePerNight));
    }

    [Test]
    public async Task DeleteRoom_ShouldSuccessfullyDeleteRoom()
    {
        string existingRoomId = "existingId";
        string agentId = "agentId";
        string hotelName = "Test Hotel";

        var model = new RoomPreDeleteViewModel
        {
            HotelName = "1"
        };

        mockAgentService.Setup(service => service.GetAgentIdAsync(It.IsAny<string>())).ReturnsAsync(agentId);
        mockRoomService.Setup(service => service.HasRoomAgenWithIdAsync(agentId, existingRoomId)).ReturnsAsync(true);
        mockHotelService.Setup(service => service.GetHotelNameByIdAsync(It.IsAny<string>())).ReturnsAsync(hotelName);
        mockRoomService.Setup(service => service.DeleteRoomByIdAsync(existingRoomId)).Returns(Task.CompletedTask);

        var result = await controller.Delete(model, existingRoomId);

        Assert.IsInstanceOf<RedirectToActionResult>(result);

        var resultAsRedirect = (RedirectToActionResult)result;
        Assert.That(resultAsRedirect.ActionName, Is.EqualTo(nameof(controller.All)));
        Assert.That(resultAsRedirect.ControllerName, Is.EqualTo("Room"));
        Assert.That(controller.TempData[WarningMessage], Is.EqualTo(string.Format(SuccessfullyDeleteRoom, hotelName)));
    }

    [Test]
    public async Task DeleteRoom_ShouldDeleteRoomSuccsssfullyWhenRoomHasReview()
    {
        string existingRoomId = "existingId";
        string agentId = "agentId";
        string hotelName = "Test Hotel";

        var model = new RoomPreDeleteViewModel
        {
            HotelName = "1"
        };

        mockAgentService.Setup(service => service.GetAgentIdAsync(It.IsAny<string>())).ReturnsAsync(agentId);
        mockRoomService.Setup(service => service.HasRoomAgenWithIdAsync(agentId, existingRoomId)).ReturnsAsync(true);
        mockHotelService.Setup(service => service.GetHotelNameByIdAsync(It.IsAny<string>())).ReturnsAsync(hotelName);
        mockRoomService.Setup(service => service.DeleteRoomByIdAsync(existingRoomId)).Returns(Task.CompletedTask);
        mockRoomService.Setup(service => service.RoomHasReviewAsync(existingRoomId)).ReturnsAsync(true);
        mockReviewService.Setup(service => service.DeleteReviewsByRoomIdAsync(existingRoomId)).Returns(Task.CompletedTask);

        var result = await controller.Delete(model, existingRoomId);

        Assert.IsInstanceOf<RedirectToActionResult>(result);

        var resultAsRedirect = (RedirectToActionResult)result;
        Assert.That(resultAsRedirect.ActionName, Is.EqualTo(nameof(controller.All)));
        Assert.That(resultAsRedirect.ControllerName, Is.EqualTo("Room"));
        Assert.That(controller.TempData[WarningMessage], Is.EqualTo(string.Format(SuccessfullyDeleteRoom, hotelName)));
    }

    [Test]
    public async Task RentRoom_ShouldRedirectToAllRoomsWhenRoomIsRented()
    {
        string existingRoomId = "existingId";

        mockRoomService.Setup(service => service.IsRentedAsync(existingRoomId)).ReturnsAsync(true);
        mockRoomService.Setup(service => service.RoomExistsAsync(existingRoomId)).ReturnsAsync(true);

        var result = await controller.Rent(existingRoomId);

        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var resultAsRedirect = (RedirectToActionResult)result;
        Assert.That(resultAsRedirect.ActionName, Is.EqualTo("All"));
        Assert.That(resultAsRedirect.ControllerName, Is.EqualTo("Room"));
        Assert.That(controller.TempData[ErrorMessage], Is.EqualTo(RoomIsAlreadyRented));
    }

    [Test]
    public async Task RentRoom_ShouldRedirectToAllRoomsWhenRoomDoesNotExists()
    {
        string nonExistingRoomId = "nonExistingRoomId";

        mockRoomService.Setup(service => service.RoomExistsAsync(nonExistingRoomId)).ReturnsAsync(false);

        var result = await controller.Rent(nonExistingRoomId);

        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var resultAsRedirect = (RedirectToActionResult)result;
        Assert.That(resultAsRedirect.ActionName, Is.EqualTo("All"));
        Assert.That(resultAsRedirect.ControllerName, Is.EqualTo("Room"));
        Assert.That(controller.TempData[ErrorMessage], Is.EqualTo(RoomIdDoesNotExists));
    }

    [Test]
    public async Task RentRoom_ShouldRedirectToIndexHomeWhenuserIsAdminOrAgent()
    {
        string existingRoomId = "existingId";

        mockRoomService.Setup(service => service.RoomExistsAsync(existingRoomId)).ReturnsAsync(true);
        mockRoomService.Setup(service => service.IsRentedAsync(existingRoomId)).ReturnsAsync(false);
        mockAgentService.Setup(service => service.ExistsByIdAsync(It.IsAny<string>())).ReturnsAsync(true);
        mockRoomService.Setup(service => service.GetRoomByIdAsync(existingRoomId)).ReturnsAsync(new RoomServiceModel { Id = existingRoomId });

        var result = await controller.Rent(existingRoomId);

        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var resultAsRedirect = (RedirectToActionResult)result;

        Assert.That(resultAsRedirect.ActionName, Is.EqualTo("Index"));
        Assert.That(resultAsRedirect.ControllerName, Is.EqualTo("Home"));
        Assert.That(controller.TempData[ErrorMessage], Is.EqualTo(AgenCannonRentRoom));
    }

    [Test]
    public async Task Rent_ShouldRedirectToDetails_WhenRoomIsSuccessfullyRented()
    {
        string roomId = "existingRoomId";
        string userId = "userId";
        var hotelName = "Luxory Place resort";


        var model = new RoomServiceModel
        {
            Id = roomId,
            HotelId = "hotelId",
            HotelName = hotelName,
        };

        mockRoomService.Setup(service => service.RoomExistsAsync(roomId)).ReturnsAsync(true);
        mockRoomService.Setup(service => service.IsRentedAsync(roomId)).ReturnsAsync(false);
        mockAgentService.Setup(service => service.ExistsByIdAsync(It.IsAny<string>())).ReturnsAsync(false);
        mockRoomService.Setup(service => service.GetRoomByIdAsync(roomId)).ReturnsAsync(model);
        mockRoomService.Setup(service => service.RentAsync(roomId, userId, It.IsAny<string>())).Returns(Task.CompletedTask);

        var result = await controller.Rent(roomId);

        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var resultAsRedirect = (RedirectToActionResult)result;
        Assert.That(resultAsRedirect.ActionName, Is.EqualTo("Details"));
        Assert.That(resultAsRedirect.RouteValues["id"], Is.EqualTo(roomId));
        Assert.That(controller.TempData[SuccessMessage], Is.EqualTo(string.Format(SuccessfullyRentedRoom, hotelName)));
    }

    [Test]
    public async Task LeaveRoom_ShouldLeaveRoomSuccessfully()
    {
        string roomId = "existingRoomId";
        string userId = "userId";
        var hotelName = "Luxory Place resort";

        var model = new RoomServiceModel
        {
            Id = roomId,
            HotelId = "hotelId",
            HotelName = hotelName
        };

        mockRoomService.Setup(service => service.RoomExistsAsync(roomId)).ReturnsAsync(true);
        mockRoomService.Setup(service => service.GetRoomByIdAsync(roomId)).ReturnsAsync(model);
        mockRoomService.Setup(service => service.LeaveAsync(userId, roomId, It.IsAny<string>())).Returns(Task.CompletedTask);

        var result = await controller.Leave(roomId);


        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var resultAsRedirect = (RedirectToActionResult)result;
        Assert.That(resultAsRedirect.ActionName, Is.EqualTo("Details"));
        Assert.That(resultAsRedirect.RouteValues["id"], Is.EqualTo(roomId));
        Assert.That(controller.TempData[SuccessMessage], Is.EqualTo(string.Format(SuccessfullyRentedRoom, hotelName)));

    }


    [TearDown]
    public void OneTimeTearDown()
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
