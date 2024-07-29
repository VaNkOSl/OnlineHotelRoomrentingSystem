namespace OnlineHotelRoomrentingSystem.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Agent;
using OnlineHotelRoomBookingSystem.Web.Infrastructure.Extensions;
using static OnlineHotelRoomrentingSystem.Commons.NotificationMessagesConstants;
using static OnlineHotelRoomrentingSystem.Commons.MessagesConstants;

public class AgentController : BaseController
{
    private readonly IAgentService agentService;

    public AgentController(IAgentService _agentService)
    {
        this.agentService = _agentService;  
    }

    [HttpGet]
    public async Task<IActionResult> Become()
    {
        if(await this.agentService.ExistsByIdAsync(User.GetId()!))
        {
            TempData[ErrorMessage] = UserDoesNotExist;
        }

        return View();  
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Become(BecomeAgentFormModel model)
    {
        var userId = User.GetId();

        if (await this.agentService.UserWithEgnExistAsync(model.EGN))
        {
            ModelState.AddModelError(nameof(model.EGN), UserWithSameEgnExist);
        }

        if (await this.agentService.UserWithPhoneNumberExistAsync(model.PhoneNumber))
        {
            ModelState.AddModelError(nameof(model.PhoneNumber), UserWithTheSamePhoneNumberExists);
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (await this.agentService.ExistsByIdAsync(userId))
        {
            TempData[ErrorMessage] = UserDoesNotExist;
        }

        await this.agentService.CreateAsync(model, userId);
        TempData[SuccessMessage] = SuccessfullyBecomeAHotelManager;
        return RedirectToAction(nameof(HotelController.All), "Hotel");
    }
}
