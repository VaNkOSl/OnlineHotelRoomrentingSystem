namespace OnlineHotelRoomrentingSystem.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.Infrastructure.Extensions;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Review;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Room;
using OnlineHotelRoomrentingSystem.Extensions.Contacts;
using static OnlineHotelRoomrentingSystem.Commons.GeneralApplicationConstants;
using static OnlineHotelRoomrentingSystem.Commons.MessagesConstants;
using static OnlineHotelRoomrentingSystem.Commons.NotificationMessagesConstants;

public class RoomController : BaseController
{
    private readonly IRoomService roomService;
    private readonly IAgentService agentService;
    private readonly IHotelService hotelService;
    private readonly IReviewService reviewService;
    private readonly IFileService fileHelper;

    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly IMemoryCache memoryCache;

    public RoomController(IRoomService _roomService,IAgentService _agentService,
                          IHotelService _hotelService, IWebHostEnvironment _webHostEnvironment,
                          IReviewService _reviewService, IMemoryCache _memoryCache, IFileService _fileHelper)
    {
        roomService = _roomService;
        agentService = _agentService;
        hotelService = _hotelService;
        webHostEnvironment = _webHostEnvironment;
        reviewService = _reviewService;
        memoryCache = _memoryCache;
        fileHelper = _fileHelper;
    }

    [AllowAnonymous]
    public async Task<IActionResult> All()
    {
        var model = await roomService.GetAllRoomsAsync();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Mine()
    {
        //IEnumerable<RoomServiceModel?> myRooms = null!;
        IEnumerable<RoomServiceModel?> myRooms = new List<RoomServiceModel>();

        var userId = User.GetId();

        if(await agentService.ExistsByIdAsync(userId!))
        {
            if (User.IsInRole(AdminRoleName))
            {
                return RedirectToAction("Mine", "Room", new { area = "Admin" });
            }

            var currentUserId = await agentService.GetAgentIdAsync(userId!);
            if(currentUserId != null)
            {
                myRooms =  await roomService.AllRoomsByAgentIdAsync(currentUserId);
            }
        }
        return View(myRooms);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> AddReview(RoomReviewViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var userId = User.GetId();

                var reviewId = await reviewService.CreateRoomReviewAsync(model, userId);
                TempData[SuccessMessage] = SuccessfullyAddedReview;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while adding a review: {ex.Message}");
            }
        }
        else
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        }

        var roomModel = await roomService.RoomDetailsByIdAsync(model.RoomId);
        return View("Details", roomModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        if(await roomService.RoomExistsAsync(id) == false)
        {
            TempData[ErrorMessage] = RoomIdDoesNotExists;
            return RedirectToAction("All", "Room");
        }

        var roomModel = await roomService.RoomDetailsByIdAsync(id);
        
        if(roomModel == null)
        {
            TempData[ErrorMessage] = RoomFormModelIsNotFound;
            return RedirectToAction("All", "Room");
        }

        return View(roomModel);
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        if(await agentService.ExistsByIdAsync(User.GetId()!) == false)
        {
            TempData[ErrorMessage] = AgenDoesNotExists;
            return RedirectToAction(nameof(AgentController.Become), "Agent");
        }

        if (await agentService.IsAgentApprovedAsync(User.GetId()!) == false)
        {
            TempData[ErrorMessage] = StillNonAppoveForHotelManager;
            return RedirectToAction(nameof(All), "Hotel");
        }

        var model = new RoomFormModel
        {
          RoomTypes = await roomService.AllRoomTypesAsync(),
          Hotels = await roomService.GetAllHotelsAsync()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Add(RoomFormModel model, IFormFile imageFile)
    {
        try
        {
            var userId = User.GetId()!;

            if (imageFile != null)
            {
                model.imageFile = await fileHelper.UploadFileRoomsAsync(imageFile, webHostEnvironment);
            }
            else
            {
                ModelState.AddModelError(nameof(model.imageFile), NoPhotoSelected);
            }

            if (await agentService.ExistsByIdAsync(userId) == false)
            {
                TempData[ErrorMessage] = AgenDoesNotExists;
                return RedirectToAction(nameof(AgentController.Become), "Agent");
            }

            if (await roomService.RoomTypeExistAsync(model.RoomTypeId) == false)
            {
                TempData[ErrorMessage] = RoomTypeDoesNotExists;
                return RedirectToAction(nameof(Add), "Room");
            }

            if (await hotelService.HotelExistsAsync(model.HotelId) == false)
            {
                TempData[ErrorMessage] = HotelDoesNotExists;
                return RedirectToAction(nameof(All), "Hotel");
            }

            if (await agentService.HasHotelWithIdAsync(userId, model.HotelId) == false)
            {
                TempData["ErrorMessage"] = TheAgentIsNotTheOwnerOfTheHotel;
                return RedirectToAction(nameof(HotelController.Add), "Hotel");
            }

            if (await roomService.RoomNumberAlreadyExistInTheHotelAsync(model.RoomNumber, model.HotelId))
            {
                ModelState.AddModelError(nameof(model.RoomNumber),RoomWithTheSameNumberExists);
                model.RoomTypes = await roomService.AllRoomTypesAsync();
                model.Hotels = await roomService.GetAllHotelsAsync();
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                model.Hotels = await roomService.GetAllHotelsAsync();
                return View(model);
            }
                                                             
            string? agentId = await agentService.GetAgentIdAsync(userId);
            var hotelName = await hotelService.GetHotelNameByIdAsync(model.HotelId);
            var roomId = await roomService.CreateRoomAsync(model,agentId!,imageFile);
            TempData[SuccessMessage] = string.Format(SuccessfullyCreateRoom, hotelName); 

            return RedirectToAction("Details", new { id = roomId });

        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty,ErrorWhileCreateRoom);
            model.RoomTypes = await roomService.AllRoomTypesAsync();
            model.Hotels = await roomService.GetAllHotelsAsync();
            return View(model);
        }

    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if(await roomService.RoomExistsAsync(id) == false)
        {
            TempData[ErrorMessage] = RoomIdDoesNotExists;
            return RedirectToAction(nameof(All), "Room");
        }

        if (await agentService.IsAgentApprovedAsync(User.GetId()!) == false)
        {
            TempData[ErrorMessage] = StillNonAppoveForHotelManager;
            return RedirectToAction(nameof(All), "Hotel");
        }

        string? agentId = await agentService.GetAgentIdAsync(User.GetId()!);

        if (await roomService.HasRoomAgenWithIdAsync(agentId!, id) == false && User.IsAdmin() == false)
        {
            TempData[ErrorMessage] = NotAuthorizedHotelManager;
            return Unauthorized();
        }
        try
        {
            var roomFormModel = await roomService.EditRoomByIdAsync(id);
            return View(roomFormModel);
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, CannonEditRoomNow);
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id,RoomFormModel model,IFormFile imageFile)
    {
        try
        {
            if (imageFile != null)
            {
                model.imageFile = await fileHelper.UploadFileRoomsAsync(imageFile, webHostEnvironment);
            }
            else
            {
                ModelState.AddModelError(nameof(model.imageFile), NoPhotoSelected);
            }

            var existingRoom = await roomService.AllRoomsByUsersIdAsync(id);

            if (existingRoom == null)
            {
                TempData[ErrorMessage] = UserDoesNotHaveRoom;
                return RedirectToAction(nameof(All), "Room");
            }

            string? agentId = await agentService.GetAgentIdAsync(User.GetId()!);

            if (await roomService.HasRoomAgenWithIdAsync(agentId!, id) == false && User.IsAdmin() == false)
            {
                TempData[ErrorMessage] = NotAuthorizedHotelManager;
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await roomService.EditRoomByFormModel(model, id, imageFile);
            return RedirectToAction(nameof(Details), new { id = id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, CannonEditRoomNow);
            return RedirectToAction(nameof(Details), new { id });
        }
 
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        if (await roomService.RoomExistsAsync(id) == false)
        {
            TempData[ErrorMessage] = RoomIdDoesNotExists;
            return RedirectToAction(nameof(All), "Room");
        }

        if (await agentService.IsAgentApprovedAsync(User.GetId()!) == false)
        {
            TempData[ErrorMessage] = StillNonAppoveForHotelManager;
            return RedirectToAction(nameof(All), "Hotel");
        }

        string? agentId = await agentService.GetAgentIdAsync(User.GetId()!);

        if (await roomService.HasRoomAgenWithIdAsync(agentId!, id) == false && User.IsAdmin() == false)
        {
            TempData[ErrorMessage] = NotAuthorizedHotelManager;
            return Unauthorized();
        }

        try
        {
            RoomPreDeleteViewModel roomPreDeleteViewModel = await
                roomService.DeteleRoomFormModelByIdAsync(id);

             return View(roomPreDeleteViewModel);
        }
        catch (Exception ex)
        {
            return GeneralError();
        }
    }

    [HttpPost] 
    public async Task<IActionResult> Delete(RoomPreDeleteViewModel model,string id)
    {
        string? agentId = await agentService.GetAgentIdAsync(User.GetId()!);

        if (await roomService.HasRoomAgenWithIdAsync(agentId!, id) == false && User.IsAdmin() == false)
        {
            TempData[ErrorMessage] = NotAuthorizedHotelManager;
            return Unauthorized();
        }

        try
        {
            var hotelName = await hotelService.GetHotelNameByIdAsync(model.HotelId);
            await roomService.DeleteRoomByIdAsync(id);
            TempData[WarningMessage] = string.Format(SuccessfullyDeleteRoom, hotelName);
            return RedirectToAction(nameof(All), "Room");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, CannonDeleteHotel);
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Rent(string id)
    {
        if (await roomService.RoomExistsAsync(id) == false)
        {
            TempData[ErrorMessage] = RoomIdDoesNotExists;
            return RedirectToAction(nameof(All), "Room");
        }

        if (await roomService.IsRentedAsync(id))
        {
            TempData[ErrorMessage] = RoomIsAlreadyRented;
            return RedirectToAction(nameof(All), "Room");
        }

        bool isUserAgent =
             await agentService.ExistsByIdAsync(User.GetId());

        if (isUserAgent && !User.IsAdmin())
        {
            TempData[ErrorMessage] = AgenCannonRentRoom;
            return RedirectToAction("Index", "Home");
        }

        var room = await roomService.GetRoomByIdAsync(id);
        if (room == null)
        {
            TempData[ErrorMessage] = RoomIdDoesNotExists;
            return NotFound();
        }

        try
        {
            var hotelId = room.HotelId.ToString();
            await roomService.RentAsync(id, User.GetId()!, hotelId);

            memoryCache.Remove(RentsCacheKey);
            TempData[SuccessMessage] = string.Format(SuccessfullyRentedRoom, room.HotelName);
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            return GeneralError();
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Leave(string id)
    {
        var userId = User.GetId()!;

        var room = await roomService.GetRoomByIdAsync(id);

        if (room == null)
        {
            TempData[ErrorMessage] = RoomIdDoesNotExists;
            return NotFound();
        }

        try
        {
            var hotelId = room.HotelId.ToString();
            await roomService.LeaveAsync(userId, id, hotelId);
            memoryCache.Remove(RentsCacheKey);
            TempData[SuccessMessage] = string.Format(SuccessfullyLeavedRoom,room.HotelName);

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            return GeneralError();
        }
    }

    private IActionResult GeneralError()
    {
        TempData[ErrorMessage] = GeneralErrors;

        return RedirectToAction("Index", "Home");
    }
}
