namespace OnlineHotelRoomrentingSystem.Commons;

public static class EntityValidationConstants
{
    public static class Hotels
    {
        public const int HotelNameMinLength = 4;
        public const int HotelNameMaxLength = 40;

        public const int HotelAdressMinLenght = 10;
        public const int HotelAdressMaxLenght = 50;

        public const int HotelCityMinLenght = 3;
        public const int HotelCityMaxLenght = 20;

        public const int HotelCountryMinLenght = 5;
        public const int HotelCountryMaxLenght = 35;

        public const int HotelPhoneNumberMinLength = 7;
        public const int HotelPhoneNumberMaxLength = 14;

        public const int HotelDescriptionMinLength = 30;
        public const int HotelDescriptionMaxLength = 500;
    }

    public static class User
    {
        public const int UserFirstNameMinLength = 2;
        public const int UserFirstNameMaxLength = 15;

        public const int UserLastNameMinLength = 3;
        public const int UserLastNameMaxLength = 15;
    }

    public static class Rooms
    {
        public const int RoomNameMinLength = 3;
        public const int RoomNameMaxLength = 30;

        public const string PricePerNighthMinValue = "0";
        public const string PricePerNighthMaxValue = "2000";

        public const int RoomDescriptionMinLength = 15;
        public const int RoomDescriptionMaxLength = 300;
    }

    public static class ApplicationUsers
    {
        public const int FirstNameMinLength = 3;
        public const int FirstNameMaxLength = 15;

        public const int LastNameMinLength = 5;
        public const int LastNameMaxLength = 15;
    }

    public static class Reviews
    {
        public const int ReviewContentMinLength = 10;
        public const int ReviewContentMaxLength = 300;
    }

    public static class Agent
    {
        public const int AgentFirstNameMinLength = 3;
        public const int AgentFirstNameMaxLength = 15;

        public const int AgentMiddleNameMinLength = 5;
        public const int AgentMiddleNameMaxLength = 15;

        public const int AgentLastNameMinLength = 5;
        public const int AgentLastNameMaxLength = 15;

        public const int AgentPhoneNumberMinLength = 7;
        public const int AgentPhoneNumberMaxLength = 15;

        public const int AgentEGNTMinLenngth = 10;
        public const int AgentEGNTMaxLenngth = 10;
    }

    public static class Categories
    {
        public const int CategoryNameMinLength = 3;
        public const int CategoryNameMaxLength = 10;
    }
}
