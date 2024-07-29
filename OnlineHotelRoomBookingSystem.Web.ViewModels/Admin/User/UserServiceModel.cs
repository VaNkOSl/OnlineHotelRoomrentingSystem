namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Admin.User;

using System.ComponentModel.DataAnnotations;
public class UserServiceModel
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    public bool IsAgent { get; set; }
}

