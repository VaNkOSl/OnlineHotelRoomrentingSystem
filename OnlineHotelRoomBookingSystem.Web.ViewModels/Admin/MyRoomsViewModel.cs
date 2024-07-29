namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Admin;

using OnlineHotelRoomBookingSystem.Web.ViewModels.Room;
public class MyRoomsViewModel
{
    public IEnumerable<RoomServiceModel> AddedRooms { get; set; } = new List<RoomServiceModel>();
}
