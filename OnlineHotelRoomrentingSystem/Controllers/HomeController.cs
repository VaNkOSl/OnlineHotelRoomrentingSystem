namespace OnlineHotelRoomrentingSystem.Controllers;

using Microsoft.AspNetCore.Mvc;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomrentingSystem.Models;
using System.Diagnostics;
using static OnlineHotelRoomrentingSystem.Commons.GeneralApplicationConstants;
public class HomeController : BaseController
{
    private readonly IHotelService hotelService;

    public HomeController(IHotelService _hotelService)
    {
        this.hotelService = _hotelService;  
    }
    public async Task<IActionResult> Index()
    {
        if(User.IsInRole(AdminRoleName))
        {
            return RedirectToAction("Dashboard", "Home", new { area = "Admin" });
        }

        var hotels = await this.hotelService.LastThreeHotelsAsync();
        return View(hotels);
    }

    public async Task<IActionResult> Chat()
    {
        return View();
    }
    public async Task<IActionResult> Error(int statusCode)
    {
        if (statusCode == 400)
        {
            return View("Error400");
        }

        if (statusCode == 401)
        {
            return View("Error401");
        }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
