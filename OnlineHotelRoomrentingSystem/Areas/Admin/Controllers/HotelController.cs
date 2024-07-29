namespace OnlineHotelRoomrentingSystem.Areas.Admin.Controllers;

using Microsoft.AspNetCore.Mvc;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.Infrastructure.Extensions;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Admin;

public class HotelController : AdminBaseController
{
    private readonly IHotelService hotelService;
    private readonly IRoomService roomService;
    private readonly IAgentService agentService;
    private readonly ILogger logger;

    public HotelController(IHotelService _hotelService,IRoomService _roomService,
                           IAgentService _agentService)
    {
        hotelService = _hotelService;
        roomService = _roomService;
        agentService = _agentService;

    }
    public async Task<IActionResult> Mine()
    {
        var userId = User.GetId();
        var agentid = await agentService.GetAgentIdAsync(userId!);

        var model = new MyHotelsViewModel
        {
            AddedHotels = await hotelService.AllHotelsByAgentIdAsync(agentid!),
        };

        return View(model);
    }


}
