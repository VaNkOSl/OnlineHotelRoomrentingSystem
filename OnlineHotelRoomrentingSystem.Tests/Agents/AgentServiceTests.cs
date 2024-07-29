namespace OnlineHotelRoomrentingSystem.Tests.Agents;

using Microsoft.EntityFrameworkCore;
using OnilineHotelRoomBookingSystem.Services.Data;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Agent;
using OnlineHotelRoomrentingSystem.Data;
using OnlineHotelRoomrentingSystem.Data.Data.Common;
using static DataBaseSeeder;

public class AgentServiceTests
{
    private DbContextOptions<HotelRoomBookingDb> dbOptions;
    private HotelRoomBookingDb dbContext;

    private IAgentService agentService;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        dbOptions = new DbContextOptionsBuilder<HotelRoomBookingDb>()
              .UseInMemoryDatabase("HotelRentingInMemory" + Guid.NewGuid().ToString())
              .Options;
        dbContext = new HotelRoomBookingDb(dbOptions);

        dbContext.Database.EnsureCreated();
        SeedDatabase(dbContext);

        IRepository repository = new Repository(dbContext);
        agentService = new AgentService(repository);
    }

    [Test]
    public async Task AgentExistsByUserIdAsyncShouldReturnTrueWhenExists()
    {
        string existingAgentUserId = AgentUser.Id.ToString();

        bool result = await agentService.ExistsByIdAsync(existingAgentUserId);

        Assert.IsTrue(result);
    }

    [Test]
    public async Task AgentExistsByUserIdAsyncShouldReturnFalseWhenNotExists()
    {
        string existingAgentUserId = RenterUser.Id.ToString();

        bool result = await agentService.ExistsByIdAsync(existingAgentUserId);

        Assert.IsFalse(result);
    }

    [Test]
    public async Task AgentWithPhoneNumberAlreadyExistsSholdReturnTrue()
    {
        string existingPhoneNumber = Agent.PhoneNumber.ToString();

        bool result = await agentService.UserWithPhoneNumberExistAsync(existingPhoneNumber);

        Assert.IsTrue(result);
    }

    [Test]
    public async Task AgentWithPhoneNumberDoesNotExistSholdReturnFalse()
    {
        string nonExistingPhoneNumber = "+359999999999";

        bool result = await agentService.UserWithPhoneNumberExistAsync(nonExistingPhoneNumber);

        Assert.IsFalse(result);
    }

    [Test]
    public async Task AgentWithEGNDoesNotExistSholdReturnFalse()
    {
        string nonExistinEgn = "8501151234";

        bool result = await agentService.UserWithEgnExistAsync(nonExistinEgn);

        Assert.IsFalse(result);
    }

    [Test]
    public async Task AgenUserWithEgnAlreadyExistSholdReturnTrue()
    {
        string existingEgn = Agent.EGN.ToString();

        bool result = await agentService.UserWithEgnExistAsync(existingEgn);

        Assert.IsTrue(result);
    }

    [Test]
    public async Task GetAgentIdShouldReturnTrue()
    {
        string expectedAgentId = RenterUser.Id.ToString();

        string agentId = await agentService.GetAgentIdAsync(Agent.UserId.ToString());

        Assert.IsNotNull(agentId);

        Assert.AreNotEqual(expectedAgentId, agentId);
    }

    [Test]
    public async Task GetAgentIdShouldReturnFalse()
    {
        string nonExistingAgentId = "f8c8ce23-d7eb-4e7a-a72e-7a9f178f95f3";

        string agentId = await agentService.GetAgentIdAsync(nonExistingAgentId);

        Assert.IsNull(agentId);
    }

    [Test]
    public async Task CreateAsync_ShouldAddAgentToDatabase()
    {
        var agentsCountBefore = dbContext.Agents.Count();

        var model = new BecomeAgentFormModel
        {
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "Testov",
            PhoneNumber = "+123456789",
            DateOfBirth = new DateTime(1990, 1, 1),
            EGN = "1234567890"
        };
        var userId = Guid.NewGuid().ToString();

        await agentService.CreateAsync(model, userId);

        var agentsCountAfter = dbContext.Agents.Count();
        Assert.That(agentsCountAfter,Is.EqualTo(agentsCountBefore + 1));

        var agent = await dbContext.Agents.FirstOrDefaultAsync(a => a.UserId.ToString() == userId);
        Assert.NotNull(agent);
        Assert.AreEqual(model.FirstName, agent.FirstName);
        Assert.AreEqual(model.LastName, agent.LastName);
        Assert.AreEqual(model.PhoneNumber, agent.PhoneNumber);
        Assert.AreEqual(model.DateOfBirth, agent.DateOfBirth);
        Assert.AreEqual(model.EGN, agent.EGN);
        Assert.AreEqual(model.MiddleName, agent.MiddleName);

    }

    [Test]
    public async Task AgentHasHotelReturnTrue()
    {
        string agentId = Agent.UserId.ToString();
        string hotelId = AgentHotel.Id.ToString();

        bool result = await agentService.HasHotelWithIdAsync(agentId, hotelId);

        Assert.IsTrue(result);
    }

    [Test]
    public async Task AgentHasHotelReturnFalse()
    {
        string agentId = Agent.UserId.ToString();
        string nonExistingHotelId = Guid.NewGuid().ToString();

        bool result = await agentService.HasHotelWithIdAsync(agentId, nonExistingHotelId);
         
        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task AgentHasRoomReturnTrue()
    {
        string agentid = Agent.UserId.ToString();
        string roomId  = AgentRoom.Id.ToString();

        bool result = await agentService.HasRoomsWithIdAsync(agentid, roomId);

        Assert.IsNotNull(result);
        Assert.IsTrue(result);
    }

    [Test]
    public async Task AgentHasRoomReturnFalse()
    {
        string agentId = Agent.UserId.ToString();
        string nonExistingRoomId = Guid.NewGuid().ToString();

        bool result = await agentService.HasRoomsWithIdAsync(agentId, nonExistingRoomId);

        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        dbContext.Dispose();
    }
}
