namespace OnlineHotelRoomrentingSystem.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.Infrastructure.Extensions;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Hotel;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Review;
using OnlineHotelRoomrentingSystem.Extensions;
using OnlineHotelRoomrentingSystem.Extensions.Contacts;
using static OnlineHotelRoomrentingSystem.Commons.GeneralApplicationConstants;
using static OnlineHotelRoomrentingSystem.Commons.MessagesConstants;
using static OnlineHotelRoomrentingSystem.Commons.NotificationMessagesConstants;

public class HotelController : BaseController
{
    private readonly IHotelService hotelService;
    private readonly IAgentService agentService;
    private readonly ICategoryService categoryService;
    private readonly IReviewService reviewService;
    private readonly IRoomService roomService;
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly IFileService fileService;

    public HotelController(IHotelService _hotelService, IAgentService _agentService,
                           IWebHostEnvironment _webHostEnvironment, ICategoryService _categoryService,
                           IReviewService _reviewService, IRoomService _roomService, IFileService _fileService)
    {
        this.hotelService = _hotelService;
        this.agentService = _agentService;
        this.webHostEnvironment = _webHostEnvironment;
        this.categoryService = _categoryService;
        this.reviewService = _reviewService;
        this.roomService = _roomService;
        this.fileService = _fileService;

    }
    [AllowAnonymous]
    public async Task<IActionResult> All([FromQuery] AllHotelsQueryModel model)
    {
        HotelQueryServiceModel serviceModel = await hotelService.AllHotelsAsync(model);

        model.Hotels = serviceModel.Hotels;
        model.TotalHotels = serviceModel.TotalHotelsCount;
        model.Categories = await categoryService.AllCategoriesNamesAsync();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Mine()
    {     
       IEnumerable<HotelServiceModel>? myHotels = null;
  
       var userId = User.GetId();

        if (await agentService.ExistsByIdAsync(userId!))
        {
            if (User.IsInRole(AdminRoleName))
            {
                return RedirectToAction("Mine", "Hotel", new { area = AdminAreaName });
            }

            var currentAgentId = await agentService.GetAgentIdAsync(userId!);

            if (currentAgentId != null)
            {
                myHotels = await hotelService.AllHotelsByAgentIdAsync(currentAgentId);
            }
        }
        return View(myHotels); 
    }

    [HttpPost]
    public async Task<IActionResult> AddReview(HotelReviewViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var userId = User.GetId();
                var reviewId = await reviewService.CreateHotelReviewAsync(model, userId);
                TempData[SuccessMessage] = SuccessfullyAddedReview;
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Възникна грешка при добавяне на ревю.");
            }
        }

        var hotelModel = await hotelService.HotelDetailsByIdAsync(model.HotelId);
        return View("Details", hotelModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        if (await hotelService.HotelExistsAsync(id) == false)
        {
            TempData[ErrorMessage] = HotelIdDoesNotExist;
            return RedirectToAction("All", "Hotel");
        }

        var hotelModel = await hotelService.HotelDetailsByIdAsync(id);

        if(hotelModel == null)
        {
            TempData[ErrorMessage] = HotelFormModelIsNotFound;
            return RedirectToAction("All", "Hotel");
        }

        try
        {
            hotelModel.Rooms = await roomService.GetRoomsByHotelIdAsync(id);

            return View(hotelModel);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        if(await this.agentService.ExistsByIdAsync(User.GetId()!) == false)
        {
            TempData[ErrorMessage] = AgenDoesNotExists;
            return RedirectToAction(nameof(AgentController.Become), "Agent");
        }

        if(await agentService.IsAgentApprovedAsync(User.GetId()!) == false)
        {
            TempData[ErrorMessage] = StillNonAppoveForHotelManager;
            return RedirectToAction(nameof(All), "Hotel");
        }

        return View(new HotelFormModel
        {
           Categories = await this.hotelService.AllCategoriesAsync()
        });
    }

    [HttpPost]
    public async Task<IActionResult> Add(HotelFormModel model,IFormFile imageFile)
    {
        try
        {
            var userId = User.GetId();

            if (imageFile != null)
            {
                model.imageFile = await fileService.UploadFileAsync(imageFile, webHostEnvironment);
            }
            else
            {
                ModelState.AddModelError(nameof(model.imageFile), NoPhotoSelected);
            }

            if (await this.agentService.ExistsByIdAsync(userId!) == false)
            {
                TempData[ErrorMessage] = AgenDoesNotExists;
                return RedirectToAction(nameof(AgentController.Become), "Agent");
            }

            if (await this.hotelService.CategoryExistAsync(model.CategoryId) == false)
            {
                ModelState.AddModelError(nameof(model.CategoryId), CategoryDoesNotExists);
            }

            if(await this.hotelService.HotelWithTheSameEmailExistAsync(model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), SameHotelEmail);
            }

            if(await this.hotelService.HotelWithTheSamePhoneNumberExist(model.PhoneNumber))
            {
                ModelState.AddModelError(nameof(model.PhoneNumber), SameHotelPhoneNumbers);
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await this.hotelService.AllCategoriesAsync();
                return View(model);
            }

            string? hotelManager = await this.agentService.GetAgentIdAsync(userId!);

            var hotel = await this.hotelService.CreateHotelAsync(model, hotelManager!, imageFile);

            TempData[SuccessMessage] = SuccessfullyCreatedHotel;

            return RedirectToAction(nameof(Details), new { id = hotel });
        }
        catch(Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorWhileCreatedHotel);
            model.Categories = await this.hotelService.AllCategoriesAsync();
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if(await hotelService.HotelExistsAsync(id) == false ) 
        {
            TempData[ErrorMessage] = HotelDoesNotExists;
            return RedirectToAction(nameof(All), "Hotel");
        }

        if (await agentService.IsAgentApprovedAsync(User.GetId()!) == false)
        {
            TempData[ErrorMessage] = StillNonAppoveForHotelManager;
            return RedirectToAction(nameof(All), "Hotel");
        }

        string? agenId = await agentService.GetAgentIdAsync(User.GetId()!);

        if(await hotelService.HasHotelManagerWithIdAsync(id,agenId!) == false && User.IsAdmin() == false)
        {
            TempData[ErrorMessage] = NotAuthorizedHotelManager;
           return Unauthorized();
        }

        var hotelToEdit = await hotelService.EditHotelByIdAsync(id);

        return View(hotelToEdit);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, HotelFormModel model, IFormFile imageFile)
    {
        if (imageFile != null)
        {
            model.imageFile = await fileService.UploadFileAsync(imageFile, webHostEnvironment);
        }
        else
        {
            ModelState.AddModelError(nameof(model.imageFile), NoPhotoSelected);
        }

        if (await hotelService.HotelExistsAsync(id) == false)
        {
            TempData[ErrorMessage] = HotelDoesNotExists;
            return RedirectToAction(nameof(All), "Hotel");
        }

        string? agenId = await agentService.GetAgentIdAsync(User.GetId()!);

        if (await hotelService.HasHotelManagerWithIdAsync(id, agenId!) == false && User.IsAdmin() == false)
        {
            TempData[ErrorMessage] = NotAuthorizedHotelManager;
            return Unauthorized();
        }

        if (await this.hotelService.CategoryExistAsync(model.CategoryId) == false)
        {
            ModelState.AddModelError(nameof(model.CategoryId), CategoryDoesNotExists);
        }

        if (!ModelState.IsValid)
        {
            model.Categories = await hotelService.AllCategoriesAsync();
            return View(model);
        }

        await hotelService.EditHotelByFormModel(model, id,imageFile);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        string? agenId = await agentService.GetAgentIdAsync(User.GetId()!);

        if(agenId  == null)
        {
            TempData[ErrorMessage] = AgenIdDoesNotExists;
           return RedirectToAction(nameof(AgentController.Become),"Agent"); 
        }

        if (await hotelService.HotelExistsAsync(id) == false )
        {
            TempData[ErrorMessage] = HotelDoesNotExists;
            return RedirectToAction(nameof(All), "Hotel");
        }

        if (await agentService.IsAgentApprovedAsync(User.GetId()!) == false)
        {
            TempData[ErrorMessage] = StillNonAppoveForHotelManager;
            return RedirectToAction(nameof(All), "Hotel");
        }

        if (await hotelService.HasHotelManagerWithIdAsync(id,agenId) == false && User.IsAdmin() == false)
        {
            TempData[ErrorMessage] = NotAuthorizedHotelManager;
            return Unauthorized();
        }

        try
        {
          HotelPreDeleteDetailsViewModel model = await
                hotelService.GetHotelForDeleteByIdAsync(id);

            return View(model);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(HotelDetailsViewModel model,string id)
    {
        string? agenId = await agentService.GetAgentIdAsync(User.GetId()!);

        if (agenId == null)
        {
            TempData[ErrorMessage] = AgenIdDoesNotExists;
            return RedirectToAction(nameof(AgentController.Become), "Agent");
        }

        if (await hotelService.HotelExistsAsync(id) == false)
        {
            TempData[ErrorMessage] = HotelDoesNotExists;
            return RedirectToAction(nameof(All), "Hotel");
        }


        if (await hotelService.HasHotelManagerWithIdAsync(id, agenId) == false && User.IsAdmin() == false)
        {
            TempData[ErrorMessage] = NotAuthorizedHotelManager;
            return Unauthorized();
        }

        try
        {
            bool hasRooms = await hotelService.HasRoomsAsync(id);
            if (hasRooms)
            {
                await hotelService.DeleteRoomsByHotelIdAsync(id);
            }

            await hotelService.DeleteHotelByIdAsync(id);
            TempData[WarningMessage] = SuccessfullyDeletedHotel;
            return RedirectToAction(nameof(All), "Hotel");
        }
        catch (Exception )
        {
            throw;
        }
    }
    private IActionResult GeneralError()
    {
        TempData[ErrorMessage] = GeneralErrors;

        return RedirectToAction("Index", "Home");
    }
}
