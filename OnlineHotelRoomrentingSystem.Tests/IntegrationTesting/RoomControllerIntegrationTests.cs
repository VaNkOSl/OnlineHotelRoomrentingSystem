namespace OnlineHotelRoomrentingSystem.Tests.IntegrationTesting;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Room;
using OnlineHotelRoomrentingSystem.Data;
using OnlineHotelRoomrentingSystem.Models;

public class RoomControllerIntegrationTests
{
    private WebApplicationFactory<Program> factory;
    private HttpClient client;
    private Mock<IRoomService> mockRoomService;
    private Mock<IHotelService> hotelServiceMock;
    private Mock<IAgentService> agentServiceMock;   

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        mockRoomService = new Mock<IRoomService>();
        hotelServiceMock = new Mock<IHotelService>();
        agentServiceMock = new Mock<IAgentService>();

        var options = new DbContextOptionsBuilder<HotelRoomBookingDb>()
            .UseInMemoryDatabase("RoomDataBaseTest" + Guid.NewGuid().ToString())
            .Options;

        factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(service =>
                {
                    service.AddScoped<HotelRoomBookingDb>(_ => new HotelRoomBookingDb(options));
                    service.AddScoped(_ => mockRoomService.Object);
                    service.AddScoped(_ => hotelServiceMock.Object);
                    service.AddScoped(_ => agentServiceMock.Object);

                });
            });

        client = factory.CreateClient();
    }

    [Test]
    public async Task AllRooms_ShouldReturnViewWithAllRooms()
    {
        var expectedRooms = new List<RoomServiceModel>
        {
            new RoomServiceModel { Id = "1", HotelName = "Room 1",  PricePerNight = 200 },
            new RoomServiceModel { Id = "2", HotelName = "Room 2",  PricePerNight = 300 }
        };

        mockRoomService.Setup(service => service.GetAllRoomsAsync())
            .ReturnsAsync(expectedRooms);

        var response = await client.GetAsync("/Room/All");

        response.EnsureSuccessStatusCode();


        var responseAsString = await response.Content.ReadAsStringAsync();

        foreach ( var rooms in expectedRooms)
        {
            Assert.That(responseAsString, Does.Contain($"{rooms.HotelName}"));
            Assert.That(responseAsString, Does.Contain($"{rooms.PricePerNight}"));
        }
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        factory.Dispose();
        client?.Dispose();
    }
}
