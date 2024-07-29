namespace OnilineHotelRoomBookingSystem.Services.Data.Contacts;

using Microsoft.AspNetCore.Http;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Room;
public interface IRoomService
{
    Task<bool> RoomTypeExistAsync(int roomType);
    Task<IEnumerable<RoomTypeServiceModel>> AllRoomTypesAsync();
    Task<string> CreateRoomAsync(RoomFormModel model, string agentId, IFormFile imageFile);
    Task<IEnumerable<RoomServiceModel>> AllRoomsByAgentIdAsync(string agentId);
    Task<bool> RoomNumberAlreadyExistInTheHotelAsync(int roomNumber,string hotelId);
    Task<IEnumerable<HotelRoomViewModel>> GetAllHotelsAsync();
    Task<IEnumerable<RoomServiceModel>> GetAllRoomsAsync();
    Task<List<RoomServiceModel>> GetRoomsByHotelIdAsync(string hotelId);
    Task<RoomDetailsViewModel> RoomDetailsByIdAsync(string roomId);
    Task<IEnumerable<RoomServiceModel>> AllRoomsByUsersIdAsync(string userId);
    Task<RoomFormModel> EditRoomByIdAsync(string roomId);
    Task EditRoomByFormModel(RoomFormModel model,string roomId, IFormFile imageFile);
    Task<bool> HasRoomAgenWithIdAsync(string agentId, string roomId);
    Task DeleteRoomByIdAsync(string roomId);
    Task<RoomPreDeleteViewModel> DeteleRoomFormModelByIdAsync(string roomId);
    Task<bool> IsRentedAsync(string roomId);
    Task RentAsync(string roomId, string userId, string hotelId);
    Task LeaveAsync(string userId, string id, string hotelId);
    Task<bool> IsRentedByUserWithIdAsync(string userId, string roomId);
    Task<RoomServiceModel> GetRoomByIdAsync(string roomId);
    Task<bool> RoomExistsAsync(string roomId);
    Task<bool> RoomHasReviewAsync(string roomId);
}
