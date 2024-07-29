namespace OnilineHotelRoomBookingSystem.Services.Data.Contacts;

public interface ICategoryService
{
    Task<IEnumerable<string>> AllCategoriesNamesAsync();
}
