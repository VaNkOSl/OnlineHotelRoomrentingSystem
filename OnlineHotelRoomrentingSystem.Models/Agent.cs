namespace OnlineHotelRoomrentingSystem.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using static OnlineHotelRoomrentingSystem.Commons.EntityValidationConstants.Agent;

/// <summary>
/// Represents an agent with personal and contact information.
/// </summary>
public class Agent
{
    public Agent()
    {
        Id = Guid.NewGuid();
        OwnedRooms = new HashSet<Room>();
        OwnedHotels = new HashSet<Hotel>();
    }

    /// <summary>
    /// That represents the unique identifier of the agent
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// That represent the first name of the agent 
    /// </summary>
    [Required]
    [MaxLength(AgentFirstNameMaxLength)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// That represents the middle name or initial of the agent.
    /// </summary>

    [Required]
    [MaxLength(AgentMiddleNameMaxLength)]
    public string MiddleName { get; set; } = string.Empty;

    /// <summary>
    /// That represents the last name of the agent
    /// </summary>
    /// 
    [Required]
    [MaxLength(AgentLastNameMaxLength)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// That represents the phone number of the agent
    /// </summary>
    [Required]
    [MaxLength(AgentPhoneNumberMaxLength)]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// That represents the date of birth of the agent
    /// </summary>

    [Required]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// That represents the unique personal identification number of the agent
    /// </summary>

    [Required]
    [MaxLength(AgentEGNTMaxLenngth)]
   // [EgnValidator]
    public string EGN { get; set; } = string.Empty;
    public bool IsApproved { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; }


    /// <summary>
    /// This is a collection of all the rooms that agents have
    /// </summary>
    public virtual ICollection<Room> OwnedRooms { get; set; }

    /// <summary>
    /// This is a collection of all the hotels that the agents have
    /// </summary>
    public virtual ICollection<Hotel> OwnedHotels { get; set; }
}
