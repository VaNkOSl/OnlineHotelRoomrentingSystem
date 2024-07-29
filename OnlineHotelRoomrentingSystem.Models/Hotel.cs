namespace OnlineHotelRoomrentingSystem.Models;

using static OnlineHotelRoomrentingSystem.Commons.EntityValidationConstants.Hotels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a hotel with various properties such as name, address, city, country, email, phone number,
/// description, image, number of rooms, maximum capacity, category, manager, rooms, and reviews.
/// </summary>

public class Hotel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Hotel"/> class.
    /// Sets default values for Id, Rooms, and Reviews.
    /// </summary>
    public Hotel()
    {
        Id = Guid.NewGuid();
        Rooms = new HashSet<Room>();
        Reviews = new HashSet<Review>();
    }

    /// <summary>
    /// This is unique identifier for the hotel.
    /// </summary>

    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the hotel
    /// </summary>

    [Required]
    [MaxLength(HotelNameMaxLength)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Adress of the hotel
    /// </summary>

    [Required]
    [MaxLength(HotelAdressMaxLenght)]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// City ​​where the hotel is located
    /// </summary>

    [Required]
    [MaxLength(HotelCityMaxLenght)]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// The country where the hotel is located
    /// </summary>

    [Required]
    [MaxLength(HotelCountryMaxLenght)]
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the hotel
    /// </summary>

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// Phone number of the hotel
    /// </summary>

    [Required]
    [MaxLength(HotelPhoneNumberMaxLength)]
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Description of the hotel
    /// </summary>

    [Required]
    [MaxLength(HotelDescriptionMaxLength)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Image of the hotel
    /// </summary>

    public string? Image { get; set; }

    /// <summary>
    /// Number of rooms of the hotel
    /// </summary>

    [Required]
    public int NumberOfRooms { get; set; }

    /// <summary>
    /// Maximum capacity of the hotel
    /// </summary>

    [Required]
    public int MaximumCapacity { get; set; }


    /// <summary>
    /// This is property to show the user in wich year is Created hotel
    /// </summary>
    public DateTime? CreatedOn { get; set; }

    /// <summary>
    /// Representing the category ID of the hotel
    /// </summary>

    [Required]
    public int CategoryId { get; set; }

    /// <summary>
    /// Representing the category of the hotel
    /// </summary>
    public bool? IsApproved { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public virtual Category Category { get; set; } = null!;

    /// <summary>
    /// Representing the ID of the hotel manager.
    /// </summary>
    public Guid HotelManagerId { get; set; }

    /// <summary>
    /// Representing the manager of the hotel.
    /// </summary>

    [ForeignKey(nameof(HotelManagerId))]
    public virtual Agent HotelManager { get; set; } = null!;

    /// <summary>
    /// All the rooms that the hotel has
    /// </summary>

    public virtual ICollection<Room> Rooms { get; set; }

    /// <summary>
    /// All reviews that the hotel has
    /// </summary>
    public virtual ICollection<Review> Reviews { get; set; }

}