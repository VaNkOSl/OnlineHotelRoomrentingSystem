namespace OnlineHotelRoomrentingSystem.Tests.IntegrationTesting;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineHotelRoomrentingSystem.Data;

public class AgentBecomeIntegrationTesting
{
    private WebApplicationFactory<Program> factory;
    private HttpClient client;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var options = new DbContextOptionsBuilder<HotelRoomBookingDb>()
            .UseInMemoryDatabase("TestDataBaseForAgent")
            .Options;

        factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(service =>
                {
                    service.AddScoped<HotelRoomBookingDb>(_ => new HotelRoomBookingDb(options));
                });
            });

        client = factory.CreateClient();
    }

    [Test]
    public async Task Become_ReturnViewModelForBecomeAgent()
    {
        var response = await client.GetAsync("/Agent/Become");

        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();

        Console.WriteLine(responseAsString);

        Assert.That(responseAsString, Does.Contain("<label class=\"font-weight-bold\" for=\"FirstName\">First Name:</label>"));
        Assert.That(responseAsString, Does.Contain("<label class=\"font-weight-bold\" for=\"MiddleName\">Middle Name:</label>"));
        Assert.That(responseAsString, Does.Contain("<label class=\"font-weight-bold\" for=\"LastName\">Last Name:</label>"));
        Assert.That(responseAsString, Does.Contain("<label class=\"font-weight-bold\" for=\"DateOfBirth\">Date of Birth:</label>"));
        Assert.That(responseAsString, Does.Contain("<label class=\"font-weight-bold\" for=\"EGN\">EGN:</label>"));
        Assert.That(responseAsString, Does.Contain("<label class=\"font-weight-bold\" for=\"PhoneNumber\">Phone Number:</label>"));
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        factory.Dispose();
        client?.Dispose();
    }


}
