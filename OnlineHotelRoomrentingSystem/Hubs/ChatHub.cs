namespace OnlineHotelRoomrentingSystem.Hubs;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using OnlineHotelRoomBookingSystem.Web.Infrastructure.Extensions;
using OnlineHotelRoomrentingSystem.Models;

public class ChatHub : Hub
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatHub(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager; 
    }

    public async Task SendMessage(string message)
    {
        var userId = Context.User.GetId();

        if (userId != null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                string userName = $"{user.FirstName} {user.LastName}";
                await Clients.All.SendAsync("ReceiveMessage", userName, message);
            }
        }
    }
}
