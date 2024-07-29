namespace OnlineHotelRoomrentingSystem.Models;

using Microsoft.AspNetCore.Identity;
using static OnlineHotelRoomrentingSystem.Commons.EntityValidationConstants.User;
using System.ComponentModel.DataAnnotations;

public class ApplicationUser : IdentityUser<Guid>
{

    public ApplicationUser()
    {
        Id = Guid.NewGuid();

        RentedRooms = new HashSet<Room>();
        Reviews = new HashSet<Review>();
    }

    [Required]
    [MaxLength(UserFirstNameMaxLength)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(UserLastNameMaxLength)]
    public string LastName { get; set; } = string.Empty;
    public Agent? Agent { get; set; }

    public virtual ICollection<Room> RentedRooms { get; set; }
    public virtual ICollection<Review> Reviews { get; set; }
}
