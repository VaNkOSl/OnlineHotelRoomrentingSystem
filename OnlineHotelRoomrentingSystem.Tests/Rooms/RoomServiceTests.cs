namespace OnlineHotelRoomrentingSystem.Tests.Rooms;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using OnilineHotelRoomBookingSystem.Services.Data;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Room;
using OnlineHotelRoomrentingSystem.Data;
using OnlineHotelRoomrentingSystem.Data.Data.Common;
using OnlineHotelRoomrentingSystem.Models;
using static OnlineHotelRoomrentingSystem.Tests.DataBaseSeeder;
using Agent = OnlineHotelRoomrentingSystem.Models.Agent;

public class RoomServiceTests
{
    private DbContextOptions<HotelRoomBookingDb> dbOptions;
    private HotelRoomBookingDb dbcontext;

    private IRepository repository;
    private IRoomService roomService;
    private IHotelService hotelService;
    private IReviewService reviewService;

    [SetUp]
    public void SetUp()
    {
        dbOptions = new DbContextOptionsBuilder<HotelRoomBookingDb>()
            .UseInMemoryDatabase("HotelRentingDbInMemory")
            .EnableSensitiveDataLogging()
            .Options;

        dbcontext = new HotelRoomBookingDb(dbOptions);
        dbcontext.Database.EnsureCreated();
        SeedDatabase(dbcontext);

        repository = new Repository(dbcontext);
        hotelService = new HotelService(repository, reviewService);
        roomService = new RoomService(repository, hotelService, reviewService);
    }

    [Test]
    public async Task RoomTypeExist_ShouldReturnTrueWhenRoomTypeExist()
    {
        var existingRoopType = FirstRoomType.Id;

        bool result = await roomService.RoomTypeExistAsync(existingRoopType);

        Assert.IsNotNull(result);
        Assert.IsTrue(result);
    }

    [Test]
    public async Task RoomTypeExists_ShouldReturnFalseWhenRoopTypeDoesNotExists()
    {
        var nonExistingRoomType = 14;

        bool result = await roomService.RoomTypeExistAsync(nonExistingRoomType);

        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task AllRoomTypes_ShouldRetturnCorrectRoomTypes()
    {
        var result = await roomService.AllRoomTypesAsync();

        Assert.IsNotNull(result);
        Assert.That(3, Is.EqualTo(result.Count()));
    }

    [Test]
    public async Task AllRoomTypes_ShouldReturnNotCorrectRoomTypes()
    {
        var result = await roomService.AllRoomTypesAsync();

        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.Not.EqualTo(10));
    }

    [Test]
    public async Task AllRoomByAgentId_ShouldReturncCorrectCount()
    {
        var agentId = AgentRoom.Id.ToString();

        var result = await roomService.AllRoomsByAgentIdAsync(agentId);

        var roomInDb = dbcontext.Rooms
            .Where(r => r.AgentId.ToString() == agentId);

        Assert.IsNotNull(result);
        Assert.That(result.Count(),Is.EqualTo(roomInDb.Count()));
    }

    [Test]
    public async Task AllRoomByAgent_ShouldReturnEmptyWhenAgentHasNoRooms()
    {
        var newAgent = new Agent
        {
            Id = Guid.NewGuid(),
            FirstName = "Room Test",
            LastName = "Testov"
        };

        var result = await roomService.AllRoomsByAgentIdAsync(newAgent.Id.ToString());

        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    [Test]
    public async Task AllRoomByAgent_ShouldReturncCorrectDetailsForRoom()
    {
        var agenId = AgentRoom.Id.ToString();

        var agentRooms = dbcontext.Rooms
            .Where(r => r.Id.ToString() == agenId)
            .ToList();

        var result = await roomService.AllRoomsByAgentIdAsync(agenId);

        foreach (var room in result)
        {
            var dbRoom = agentRooms.First(h => h.Id.ToString() == room.Id);

            Assert.That(room.HotelName, Is.EqualTo(dbRoom.Hotel.Name));
            Assert.That(room.PricePerNight,Is.EqualTo(dbRoom.PricePerNight));
            Assert.That(room.RoomType, Is.EqualTo(dbRoom.RoomType.Name));
            Assert.That(room.IsAvaible, Is.EqualTo(dbRoom.IsAvaible));
        }
    }

    [Test]
    public async Task RoomNumberAlreadyExists_ShouldReturnTrue()
    {
        var agenHotel = AgentHotel.Id.ToString();
        var roomNumber = AgentRoom.RoomNumber;

        var result = await roomService.RoomNumberAlreadyExistInTheHotelAsync(roomNumber, agenHotel);

        Assert.IsNotNull(result);
        Assert.IsTrue(result);
    }

    [Test]
    public async Task RoomNumberAlreadyExists_ShouldReturnFalse()
    {
        var agenHotel = AgentHotel.Id.ToString();
        var roomNumber = 15;

        var result = await roomService.RoomNumberAlreadyExistInTheHotelAsync(roomNumber, agenHotel);

        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task GetAllHotels_ShouldReturnCorrectCount()
    {
        var result = await roomService.GetAllHotelsAsync();

        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(5));
    }

    [Test]
    public async Task GetAllHotels_ShouldReturnNotCorrectCount()
    {
        var result = await roomService.GetAllHotelsAsync();

        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.Not.EqualTo(20));
    }

    [Test]
    public async Task GetAllRooms_ShouldReturnCorrectCount()
    {
        var result = await roomService.GetAllRoomsAsync();

        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetAllRooms_ShouldReturnNotCorrectCount()
    {
        var result = await roomService.GetAllRoomsAsync();

        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.Not.EqualTo(10));
    }

    [Test]
    public async Task GetRoomDetailsById_ShouldReturnCorrectRoomDetais()
    {
        var roomId = AgentRoom.Id.ToString();

        var result = await roomService.RoomDetailsByIdAsync(roomId);

        var roomInDb = await dbcontext.Rooms
                .Include(h => h.Hotel)
                .Include(a => a.Agent)
                .Include(r => r.Reviews)
                .Include(rt => rt.RoomType)
                .Include(r => r.Renter)
                .FirstAsync(r => r.Id.ToString() == roomId);

        Assert.Multiple(() =>
        {
            Assert.That(result.HotelName, Is.EqualTo(roomInDb.Hotel.Name));
            Assert.That(result.PricePerNight,Is.EqualTo(roomInDb.PricePerNight));
            Assert.That(result.IsAvaible, Is.EqualTo(roomInDb.IsAvaible));
            Assert.That(result.imageFile, Is.EqualTo(roomInDb.Image));
        });
    }

    [Test]
    public async Task AllRoomByUserId_ShouldReturnCorrectCount()
    {
        var renterId = AgentRoom.RenterId.ToString();

        var result = await roomService.AllRoomsByUsersIdAsync(renterId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task AllRoomByUserId_ShouldReturnNotCorrectCount()
    {
        var renterId = AgentRoom.RenterId.ToString();

        var result = await roomService.AllRoomsByUsersIdAsync(renterId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.Not.EqualTo(5));
    }

    [Test]
    public async Task GetRoomForEditing_ShouldRetturnCorrectHotelToEdit()
    {
        var roomId = AgentRoom.Id.ToString();

        var result = await roomService.EditRoomByIdAsync(roomId);

        var roomDb = dbcontext.Rooms
            .Include(rt => rt.RoomType)
            .First(r => r.Id.ToString() == roomId);

        Assert.Multiple(() =>
        {
            Assert.That(result.PricePerNight, Is.EqualTo(roomDb.PricePerNight));
            Assert.That(result.imageFile, Is.EqualTo(roomDb.Image));
            Assert.That(result.RoomNumber, Is.EqualTo(roomDb.RoomNumber));
            Assert.That(result.Description, Is.EqualTo(roomDb.Description));
        });
    }

    [Test]
    public async Task AgenHasRoom_ShouldReturnTrueWhenAgentHasRoom()
    {
        string agentId = DataBaseSeeder.Agent.Id.ToString();
        string roomId = AgentRoom.Id.ToString();

        bool result = await roomService.HasRoomAgenWithIdAsync(agentId, roomId);

        Assert.IsNotNull(result);
        Assert.True(result);

    }

    [Test]
    public async Task AgenHasRoom_ShouldRetunFalseWhenAgenIdIsStringEmty()
    {
        string agentId = string.Empty;
        string roomId = AgentRoom.Id.ToString();

        bool result = await roomService.HasRoomAgenWithIdAsync(agentId, roomId);

        Assert.IsNotNull(result);
        Assert.False(result);
    }

    [Test]
    public async Task AgenHasRoom_ShouldRetunrFalseWhenAgenIdDoesNotExist()
    {
        string agentId = Guid.NewGuid().ToString();
        string roomId = AgentRoom.Id.ToString();

        bool result = await roomService.HasRoomAgenWithIdAsync(agentId, roomId);

        Assert.IsNotNull(result);
        Assert.False(result);
    }

    [Test]
    public async Task AgenHasRoom_ShouldReturnFalseWhenAgenIdIsNull()
    {
        string agentId = null;
        string roomId = AgentRoom.Id.ToString();

        bool result = await roomService.HasRoomAgenWithIdAsync(agentId, roomId);

        Assert.IsNotNull(result);
        Assert.False(result);
    }

    [Test]
    public async Task EditRoomByFormModel_ShouldEditRoomSuccessfully()
    {
        var imageFileMock = new Mock<IFormFile>();

        var room = AgentRoom;

        var roomModel = new RoomFormModel
        {
            HotelId = AgentHotel.Id.ToString(),
            RoomNumber = 101,
            PricePerNight = 150.00M,
            Description = "Test description",
        };

        await roomService.EditRoomByFormModel(roomModel,room.Id.ToString() ,imageFileMock.Object);

        var uploadRooms = dbcontext.Rooms.First(r => r.Id == room.Id);

        Assert.Multiple(() =>
        {
            Assert.That(uploadRooms.Description,Is.EqualTo(roomModel.Description));
            Assert.That(uploadRooms.RoomNumber, Is.EqualTo(roomModel.RoomNumber));
            Assert.That(uploadRooms.PricePerNight, Is.EqualTo(roomModel.PricePerNight));
        });
    }

    [Test]
    public async Task DeleteRoomById_ShoulDeleteRoomSuccessfylly()
    {
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Sample Hotel",
            Email = "simpleEmail@com",
            PhoneNumber = "05896541455",
            NumberOfRooms = 5
        };

        var room = new Room
        {
            Id = Guid.NewGuid(),
            PricePerNight = 100,
            Hotel = hotel,
            Image = "sample.jpg",
            IsAvaible = true,
            Description = "Sample room description",
            HotelId = hotel.Id
        };

        dbcontext.Add(hotel);
        dbcontext.Add(room);
        await dbcontext.SaveChangesAsync();

        var roomId = room.Id.ToString();
        var hotelId = hotel.Id.ToString();

        var roomCountBeforeDeleting = dbcontext.Rooms.Count();

        await roomService.DeleteRoomByIdAsync(roomId);

        hotel.NumberOfRooms -= 1;

        var roomCountAfterDeleting = dbcontext.Rooms.Count();
        var deletedRoom = await dbcontext.Rooms.FindAsync(Guid.Parse(roomId));
        var updatedHotel = await dbcontext.Hotels.FindAsync(Guid.Parse(hotelId));

        Assert.Null(deletedRoom);
        Assert.NotNull(updatedHotel);
        Assert.AreEqual(4, updatedHotel.NumberOfRooms);
        Assert.That(roomCountAfterDeleting, Is.EqualTo(roomCountBeforeDeleting - 1));
    }

    [Test]
    public void DeleteRoomById_ShouldThrowExceptionForInvalidId()
    {
        var invalidRoomId = Guid.NewGuid().ToString();

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await roomService.DeleteRoomByIdAsync(invalidRoomId);
        });
    }

    [Test]
    public async Task IsRentedRoom_ShoulReturnTrueWenRoomIsRented()
    {
        var roomId = AgentRoom.Id.ToString();

        bool result = await roomService.IsRentedAsync(roomId);

        Assert.IsNotNull(result);
        Assert.IsTrue(result);
    }

    [Test]
    public async Task IsRentedAsync_ShouldReturnFalse_WhenRoomDoesNotExist()
    {
        var nonExistentRoomId = Guid.NewGuid().ToString();

        bool result = await roomService.IsRentedAsync(nonExistentRoomId);

        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task IsRentedAsync_ShouldReturnFalse_WhenRoomIsNotRented()
    {
        var room = new Room
        {
            Id = Guid.NewGuid(),
            RenterId = null, 
            IsAvaible = true
        };
        await dbcontext.Rooms.AddAsync(room);
        await dbcontext.SaveChangesAsync();

        bool result = await roomService.IsRentedAsync(room.Id.ToString());

        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task RoomIsRentedByUserWihtId_ShoulRetunTrue()
    {
        var userId = RenterUser.Id.ToString();
        var roomId = AgentRoom.Id.ToString();

        bool result = await roomService.IsRentedByUserWithIdAsync(userId, roomId);

        Assert.IsNotNull(result);
        Assert.IsTrue(result);
    }

    [Test]
    public async Task RoomIsRentedByUserWihtId_ShoulRetunFalse()
    {
        var userId = Guid.NewGuid().ToString();
        var roomId = AgentRoom.Id.ToString();

        bool result = await roomService.IsRentedByUserWithIdAsync(userId, roomId);

        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task RentAsync_ShouldRentRoomSuccessfully_WhenRoomIsAvailable()
    {
        var room = new Room
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            IsAvaible = true,
            RenterId = null
        };
        await dbcontext.Rooms.AddAsync(room);
        await dbcontext.SaveChangesAsync();

        var userId = Guid.NewGuid().ToString();

        await roomService.RentAsync(room.Id.ToString(), userId, room.HotelId.ToString());
        var updatedRoom = await dbcontext.Rooms.FindAsync(room.Id);

        Assert.IsFalse(updatedRoom.IsAvaible);
        Assert.That(updatedRoom.RenterId, Is.EqualTo(Guid.Parse(userId)));
    }

    [Test]
    public async Task RentAsync_ShouldThrowArgumentException_WhenRoomIsAlreadyRented()
    {
        var room = new Room
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            IsAvaible = false,
            RenterId = Guid.NewGuid()
        };
        await dbcontext.Rooms.AddAsync(room);
        await dbcontext.SaveChangesAsync();

        var userId = Guid.NewGuid().ToString();

        var ex = Assert.ThrowsAsync<ArgumentException>(() => roomService.RentAsync(room.Id.ToString(), userId, room.HotelId.ToString()));
        Assert.AreEqual("Room is already rented.", ex.Message);
    }

    [Test]
    public async Task RentAsync_ShouldThrowArgumentException_WhenRoomDoesNotExist()
    {
        var nonExistentRoomId = Guid.NewGuid().ToString();
        var hotelId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();

        var ex = Assert.ThrowsAsync<ArgumentException>(() => roomService.RentAsync(nonExistentRoomId, userId, hotelId));
        Assert.AreEqual("Room does not exist.", ex.Message);
    }

    [Test]
    public async Task LeaveRoom_WhenRoomExists()
    {
        var room = new Room
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            IsAvaible = false,
            RenterId = null
        };
        await dbcontext.Rooms.AddAsync(room);
        await dbcontext.SaveChangesAsync();

        var userId = Guid.NewGuid();

        await roomService.LeaveAsync(userId.ToString(), room.Id.ToString(), room.HotelId.ToString());
        var updatedRoom = await dbcontext.Rooms.FindAsync(room.Id);

        Assert.IsTrue(room.IsAvaible);
    }

    [Test]
    public async Task GetRoomById_ShouldReturnCorrectRoomId()
    {
        var roomId = AgentRoom.Id.ToString();

        var result = await roomService.GetRoomByIdAsync(roomId);

        var roomInDb = await dbcontext.Rooms
             .Include(x => x.Hotel)
            .Include(x => x.Agent)
            .Include(r => r.Renter)
            .Include(r => r.RoomType)
            .FirstAsync(x => x.Id.ToString() == roomId);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsAvaible, Is.EqualTo(roomInDb.IsAvaible));
            Assert.That(result.PricePerNight, Is.EqualTo(roomInDb.PricePerNight));
            Assert.That(result.imageFile, Is.EqualTo(roomInDb.Image));
            Assert.That(result.HotelId, Is.EqualTo(roomInDb.HotelId.ToString()));
            Assert.That(result.HotelName, Is.EqualTo(roomInDb.Hotel.Name));
            Assert.That(result.RoomType, Is.EqualTo(roomInDb.RoomType.Name));
        });
    }

    [Test]
    public async Task CreateRoomAsync_ShouldCreateRoomAndIncreaseHotelNumberOfRooms()
    {
        var roomFormModel = new RoomFormModel
        {
            Description = "Test Room",
            HotelId = Guid.NewGuid().ToString(),
            PricePerNight = 100,
            imageFile = null, 
            RoomNumber = 1011,
            RoomTypeId = 1
        };

        var agentId = Guid.NewGuid().ToString();

        var mockRepository = new Mock<IRepository>();
        var mockHotelService = new Mock<IHotelService>();
        var mockReviewService = new Mock<IReviewService>();

        mockRepository.Setup(r => r.AddAsync(It.IsAny<Room>()))
                      .Returns(Task.CompletedTask);

        mockHotelService.Setup(h => h.GetHotelByIdAsync(roomFormModel.HotelId))
                        .ReturnsAsync(new Hotel { Id = Guid.Parse(roomFormModel.HotelId), NumberOfRooms = 10 });

        var roomService = new RoomService(mockRepository.Object, mockHotelService.Object, mockReviewService.Object);

        var roomId = await roomService.CreateRoomAsync(roomFormModel, agentId, null);

        Assert.NotNull(roomId);
        Assert.That(roomId.GetType(), Is.EqualTo(typeof(string)));

        mockRepository.Verify(r => r.AddAsync(It.IsAny<Room>()), Times.Once);
        mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

        mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Hotel>()), Times.Once);

        mockRepository.Verify(r => r.AddAsync(It.Is<Room>(room =>
            room.Description == roomFormModel.Description &&
            room.HotelId == Guid.Parse(roomFormModel.HotelId) &&
            room.AgentId == Guid.Parse(agentId) &&
            room.IsAvaible == true &&
            room.PricePerNight == roomFormModel.PricePerNight &&
            room.RoomNumber == roomFormModel.RoomNumber &&
            room.RoomId == roomFormModel.RoomTypeId)), Times.Once);

        mockRepository.Verify(r => r.UpdateAsync(It.Is<Hotel>(hotel =>
            hotel.Id == Guid.Parse(roomFormModel.HotelId) &&
            hotel.NumberOfRooms == 11)), Times.Once); 
    }

    [Test]
    public async Task DeleteRoomByFormModel_ShouldReturnCorrectFormModel()
    {
        var roomForDeleteModel = new RoomPreDeleteViewModel
        {
            HotelName = "Luxory Place",
            PricePerNight = 100.00M,
            imageFile = null
        };

        var roomId = AgentRoom.Id.ToString();

        var result = await roomService.DeteleRoomFormModelByIdAsync(roomId);

        var roomInDb = await dbcontext.Rooms
             .Include(h => h.Hotel)
            .FirstAsync(r => r.Id.ToString() == roomId);

        Assert.Multiple(() =>
        {
            Assert.IsNotNull(roomInDb);
            Assert.That(result.HotelName, Is.EqualTo(roomInDb.Hotel.Name));
            Assert.That(result.PricePerNight,Is.EqualTo(roomInDb.PricePerNight));
        });
    }

    [Test]
    public async Task RoomExists_ShouldReturnTrueWhenRoomExists()
    {
        var roomId = AgentRoom.Id.ToString();

        bool result = await roomService.RoomExistsAsync(roomId);

        Assert.IsNotNull(result);
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task RoomExists_ShouldReturnFalseWhenRoomDoesNotExists()
    {
        var roomId = Guid.NewGuid().ToString();

        bool result = await roomService.RoomExistsAsync(roomId);

        Assert.IsNotNull(result);
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task RoomExists_ShouldReturnFalseWhemRoomIdIsEmpty()
    {
        var roomId = string.Empty;

        bool result = await roomService.RoomExistsAsync(roomId);

        Assert.IsNotNull(result);
        Assert.That(result, Is.False);
    }

    [TearDown]
    public void OneTimeTearDown()
    {
        dbcontext.Database.EnsureDeleted();
        dbcontext.Dispose();
    }
}
