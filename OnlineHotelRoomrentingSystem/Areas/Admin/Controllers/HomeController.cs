namespace OnlineHotelRoomrentingSystem.Areas.Admin.Controllers;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnilineHotelRoomBookingSystem.Services.Data;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomrentingSystem.Models;

public class HomeController : AdminBaseController
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IHotelService hotelService;
    private readonly IAgentService agentService;

    public HomeController(UserManager<ApplicationUser> _userManager,
                         IHotelService _hotelServie,IAgentService _agentService)
    {
        userManager = _userManager; 
        hotelService = _hotelServie;
        agentService = _agentService;
    }
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ForReview()
    {
        var model = await hotelService.GetUnApprovedAsync();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Approve(string hotelId)
    {
        if (!Guid.TryParse(hotelId, out Guid hotelGuid))
        {
            throw new ArgumentException("Invalid hotel ID format", nameof(hotelId));
        }


        await hotelService.ApproveHotelAsync(hotelGuid);
        return RedirectToAction(nameof(ForReview));
    }

    [HttpGet]
    public async Task<IActionResult> AgentsForReview()
    {
        var model = await agentService.GetUnapprovedAgentsAsync();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ApproveAgent(string agentId)
    {
        if (!Guid.TryParse(agentId, out Guid agentGuid))
        {
            throw new ArgumentException("Invalid agent ID format", nameof(agentId));
        }

        await agentService.ApproveAgentAsync(agentGuid);
        return RedirectToAction(nameof(AgentsForReview));
    }
}
