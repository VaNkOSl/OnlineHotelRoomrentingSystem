namespace OnlineHotelRoomrentingSystem.Tests.IntegrationTesting;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Agent;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Hotel;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Review;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Room;
using OnlineHotelRoomrentingSystem.Data;
using OnlineHotelRoomrentingSystem.Extensions.Contacts;
using System;

public class HotelControllerIntegrationTest
{
    private WebApplicationFactory<Program> factory;
    private HttpClient client;
    private Mock<IAgentService> mockAgentService;
    private Mock<IHotelService> mockHotelService;
    private Mock<IRoomService> mockRoomService;
    private Mock<IFileService> mockFileService;
    private Mock<IWebHostEnvironment> mockWebHostEncironment;


    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        mockAgentService = new Mock<IAgentService>();
        mockHotelService = new Mock<IHotelService>();
        mockRoomService = new Mock<IRoomService>();
        mockFileService = new Mock<IFileService>();
        mockWebHostEncironment = new Mock<IWebHostEnvironment>();

        var options = new DbContextOptionsBuilder<HotelRoomBookingDb>()
            .UseInMemoryDatabase("HotelTest" + Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;


        factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(service =>
                {
                    service.AddScoped<HotelRoomBookingDb>(_ => new HotelRoomBookingDb(options));
                    service.AddScoped(_ => mockAgentService.Object);
                    service.AddScoped(_ => mockHotelService.Object);
                    service.AddScoped(_ => mockRoomService.Object);
                    service.AddScoped(_ => mockFileService.Object);
                });
            });

        client = factory.CreateClient();
    }

    [Test]
    public async Task HotelAll_ShouldRetunrViewWithAllHotels()
    {
        using(var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<HotelRoomBookingDb>();

            await dbContext.SaveChangesAsync();

            mockHotelService.Setup(service => service.AllHotelsAsync(It.IsAny<AllHotelsQueryModel>()))
                .ReturnsAsync(new HotelQueryServiceModel
                {
                    Hotels = new List<HotelServiceModel>
                    {
                        new HotelServiceModel { Id = "1", Name = "Hotel One",
                                    City = "Burgas", Country = "Bulgaria" , imageFile = "Image two" },
                        new HotelServiceModel { Id = "2", Name = "Hotel two",
                                     City = "Sofia", Country = "Bulgaria", imageFile = "Image one" }
                    },
                    TotalHotelsCount = 2
                });


            var response = await client.GetAsync("/Hotel/All");

            response.EnsureSuccessStatusCode();

            var responseAsString = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseAsString);

            Assert.That(responseAsString, Does.Contain("<h2 class=\"text-center\">All Hotels</h2>"));
            Assert.That(responseAsString, Does.Contain("<label for=\"Category\">Category</label>"));
            Assert.That(responseAsString, Does.Contain("<label for=\"HotelSorting\">Sort Hotel By</label>"));
            Assert.That(responseAsString, Does.Contain("<label for=\"SearchinString\">Search by word</label>"));
            Assert.That(responseAsString, Does.Contain("   <input type=\"submit\" value=\"Search\" class=\"btn btn-primary\" />"));

            foreach(var hotel in dbContext.Hotels)
            {
                Assert.That(responseAsString, Does.Contain($" <h4>Name: {hotel.Name}</h4>"));
                Assert.That(responseAsString, Does.Contain($"<h5>City: {hotel.City}</h5>"));
                Assert.That(responseAsString, Does.Contain($" <h5>Country: {hotel.Country}</h5>"));
                Assert.That(responseAsString, Does.Contain($" <img class=\"card-img-top\" src=\"{hotel.Image}\" alt=\"Hotel Image\">"));
            }
        }
    }

    [Test]
    public async Task HotelDetails_ShouldretunViewWithHotelDetailsWhenHotelExists()
    {
        var hotelId1 = Guid.Parse("93e72461-4811-4483-952f-41d221ab1091");

        mockHotelService.Setup(service => service.HotelExistsAsync(hotelId1.ToString())).ReturnsAsync(true);

        var expectedHotelModel = new HotelDetailsViewModel
        {
            Id = hotelId1.ToString(),
            Name = "Test Hotel",
            Email = "test@gmail.com",
            Address = "Test Adress.com",
            City = "Burgas",
            Country = "Bulgaria",
            Description = "This is test description for hotel",
            NumberOfRooms = 10,
            MaximumCapacity = 500,
            HotelManager = new AgentServiceModel
            {
                FullName = "Ivan Petrov Petrov"
            },
            Reviews = new List<HotelReviewViewModel>()
        {
            new HotelReviewViewModel {Content = "Test", Rating = 5,ReviewDate = DateTime.Now}
        }.ToList(),
            Rooms = new List<RoomServiceModel>
        {
            new RoomServiceModel { Id = "1", IsAvaible = true, PricePerNight = 100 },
            new RoomServiceModel { Id = "2", IsAvaible = false, PricePerNight = 150 }
        }.ToList()
            };

        mockHotelService.Setup(service => service.HotelDetailsByIdAsync(hotelId1.ToString()))
            .ReturnsAsync(expectedHotelModel);

        var response = await client.GetAsync($"/Hotel/Details?id={hotelId1}");

        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();


        Assert.That(responseAsString, Does.Contain("<h2 class=\"text-center\">Hotel Details</h2>"));

        Assert.That(responseAsString, Does.Contain($"<p style=\"font-size:25px;\"><u>{expectedHotelModel.Name}</u></p>"));
        Assert.That(responseAsString, Does.Contain($"<p>Located in: <b>{expectedHotelModel.Address}</b></p>"));
        Assert.That(responseAsString, Does.Contain($"<p>Number of Rooms: {expectedHotelModel.NumberOfRooms}</p>"));
        Assert.That(responseAsString, Does.Contain($"<p>Maximum Capacity: {expectedHotelModel.MaximumCapacity}</p>"));
        Assert.That(responseAsString, Does.Contain($"<p>{expectedHotelModel.Description}</p>"));
        Assert.That(responseAsString, Does.Contain("<h5 class=\"card-title\">Hotel Manager Info</h5>"));
        Assert.That(responseAsString, Does.Contain($"<p class=\"card-text\">Full Name: {expectedHotelModel.HotelManager.FullName}</p>"));
    }

    [Test]
    public async Task AddHotelGet_ShouldRetturnViewForAddingHotel()
    {
        mockAgentService.Setup(service => service.ExistsByIdAsync(It.IsAny<string>())).ReturnsAsync(true);

        var response = await client.GetAsync("/Hotel/Add");

        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();

      
        Assert.That(responseAsString, Does.Contain("<label for=\"Address\">Enter Adress</label>"));
        Assert.That(responseAsString, Does.Contain("<label for=\"City\">Enter City</label>"));
        Assert.That(responseAsString, Does.Contain("<label for=\"Country\">Enter Country</label>"));
        Assert.That(responseAsString, Does.Contain("<label for=\"City\">Enter City</label>"));
        Assert.That(responseAsString, Does.Contain("<label for=\"Email\">Enter Email</label>"));
        Assert.That(responseAsString, Does.Contain("<label for=\"PhoneNumber\">Enter Phone Number</label>"));
        Assert.That(responseAsString, Does.Contain("<label for=\"Description\">Enter Description</label>"));
        Assert.That(responseAsString, Does.Contain("<label for=\"MaximumCapacity\">Enter maximum capacity of the hotel</label>"));
        Assert.That(responseAsString, Does.Contain("label for=\"NumberOfRooms\">Enter number of rooms</label>"));
        Assert.That(responseAsString, Does.Contain("<label for=\"imageFile\">Impload Photo of the Hotel</label>"));
        Assert.That(responseAsString, Does.Contain("<label for=\"CategoryId\">Category</label>"));
        Assert.That(responseAsString, Does.Contain("<label for=\"Name\">Enter Name</label>"));
    }

    [Test]
    public async Task EditHotel_ShouldRetunrCorrecViewForEditingHotel()
    {
        var hotelId = Guid.NewGuid().ToString();
        var agentId = Guid.NewGuid().ToString();

        mockHotelService.Setup(service => service.HotelExistsAsync(hotelId)).ReturnsAsync(true);
        mockHotelService.Setup(service => service.HasHotelManagerWithIdAsync(hotelId, agentId)).ReturnsAsync(true);
        mockAgentService.Setup(service => service.GetAgentIdAsync(It.IsAny<string>())).ReturnsAsync(agentId);

        var expectedHotelToEdit = new HotelFormModel
        {
            Email = "testEmail.com",
            Address = "Test adress",
            City = "Burgas",
            Country = "Bulgaria",
            Description = "test description",
            MaximumCapacity = 100,
            NumberOfRooms = 20,
            Name = "Test Name",
            PhoneNumber = "0885555559",
            CategoryId = 1
        };

        mockHotelService.Setup(service => service.EditHotelByIdAsync(hotelId.ToString()))
          .ReturnsAsync(expectedHotelToEdit);

        var response = await client.GetAsync($"/Hotel/Edit?id={hotelId}");

        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseAsString);

        Assert.That(responseAsString, Does.Contain("Test Name"));
        Assert.That(responseAsString, Does.Contain("Test adress"));
        Assert.That(responseAsString, Does.Contain("Burgas"));
        Assert.That(responseAsString, Does.Contain("Bulgaria"));
        Assert.That(responseAsString, Does.Contain("test description"));
        Assert.That(responseAsString, Does.Contain("100"));
        Assert.That(responseAsString, Does.Contain("20"));
        Assert.That(responseAsString, Does.Contain("0885555559"));
    }

    [Test]
    public async Task DeleteHotel_ShouldRetunCorrectViewForDeletingHotel()
    {
        var hotelId = Guid.NewGuid().ToString();
        var agentId = Guid.NewGuid().ToString();

        mockAgentService.Setup(service => service.GetAgentIdAsync(It.IsAny<string>())).ReturnsAsync(agentId);
        mockHotelService.Setup(service => service.HotelExistsAsync(hotelId)).ReturnsAsync(true);
        mockHotelService.Setup(service => service.HasHotelManagerWithIdAsync(hotelId, agentId)).ReturnsAsync(true);

        var modelToDelete = new HotelPreDeleteDetailsViewModel
        {
            Address = "test Adress",
            City = "Burgas",
            Name = "Hotel one to delete",
            Image = "test.jpn"
        };

        mockHotelService.Setup(service => service.GetHotelForDeleteByIdAsync(hotelId)).ReturnsAsync(modelToDelete);

        var response = await client.GetAsync($"/Hotel/Delete/?id={hotelId}");

        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();

        Assert.That(responseAsString, Does.Contain("test.jpn"));
        Assert.That(responseAsString, Does.Contain("Hotel one to delete"));
        Assert.That(responseAsString, Does.Contain("test Adress"));
        Assert.That(responseAsString, Does.Contain("Burgas"));
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        factory.Dispose(); 
        client?.Dispose();
    }
}
