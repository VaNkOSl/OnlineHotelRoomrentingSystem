namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Hotel;

using OnlineHotelRoomBookingSystem.Web.ViewModels.Agent;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Review;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Room;

public class HotelDetailsViewModel : HotelServiceModel
{

    public HotelDetailsViewModel()
    {
        Reviews = new List<HotelReviewViewModel>();
        Rooms = new List<RoomServiceModel>();
    }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; }  = string.Empty;
    public string Description { get; set; } = string.Empty;
    public AgentServiceModel HotelManager { get; set; } = null!;
    public string CategoryName {  get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int NumberOfRooms { get; set; }
    public int MaximumCapacity { get; set; }

    public List<HotelReviewViewModel> Reviews {  get; set; }
    public List<RoomServiceModel> Rooms { get; set; }
}
