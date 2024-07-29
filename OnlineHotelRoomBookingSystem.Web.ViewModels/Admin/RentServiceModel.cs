namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Admin;

public class RentServiceModel
{
    public string RoomTitle { get; set; } = string.Empty;
    public string? imageFile { get; set; }
    public string AgentFullName { get; set; } = string.Empty;
    public string AgentEmail {  get; set; } = string.Empty;
    public string RenterFullName {  get; set; } = string.Empty;
    public string RenterEmail { get; set;} = string.Empty;
}
