namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Hotel;

using OnlineHotelRoomBookingSystem.Web.ViewModels.Hotel.Enum;
using System.ComponentModel.DataAnnotations;
using static OnlineHotelRoomrentingSystem.Commons.GeneralApplicationConstants;

public class AllHotelsQueryModel
{
    public AllHotelsQueryModel()
    {
        CurrentPage = DefaultPage;
        HotelsPerPage = EntitiesPerPage;

        Categories = new HashSet<string>();
        Hotels = new HashSet<HotelServiceModel>();
    }

    public string? Category { get; set; }

    [Display(Name = "Search by word")]
    public string? SearchinString { get; set; }

    [Display(Name = "Sort Hotel By")]
    public HotelSorting HotelSorting { get; set; }

    public int CurrentPage { get; set; }

    [Display(Name = "Show Houses On Page")]
    public int HotelsPerPage { get; set; }

    public int TotalHotels { get; set; }

    public IEnumerable<string> Categories { get; set; }
    public IEnumerable<HotelServiceModel> Hotels { get; set; }
}
