namespace OnlineHotelRoomrentingSystem.Models;

using static OnlineHotelRoomrentingSystem.Commons.EntityValidationConstants.Reviews;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represent the review of the hotel
/// </summary>
public class Review
{
    /// <summary>
    /// This is unique identifier of the review
    /// </summary>

    [Key]
    public int Id { get; set; }

    /// <summary>
    /// This is content of the review
    /// </summary>

    [Required]
    [MaxLength(ReviewContentMaxLength)]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// This is rating of the review minimum rating is one maximum is five
    /// </summary>

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    /// <summary>
    /// Represents the date of the review
    /// </summary>

    public DateTime ReviewDate { get; set; }

    public Guid? UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; } = null!;

    [Required]
    public Guid? RoomId { get; set; }

    [ForeignKey(nameof(RoomId))]
    public virtual Room Room { get; set; } = null!;

    /// <summary>
    /// Represents the Hotel Id of the review
    /// </summary>

    [Required]
    public Guid? HotelId { get; set; }

    /// <summary>
    /// Represents Hotel of the review
    /// </summary>

    [ForeignKey(nameof(HotelId))]
    public virtual Hotel Hotel { get; set; } = null!;
}