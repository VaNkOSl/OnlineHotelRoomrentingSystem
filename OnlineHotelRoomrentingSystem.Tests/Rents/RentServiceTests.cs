namespace OnlineHotelRoomrentingSystem.Tests.Rents;

using Microsoft.EntityFrameworkCore;
using OnilineHotelRoomBookingSystem.Services.Data;
using OnlineHotelRoomrentingSystem.Data;
using OnlineHotelRoomrentingSystem.Data.Data.Common;
using OnlineHotelRoomrentingSystem.Models;
using static OnlineHotelRoomrentingSystem.Commons.EntityValidationConstants;
using Agent = Models.Agent;

public class RentServiceTests
{
    private DbContextOptions<HotelRoomBookingDb> options;

    [SetUp]
    public void OneTimeSetUp()
    {
        options = new DbContextOptionsBuilder<HotelRoomBookingDb>()
            .UseInMemoryDatabase(databaseName: "TestDataBase")
            .Options;

        using (var context = new HotelRoomBookingDb(options))
        {
            var agent1 = new Agent { Id = Guid.Parse("CFE7C099-1893-4922-AA5F-04C4FB84F955"), User = new ApplicationUser { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" } };
            var renter1 = new ApplicationUser { Id = Guid.Parse("CFE7C049-1893-4922-AA5F-04C4FB84F955"), Email = "jane.smith@example.com", FirstName = "Jane", LastName = "Smith" };
            var room1 = new Room { Id = Guid.NewGuid(), AgentId =Guid.Parse("CFD7C099-1893-4922-AA5F-04C4FB84F955") , RenterId = Guid.Parse("CFE7C049-1893-4922-AA5F-04C4FB84F955"),
                                          Image = "room1.jpg", RoomType = new RoomType { Id = 1, Name = "Single Room" } };

             context.Agents.Add(agent1);
             context.Users.Add(renter1);
             context.Rooms.Add(room1);

            context.SaveChanges();
        }
    }

    [Test]
    public async Task AllRentsAsync_ShouldReturnEmptyListWhenNoRentedRooms()
    {
        using(var context = new HotelRoomBookingDb(options))
        {
            var repository = new Repository(context);
            var rentService = new RentService(repository);

            context.Rooms.RemoveRange(context.Rooms);
            context.SaveChanges();

            var result = await rentService.AllRentsAsync();

            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(0));
        }
    }
}
