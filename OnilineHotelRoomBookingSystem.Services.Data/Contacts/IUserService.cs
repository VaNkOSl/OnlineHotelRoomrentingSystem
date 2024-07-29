using OnlineHotelRoomBookingSystem.Web.ViewModels.Admin.User;

namespace OnilineHotelRoomBookingSystem.Services.Data.Contacts;

public interface IUserService
{
    Task<string> GetUserFullNameAsync(string userId);
    Task<IEnumerable<UserServiceModel>> AllUsersAsync();
}
