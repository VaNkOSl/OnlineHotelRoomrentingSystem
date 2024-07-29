namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Hotel;

public class HotelServiceModel
{
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? imageFile { get; set; }
}
