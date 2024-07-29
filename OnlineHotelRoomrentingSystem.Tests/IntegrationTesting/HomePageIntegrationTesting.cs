namespace OnlineHotelRoomrentingSystem.Tests.IntegrationTesting;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineHotelRoomrentingSystem.Data;
using OnlineHotelRoomrentingSystem.Models;
using Xunit;

public class HomePageIntegrationTesting
{
    private  WebApplicationFactory<Program> _factory;
    private  HttpClient _client;


    [OneTimeSetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<HotelRoomBookingDb>()
      .UseInMemoryDatabase(databaseName: "TestDatabase")
      .Options;

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<HotelRoomBookingDb>(_ => new HotelRoomBookingDb(options));
                });
            });

        _client = _factory.CreateClient();

    }

    [Test]
    public async Task Index_ReturnsHomePage_WithHotels()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<HotelRoomBookingDb>();

            dbContext.Hotels.AddRange(new List<Hotel>
        {
            new Hotel { Id = Guid.NewGuid(), Name = "Hotel One", Image = "Image two", Email = "testEmailOne@abv.bg", PhoneNumber = "0877775496" },
            new Hotel { Id = Guid.NewGuid(), Name = "Hotel two", Image = "Image one", Email = "testEmailTwo@abv.bg", PhoneNumber = "0874215693" }
        });

            dbContext.SaveChanges();

            var response = await _client.GetAsync("/");

            response.EnsureSuccessStatusCode();

            var responseAsString = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseAsString);

            Assert.Contains("<h1 class=\"display-4\">Welcome to the best place for renting hotels!</h1>", responseAsString);

            foreach (var hotel in dbContext.Hotels)
            {
                Assert.Contains($"<img class=\"d-block w-100\" style=\"height:500px\"\r\n                     src=\"{hotel.Image}\" alt=\"{hotel.Name}\">", responseAsString);
            }

            Assert.DoesNotContain("There are no houses", responseAsString);
        }
    }


    [Test]
    public async Task Index_ReturnsHomePage()
    {
        var response = await _client.GetAsync("/");

        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();

        Assert.Contains("<h1 class=\"display-4\">Welcome to the best place for renting hotels!</h1>", responseAsString);
    }

    [OneTimeTearDown]
    public void Dispose()
    {
        _factory.Dispose();
        _client.Dispose();
    }
}
