using System.ComponentModel.DataAnnotations;

namespace OnlineHotelRoomrentingSystem.Models;
using static OnlineHotelRoomrentingSystem.Commons.EntityValidationConstants.Rooms;

public class RoomType
{
    public RoomType()
    {
        Rooms = new HashSet<Room>();
    }

    /// <summary>
    /// This is unique identifier for the room type
    /// </summary>
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(RoomNameMaxLength)]
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<Room> Rooms { get; set; }
}
