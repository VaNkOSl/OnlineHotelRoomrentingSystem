namespace OnilineHotelRoomBookingSystem.Services.Data.Contacts;

using Microsoft.AspNetCore.Http;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Home;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Hotel;
using OnlineHotelRoomrentingSystem.Models;

public interface IHotelService
{
    Task<IEnumerable<HotelIndexServiceModel>> LastThreeHotelsAsync();
    Task<bool> CategoryExistAsync(int categoryId);
    Task<IEnumerable<HotelCategoryServiceModel>> AllCategoriesAsync();
    Task<bool> HotelWithTheSameEmailExistAsync(string hotelEmail);
    Task<bool> HotelWithTheSamePhoneNumberExist(string hotelPhoneNumber);
    Task<HotelQueryServiceModel> AllHotelsAsync(AllHotelsQueryModel model);
    Task<IEnumerable<HotelServiceModel>> AllHotelsByAgentIdAsync(string agentId);
    Task<HotelDetailsViewModel> HotelDetailsByIdAsync(string hotelId);
    Task<bool> HotelExistsAsync(string hotelId);
    Task<HotelPreDeleteDetailsViewModel> GetHotelForDeleteByIdAsync(string hotelId);
    Task DeleteHotelByIdAsync(string hotelId);
    Task<bool> HasHotelManagerWithIdAsync(string hotelId, string currentId);
    Task<HotelFormModel> EditHotelByIdAsync(string hotelId);
    Task EditHotelByFormModel(HotelFormModel model, string hotelId, IFormFile imageFile);
    Task<string> CreateHotelAsync(HotelFormModel model, string hotelManagerId, IFormFile imageFile);
    Task<Hotel> GetHotelByIdAsync(string hotelId);
    Task ApproveHotelAsync(Guid hotelId);
    Task<IEnumerable<HotelServiceModel>> GetUnApprovedAsync();
    Task<string> GetHotelNameByIdAsync(string hotelId);
    Task<bool> HasRoomsAsync(string hotelId);
    Task DeleteRoomsByHotelIdAsync(string hotelId);
    Task<bool> HotelHasReviewsAsync(string hotelId); 
}
