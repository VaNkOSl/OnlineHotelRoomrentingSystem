namespace OnlineHotelRoomrentingSystem.Models;

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static OnlineHotelRoomrentingSystem.Commons.EntityValidationConstants.Rooms;

public class Room
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Room"/> class.
    /// Sets default values for Id.
    /// </summary>
    public Room()
    {
        Id = Guid.NewGuid();
        Reviews = new HashSet<Review>();
    }

    /// <summary>
    /// This is unique identifier for the room
    /// </summary>

    [Key]
    public Guid Id { get; set; }

    [Required]
    public int RoomId { get; set; }

    [Required]
    [ForeignKey(nameof(RoomId))]
    public virtual RoomType RoomType { get; set; }

    /// <summary>
    /// This is price per night of the room
    /// </summary>

    [Required]
    public decimal PricePerNight { get; set; }

    /// <summary>
    /// This is to show if the room is available or occupied
    /// </summary>

    [Required]
    public bool IsAvaible { get; set; }

    /// <summary>
    /// Image of the room
    /// </summary>
    public string? Image { get; set; }

    [Required]
    [MaxLength(RoomDescriptionMaxLength)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int RoomNumber { get; set; }

    /// <summary>
    /// Representing the Hotel Id of the room
    /// </summary>
    /// 
    [Required]
    public Guid HotelId { get; set; }

    /// <summary>
    /// Shows which hotel a given room is located in
    /// </summary>

    [ForeignKey(nameof(HotelId))]
    public virtual Hotel Hotel { get; set; } = null!;

    /// <summary>
    /// Representing the ID of the hotel manager.
    /// </summary>

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public Guid? RenterId { get; set; }

    [ForeignKey(nameof(RenterId))]
    public virtual ApplicationUser? Renter { get; set; }

    /// <summary>
    ///  Representing the Agent Id of the room
    /// </summary>
    public Guid AgentId { get; set; }

    /// <summary>
    ///   Representing the Agent of the room
    /// </summary>

    [ForeignKey(nameof(AgentId))]
    public virtual Agent Agent { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; }
}