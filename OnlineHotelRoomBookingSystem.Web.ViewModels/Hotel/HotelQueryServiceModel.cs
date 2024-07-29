namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Hotel;

public class HotelQueryServiceModel
{
    public HotelQueryServiceModel()
    {
        Hotels = new HashSet<HotelServiceModel>();
    }

    public int TotalHotelsCount { get; set; }

    public IEnumerable<HotelServiceModel> Hotels { get; set; }
}
