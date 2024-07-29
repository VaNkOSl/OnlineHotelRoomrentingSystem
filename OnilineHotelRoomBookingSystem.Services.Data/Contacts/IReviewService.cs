namespace OnilineHotelRoomBookingSystem.Services.Data.Contacts;

using OnlineHotelRoomBookingSystem.Web.ViewModels.Review;
public interface IReviewService
{
    Task<string> CreateHotelReviewAsync(HotelReviewViewModel model,string userId);
    Task<string> CreateRoomReviewAsync(RoomReviewViewModel model,string userId);
    Task DeleteReviewsByHotelIdAsync(string hotelId);
    Task DeleteReviewsByRoomIdAsync(string roomId);
}
