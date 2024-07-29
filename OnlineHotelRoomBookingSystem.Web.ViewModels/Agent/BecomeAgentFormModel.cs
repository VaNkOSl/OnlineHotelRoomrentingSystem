namespace OnlineHotelRoomBookingSystem.Web.ViewModels.Agent;

using static OnlineHotelRoomrentingSystem.Commons.EntityValidationConstants.Agent;
using System.ComponentModel.DataAnnotations;
public class BecomeAgentFormModel
{
    [Required]
    [StringLength(AgentFirstNameMaxLength, MinimumLength = AgentFirstNameMinLength)]
    [Display(Name = "Enter First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(AgentMiddleNameMaxLength, MinimumLength = AgentMiddleNameMinLength)]
    [Display(Name = "Enter Middle Name")]
    public string MiddleName { get; set; } = string.Empty;

    [Required]
    [StringLength(AgentLastNameMaxLength, MinimumLength = AgentLastNameMinLength)]
    [Display(Name = "Enter Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [StringLength(AgentEGNTMaxLenngth, MinimumLength = AgentEGNTMaxLenngth)]
   // [EgnValidator]
    public string EGN { get; set; } = string.Empty;

    [Required]
    [StringLength(AgentPhoneNumberMaxLength, MinimumLength = AgentPhoneNumberMinLength)]
    [Display(Name = "Enter Phone Number")]
    [RegularExpression(@"^\d{7,14}$", ErrorMessage = "Phone number must be between 7 and 14 digits.")]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
}
