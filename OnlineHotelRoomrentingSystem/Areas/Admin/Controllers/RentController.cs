namespace OnlineHotelRoomrentingSystem.Areas.Admin.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Admin;
using static OnlineHotelRoomrentingSystem.Commons.GeneralApplicationConstants;

public class RentController :  AdminBaseController
{
    private readonly IRentService rentService;
    private readonly IMemoryCache memoryCache;

    public RentController(IRentService _rentService, IMemoryCache _memoryCache)
    {
        rentService = _rentService;
        memoryCache = _memoryCache;
    }

    [Route("Rent/All")]
    [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Client, NoStore = false)]
    public async Task<IActionResult> All()
    {
        IEnumerable<RentServiceModel> allRents =
            this.memoryCache.Get<IEnumerable<RentServiceModel>>(RentsCacheKey);
        if (allRents == null)
        {
            allRents = await this.rentService.AllRentsAsync();

            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(RentsCacheDurationMinutes));

            this.memoryCache.Set(UsersCacheKey, allRents, cacheOptions);
        }

        return this.View(allRents);
    }
}
