using Microsoft.AspNetCore.Identity;
using OnlineHotelRoomrentingSystem.Models;

namespace OnlineHotelRoomrentingSystem.Data.Data.SeedDB;

internal class SeedData
{
    public Agent FirstAgent {  get; set; }
    public Agent SecondAgent { get; set; }
    public IdentityUser FirstUser { get; set; }
    public IdentityUser SecondUser { get; set;}
    public Hotel FirstHotel { get; set; }
    public Hotel SecondHotel { get; set; }
    public Hotel ThirdHotel { get; set; }
    public Category FirstCategory { get; set; }
    public Category SecondCategory { get; set; }
    public Category ThirdCategory { get; set; }
    public Category FourthCategory { get; set; }
    public Category FiveCategory { get; set; }
    public Category SixCategory { get; set; }
    public RoomType FirstRoomType { get; set; }
    public RoomType SecondRoomType { get; set; }
    public Review FirstReview { get; set; }
    public Review SecondReview { get; set; }
    public Room FirstRoom { get; set; }
    public Room SecondRoom { get; set; }

    public SeedData()
    {
        SeedUsers();
        SeedAgents();
        SeedCategories();
        SeedHotels();
        SeedReviews();
        SeedRoomsTypes();
        SeedRooms();
    }

    private void SeedUsers()
    {
        var hasher = new PasswordHasher<IdentityUser>();

        FirstUser = new IdentityUser
        {
            Id = "dea12856-c198-4129-b3f3-b893d8395082",
            UserName = "user@gmail.com",
            NormalizedUserName = "user@gmail.com",
            Email = "user@gmail.com",
            NormalizedEmail = "user@gmail.com"
        };

        FirstUser.PasswordHash =
            hasher.HashPassword(FirstUser, "user123");

        SecondUser = new IdentityUser
        {
            Id = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
            UserName = "secondUser@gmail.com",
            NormalizedUserName = "secondUser@gmail.com",
            Email = "secondUser@gmail.com",
            NormalizedEmail = "secondUser@gmail.com"
        };

        SecondUser.PasswordHash =
            hasher.HashPassword(SecondUser, "secondUser123");
    }

    private void SeedAgents()
    {
        FirstAgent = new Agent
        {
            Id = Guid.NewGuid(),
            FirstName = "Ivan",
            MiddleName = "Stoqnov",
            LastName = "Petrov",
            PhoneNumber = "+359888888888",
            DateOfBirth = new DateTime(1985, 5, 15),
            EGN = "8505151234",
            UserId = Guid.NewGuid()
        };

        SecondAgent = new Agent
        {
            Id = Guid.NewGuid(),
            FirstName = "Agent",
            MiddleName = "AgentJoe",
            LastName = "Agentov",
            PhoneNumber = "08459632555",
            DateOfBirth = new DateTime(1995,3,17),
            EGN = "95031756984",
           UserId = Guid.NewGuid()
        };
    }

    private void SeedCategories()
    {
        FirstCategory = new Category
        {
            Id = 1,
            Name = "Luxury"
        };

        SecondCategory = new Category
        {
            Id = 2,
            Name = "Budget"
        };

        ThirdCategory = new Category()
        {
            Id = 3,
            Name = "Business"
        };

        FourthCategory = new Category()
        {
            Id = 4,
            Name = "Family"
        };

        FiveCategory = new Category()
        {
            Id = 5,
            Name = "Hostel"
        };

        SixCategory = new Category()
        {
            Id = 6,
            Name = "Motel"
        };
    }

    private void SeedHotels()
    {
        FirstHotel = new Hotel()
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
            CategoryId = FirstCategory.Id,
            HotelManagerId = FirstAgent.Id,
            Image = "/images/HotelOne.jpg",
        };

        SecondHotel = new Hotel()
        {
            Id = Guid.NewGuid(),
            Name = "Seaside Resort",
            Address = "456 Ocean Avenue",
            City = "Beachtown",
            Country = "United States",
            Email = "info@seasideresort.com",
            PhoneNumber = "+3598756236514",
            Description = "Seaside Resort is a tranquil retreat nestled along the picturesque coastline, offering stunning ocean views and impeccable service.",
            NumberOfRooms = 150,
            MaximumCapacity = 400,
            CategoryId = SecondCategory.Id,
            HotelManagerId = FirstAgent.Id,
            Image = "/images/HotelOne.jpg",
        };

        ThirdHotel = new Hotel()
        {
            Id = Guid.NewGuid(),
            Name = "Mountain Lodge Retreat",
            Address = "789 Forest Road",
            City = "Mountainsville",
            Country = "United States",
            Email = "info@mountainlodge.com",
            PhoneNumber = "+3591914999",
            Description = "Mountain Lodge Retreat offers a peaceful escape surrounded by breathtaking natural beauty, perfect for outdoor enthusiasts and relaxation seekers.",
            NumberOfRooms = 100,
            MaximumCapacity = 300,
            CategoryId = FirstCategory.Id,
            HotelManagerId = SecondAgent.Id,
            Image = "/images/OceansViews.jpg",
        };
    }

    private void SeedReviews()
    {
        FirstReview = new Review()
        {
            Id = 1,
            Content = "Amazing experience, highly recommend!",
            Rating = 5,
            ReviewDate = DateTime.Now,
            UserId = Guid.NewGuid(),
            HotelId = FirstHotel.Id,
            RoomId = Guid.NewGuid() 
        };

        SecondReview = new Review()
        {
            Id = 2,
            Content = "Decent place, but could be better.",
            Rating = 3,
            ReviewDate = DateTime.Now,
           UserId = Guid.NewGuid(),
            HotelId = SecondHotel.Id,
            RoomId = Guid.NewGuid()
        };
    }

    private void SeedRoomsTypes()
    {
        FirstRoomType = new RoomType()
        {
            Id = 1,
            Name = "Single"
        };

        SecondRoomType = new RoomType()
        {
          Id = 2,
          Name = "Double"
        };
    }

    private void SeedRooms()
    {
        FirstRoom = new Room()
        {
            RoomId = 1,
            PricePerNight = 150.00m,
            IsAvaible = true,
            Image = "/rooms/room1.jpg",
            HotelId = FirstHotel.Id,
            AgentId = FirstAgent.Id,
        };

        SecondRoom = new Room()
        {
            RoomId = 2,
            PricePerNight = 75.00m,
            IsAvaible = true,
            Image = "/rooms/room2.jpg",
            HotelId = SecondHotel.Id,
            AgentId = SecondAgent.Id,
        };
    }
}
