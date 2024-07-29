namespace OnlineHotelRoomrentingSystem.Tests;

using OnlineHotelRoomrentingSystem.Data;
using OnlineHotelRoomrentingSystem.Models;
public static class DataBaseSeeder
{
    public static ApplicationUser AgentUser;
    public static ApplicationUser RenterUser;
    public static Agent Agent;
    public static Hotel AgentHotel;
    public static Room AgentRoom;
    public static OnlineHotelRoomrentingSystem.Models.Category FirstCategory;
    public static Hotel UnapprovedHotel;
    public static RoomType FirstRoomType;

    public static void SeedDatabase(HotelRoomBookingDb dbContext)
    {
        var agentUserId = Guid.Parse("93e72464-4832-4483-952f-41d221ab1091");
        var renterUserId = Guid.Parse("f8b8ce23-d7eb-4e7a-a72e-7a9f178f95f3");

        AgentUser = new ApplicationUser
        {
            Id = agentUserId,
            UserName = "Pesho",
            NormalizedUserName = "PESHO",
            Email = "pesho@agents.com",
            NormalizedEmail = "PESHO@AGENTS.COM",
            EmailConfirmed = true,
            PasswordHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92",
            ConcurrencyStamp = "caf271d7-0ba7-4ab1-8d8d-6d0e3711c27d",
            SecurityStamp = "ca32c787-626e-4234-a4e4-8c94d85a2b1c",
            TwoFactorEnabled = false,
            FirstName = "Pesho",
            LastName = "Petrov"
        };

        RenterUser = new ApplicationUser
        {
            Id = renterUserId,
            UserName = "Ivan",
            NormalizedUserName = "IVAN",
            Email = "ivan@renters.com",
            NormalizedEmail = "IVAN@RENTERS.COM",
            EmailConfirmed = true,
            PasswordHash = "2y04$9Hkk2kb4fH72A0DdqFstvOPExe1W18n46bnY4tlCm4aOZlmSHJrzK",
            ConcurrencyStamp = "caf271d7-0bw7-4ab1-8d8d-6d0e3711c27d",
            SecurityStamp = "ca32c787-626e-4274-a4e4-8c94d85a2b1c",
            TwoFactorEnabled = false,
            FirstName = "Ivan",
            LastName = "Petrov"
        };

        Agent = new Agent
        {
            Id = Guid.Parse("9639E0FA-A591-480C-AE67-1333545EE981"),
            FirstName = "Ivan",
            MiddleName = "Stoqnov",
            LastName = "Petrov",
            PhoneNumber = "+359888888888",
            DateOfBirth = new DateTime(1985, 5, 15),
            EGN = "8505151234",
            User = AgentUser,
            UserId = agentUserId
        };

        AgentHotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Luxury Palace Hotel",
            Address = "123 Main Street",
            City = "Metropolis",
            Country = "United States",
            Email = "info@luxurypalace.com",
            PhoneNumber = "+35965896547",
            Description = "Luxury Palace Hotel offers the epitome of comfort and elegance with its lavish accommodations and world-class amenities.",
            NumberOfRooms = 200,
            MaximumCapacity = 500,
            HotelManagerId = Agent.Id,
            HotelManager = Agent,
            Image = "/images/HotelOne.jpg",
            Category = new OnlineHotelRoomrentingSystem.Models.Category { Id = 10, Name = "Luxury" },
            Rooms = new List<Room> {},
            Reviews = new List<OnlineHotelRoomrentingSystem.Models.Review> {}
        };

        UnapprovedHotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Unapproved Hotel",
            Address = "456 Unapproved Street",
            City = "Unapproved City",
            Country = "Unapproved Country",
            Email = "unapproved@hotel.com",
            PhoneNumber = "+987654321",
            Description = "This is an unapproved hotel.",
            NumberOfRooms = 30,
            MaximumCapacity = 60,
            HotelManagerId = Agent.Id,
            HotelManager = Agent,
            Image = "/images/UnapprovedHotel.jpg",
            Category = new OnlineHotelRoomrentingSystem.Models.Category { Id = 12, Name = "Economy" },
            IsApproved = false
        };

        AgentRoom = new Room
        {
            Id = Guid.Parse("CFD7C099-1893-4922-AA5F-04C4FB84F955"),
            RoomId = 1,
            PricePerNight = 150.00m,
            IsAvaible = true,
            Image = "/rooms/room1.jpg",
            HotelId = AgentHotel.Id,
            RoomNumber = 11,
            AgentId = Agent.Id,
            RenterId = RenterUser.Id,
        };

        FirstCategory = new OnlineHotelRoomrentingSystem.Models.Category
        {
            Id = 8,
            Name = "Luxury"
        };

        FirstRoomType = new RoomType()
        {
            Id = 3,
            Name = "Single"
        };

        var room = new Room
        {
            Id = Guid.NewGuid(),
            HotelId = AgentHotel.Id,
            RoomNumber = 101,
            RoomType = FirstRoomType,
            PricePerNight = 150.00M,
            IsAvaible = true
        };

        var review = new OnlineHotelRoomrentingSystem.Models.Review
        {
            Id = 3,
            HotelId = AgentHotel.Id,
            RoomId = room.Id,
            Content = "Great stay!",
            Rating = 5,
            ReviewDate = DateTime.Now
        };

        dbContext.Users.Add(AgentUser);
        dbContext.Users.Add(RenterUser);
        dbContext.Agents.Add(Agent);
        dbContext.Hotels.Add(AgentHotel);
        dbContext.Rooms.Add(AgentRoom);
        dbContext.Categories.Add(FirstCategory);
        dbContext.Hotels.Add(UnapprovedHotel);
        dbContext.RoomType.Add(FirstRoomType);
        dbContext.Rooms.Add(room);
        dbContext.Reviews.Add(review);


        dbContext.SaveChanges();
    }
}