namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Room;
public class RoomServiceModel
{
    public string? Id { get; set; }
    public string HotelId { get; set; } = string.Empty;
    public bool IsAvaible { get; set; }
    public string? imageFile { get; set; }
    public decimal PricePerNight { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
}

