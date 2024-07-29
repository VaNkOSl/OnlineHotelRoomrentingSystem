namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Room;

using System.ComponentModel.DataAnnotations;
using static OnlineHotelRoomrentingSystem.Commons.EntityValidationConstants.Rooms;
public class RoomFormModel
{
    public RoomFormModel()
    {
        Hotels = new HashSet<HotelRoomViewModel>();
        RoomTypes = new HashSet<RoomTypeServiceModel>();
    }

    [Required]
    [Range(typeof(decimal), PricePerNighthMinValue, PricePerNighthMaxValue)]
    [Display(Name = "Enter price per night")]
    public decimal PricePerNight { get; set; }

    [Display(Name = "Select an image")]
    public string? imageFile { get; set; }

    [Required]
    [StringLength(RoomDescriptionMaxLength,MinimumLength = RoomDescriptionMinLength)]
    [Display(Name = "Enter description of the room")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Select hotel of the room")]
    public string HotelId { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Room Type")]
    public int RoomTypeId { get; set; }

    [Required]
    [Display(Name = "Select a room number for the hotel")]
    public int RoomNumber { get; set; }

    public IEnumerable<RoomTypeServiceModel> RoomTypes { get; set; }
    public IEnumerable<HotelRoomViewModel?> Hotels { get; set; }
}
