namespace OnilineHotelRoomBookingSystem.Services.Data.Contacts;

using OnlineHotelRoomBookingSystem.Web.ViewModels.Admin;
public interface IRentService
{
    Task<IEnumerable<RentServiceModel>> AllRentsAsync();
}
