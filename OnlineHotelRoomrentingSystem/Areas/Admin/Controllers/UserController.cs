namespace OnlineHotelRoomrentingSystem.Areas.Admin.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Admin.User;
using static OnlineHotelRoomrentingSystem.Commons.GeneralApplicationConstants;

public class UserController : AdminBaseController
{
    private readonly IUserService userService;
    private readonly IMemoryCache memoryCache;

    public UserController(IUserService _userService,IMemoryCache _memoryCache)
    {
        userService = _userService;
        memoryCache = _memoryCache;
    }

    [Route("User/All")]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client, NoStore = false)]
    public async Task<IActionResult> All()
    {
        IEnumerable<UserServiceModel> users = this.memoryCache.Get<IEnumerable<UserServiceModel>>(UsersCacheKey);
        if (users == null)
        {
            users = await this.userService.AllUsersAsync();

            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(UsersCacheDurationMinutes));

            this.memoryCache.Set(UsersCacheKey, users, cacheOptions);
        }

        return View(users);
    }
}
