namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Review;

public class HotelReviewViewModel
{
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime ReviewDate { get; set; }
    public string HotelId { get; set; } = string.Empty;
}
