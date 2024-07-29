namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Review;

public class RoomReviewViewModel
{
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime ReviewDate { get; set; }
    public string RoomId { get; set; } = string.Empty;
}
