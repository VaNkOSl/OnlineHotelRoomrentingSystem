using Microsoft.AspNetCore.Mvc;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.Infrastructure.Extensions;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Admin;

namespace OnlineHotelRoomrentingSystem.Areas.Admin.Controllers;

public class RoomController : AdminBaseController
{

    private readonly IHotelService hotelService;
    private readonly IRoomService roomService;
    private readonly IAgentService agentService;

    public RoomController(IHotelService _hotelService, IRoomService _roomService,
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

        var model = new MyRoomsViewModel
        {
            AddedRooms = await roomService.AllRoomsByAgentIdAsync(agentid!),
        };

        return View(model);
    }
}
