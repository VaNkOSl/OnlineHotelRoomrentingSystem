namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Hotel;

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using static OnlineHotelRoomrentingSystem.Commons.EntityValidationConstants.Hotels;
public class HotelFormModel
{
    public HotelFormModel()
    {
        Categories = new HashSet<HotelCategoryServiceModel>();
    }

    [Required]
    [StringLength(HotelNameMaxLength, MinimumLength = HotelNameMinLength)]
    [Display(Name = "Enter Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(HotelAdressMaxLenght, MinimumLength = HotelAdressMinLenght)]
    [Display(Name = "Enter Adress")]
    public string Address { get; set; } = string.Empty;

    [Required]
    [StringLength(HotelCityMaxLenght, MinimumLength = HotelCityMinLenght)]
    [Display(Name = "Enter City")]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(HotelCountryMaxLenght, MinimumLength = HotelCountryMinLenght)]
    [Display(Name = "Enter Country")]
    public string Country { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Enter Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(HotelPhoneNumberMaxLength, MinimumLength = HotelPhoneNumberMinLength)]
    [Display(Name = "Enter Phone Number")]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(HotelDescriptionMaxLength, MinimumLength = HotelDescriptionMinLength)]
    [Display(Name = "Enter Description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Impload Photo of the Hotel")]
    public string? imageFile { get; set; }

    [Required]
    [Display(Name = "Enter number of rooms")]
    public int NumberOfRooms { get; set; }

    [Required]
    [Display(Name = "Enter maximum capacity of the hotel")]
    public int MaximumCapacity { get; set; }

    [Required]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    public IEnumerable<HotelCategoryServiceModel> Categories { get; set; }
}
