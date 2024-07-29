namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Room;

using OnlineHotelRoomBookingSystem.Web.ViewModels.Agent;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Review;

public class RoomDetailsViewModel : RoomServiceModel
{
    public RoomDetailsViewModel()
    {
        Reviews = new List<RoomReviewViewModel>();
    }

    public AgentServiceModel Agent { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public List<RoomReviewViewModel> Reviews { get; set; }
}
