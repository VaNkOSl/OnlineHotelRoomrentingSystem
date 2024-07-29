namespace OnlineHotelRoomBookingSystem.Web.Infrastructure.Extensions;

using OnlineHotelRoomBookingSystem.Web.ViewModels.Category.NewFolder.Contacts;
public static class ViewModelExtensions
{
    public static string GetUrlInformation(this ICategoryDetailsModel model)
    {
        return model.Name.Replace(" ", "-");
    }
}
