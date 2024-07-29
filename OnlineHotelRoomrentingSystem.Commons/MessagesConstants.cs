using static OnlineHotelRoomrentingSystem.Commons.EntityValidationConstants;
using System.Security;

namespace OnlineHotelRoomrentingSystem.Commons;

public static class MessagesConstants
{
    public const string UserDoesNotExist = "Error checking agent status.";
    public const string UserWithSameEgnExist = "User with the same EGN already exists";
    public const string UserWithTheSamePhoneNumberExists = "User with the same phone number already exists";

    public const string SuccessfullyAddedReview = "You have successfully added a review";
    public const string SuccessfullyDeletedHotel = "You have successfully deleted the hotel";
    public const string SuccessfullyDeleteRoom = "You have successfully deleted room in hotel {0}";
    public const string SuccessfullyCreatedHotel = "You have successfully added a hotel";
    public const string SuccessfullyCreateRoom = "You have successfully added room in hotel {0}";
    public const string SuccessfullyRentedRoom = "You have successfully rent room in hotel {0}";
    public const string SuccessfullyLeavedRoom = "You have successfully rent room in hotel {0}";


    public const string SuccessfullyBecomeAHotelManager = "You have successfully become a hotel manager";

    public const string GeneralErrors = "Unexpected error occurred! Please try again later or contact administrator";
    public const string HotelIdDoesNotExist = "Hotel with the provide id does not exists!";
    public const string RoomIdDoesNotExists = "Room with the provide id does not exists!";
    public const string HotelFormModelIsNotFound = "Sorry cannot find hotel, please try again later";
    public const string RoomFormModelIsNotFound = "Sorry cannot find room, please try again later";
    public const string CategoryDoesNotExists = "Category doesn't exist";
    public const string NoPhotoSelected = "Please select an image file.";
    public const string SameHotelEmail = "There is already a hotel with this email address";
    public const string SameHotelPhoneNumbers = "There is already a hotel with the same phone number";
    public const string ErrorWhileCreatedHotel = "Unexpected error occurred while trying to add your new hotel! Please try again later or contact administrator!";
    public const string ErrorWhileCreateRoom = "Unexpected error occurred while trying to add your new room! Please try again later or contact administrator!";
    public const string HotelDoesNotExists = "The hotel could not be found. Please try again later";
    public const string NotAuthorizedHotelManager = "You are not authorized to access this resource because you are neither the hotel manager nor an administrator.";
    public const string AgenIdDoesNotExists = "Failed to retrieve the agent ID. Please try again or contact support.";
    public const string AgenDoesNotExists = "You must be an agent to access this resource!";
    public const string RoomTypeDoesNotExists = "The selected room type does not exist. Please select a valid room type.";
    public const string RoomWithTheSameNumberExists = "A room with the same number already exists in this hotel.";
    public const string CannonEditRoomNow = "You cannot edit the room at the moment.Please try again later, if the error persists please contact the administrator";
    public const string UserDoesNotHaveRoom = "You don't have a room";
    public const string CannonDeleteHotel = "An unexpected error occurred, you cannot delete the hotel right now, please try again later";
    public const string RoomIsAlreadyRented = "Selected room is already rented by another user! Please select another room.";
    public const string AgenCannonRentRoom = "Agents can't rent rooms. Please register as a user!";
    public const string StillNonAppoveForHotelManager = "Your account has not yet been approved as a hotel manager. Please wait for admin approval.";
    public const string TheAgentIsNotTheOwnerOfTheHotel = "You do not have permission to add a room to this hotel.";
}
