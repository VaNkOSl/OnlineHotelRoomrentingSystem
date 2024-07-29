using System.ComponentModel.DataAnnotations;

namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Agent;

public class AgentServiceModel
{
    public string Id { get; set; } = string.Empty;
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
