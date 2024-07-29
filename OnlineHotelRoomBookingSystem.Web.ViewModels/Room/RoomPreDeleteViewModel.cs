namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Room;

public class RoomPreDeleteViewModel
{
    public string imageFile {  get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string HotelId { get; set;} = string.Empty;
}
