namespace OnlineHotelRoomrentingSystem.Models;

using System.ComponentModel.DataAnnotations;
using static OnlineHotelRoomrentingSystem.Commons.EntityValidationConstants.Categories;

public class Category
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Category"/> class.
    /// Sets default values for Hotels.
    /// </summary>
    public Category()
    {
        Hotels = new HashSet<Hotel>();
    }

    /// <summary>
    /// This is unique identifier for the category
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// This is the name of the category
    /// </summary>

    [Required]
    [MaxLength(CategoryNameMaxLength)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// This is collection of all hotels that have category
    /// </summary>
    public virtual ICollection<Hotel> Hotels { get; set; }
}