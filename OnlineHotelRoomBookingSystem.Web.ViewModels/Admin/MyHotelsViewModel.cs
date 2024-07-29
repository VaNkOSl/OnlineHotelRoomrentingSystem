namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Admin;

using OnlineHotelRoomBookingSystem.Web.ViewModels.Hotel;

public class MyHotelsViewModel
{
    public IEnumerable<HotelServiceModel> AddedHotels { get; set; } = new List<HotelServiceModel>();
}
