namespace OnlineHotelRoomrentingSystem.Tests.Hotels;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NuGet.Protocol.Core.Types;
using OnilineHotelRoomBookingSystem.Services.Data;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Hotel;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Hotel.Enum;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Review;
using OnlineHotelRoomrentingSystem.Data;
using OnlineHotelRoomrentingSystem.Data.Data.Common;
using OnlineHotelRoomrentingSystem.Models;
using System.Linq.Expressions;
using static DataBaseSeeder;
using static OnlineHotelRoomrentingSystem.Commons.EntityValidationConstants;

public class HotelServiceTest
{
    private DbContextOptions<HotelRoomBookingDb> dbOptions;
    private HotelRoomBookingDb dbContext;
    private IRepository repository;
    private IReviewService reviewService;
    private Mock<IRepository> mockRepository;

    private IHotelService hotelService;

    [SetUp]
    public void OneTimeSetUp()
    {
        dbOptions = new DbContextOptionsBuilder<HotelRoomBookingDb>()
            .UseInMemoryDatabase("HotelRentingInMemory" + Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        dbContext = new HotelRoomBookingDb(dbOptions);
        dbContext.Database.EnsureCreated();
        SeedDatabase(dbContext);

        repository = new OnlineHotelRoomrentingSystem.Data.Data.Common.Repository(dbContext);
        reviewService = new Mock<IReviewService>().Object;
        mockRepository = new Mock<IRepository>();
        hotelService = new HotelService(repository,reviewService);
    }

    [Test]
    public async Task LastThreeHotel_SholdReturnCorrectHotels()
    {
        var result = await hotelService.LastThreeHotelsAsync();

        var hotelsInDb = dbContext.Hotels
            .OrderByDescending(h => h.Id)
            .Take(3);

        Assert.That(result.Count(), Is.EqualTo(hotelsInDb.Count()));

        var firstHotelInDb = hotelsInDb.FirstOrDefault();
        var firstResultHotel = result.FirstOrDefault();

        Assert.That(firstResultHotel.Id, Is.EqualTo(firstHotelInDb.Id));
        Assert.That(firstResultHotel.Title, Is.EqualTo(firstHotelInDb.Name));

    }

    [Test]
    public async Task CategoryExists_ShouldReturnTrue()
    {
        var categoryId = FirstCategory.Id;

        var result = await hotelService.CategoryExistAsync(categoryId);

        Assert.IsNotNull(result);
        Assert.IsTrue(result);
    }

    [Test]
    public async Task CategoryExist_ShouldReturnFalse()
    {
        var nonExistingCategoryid = 18;

        var result = await hotelService.CategoryExistAsync(nonExistingCategoryid);

        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task AllCategories_ShouldReturnCorrectCount()
    {
        var categories = await hotelService.AllCategoriesAsync();

       
        var expectedCount = dbContext.Categories.Count();
        Assert.AreEqual(expectedCount, categories.Count());

        var categoriesList = categories.ToList();

        foreach (var dbCategory in dbContext.Categories)
        {
            var serviceModel = categoriesList.FirstOrDefault(c => c.Id == dbCategory.Id);

            Assert.IsNotNull(serviceModel);
            Assert.AreEqual(dbCategory.Name, serviceModel.Name);
        }
    }

    [Test]
    public async Task AllCategories_ShouldReturnNotCorrectCount()
    {
        var categories = await hotelService.AllCategoriesAsync();

        var expectedCount = dbContext.Categories.Count() + 1;
        Assert.AreNotEqual(expectedCount, categories.Count());
    }

    [Test]
    public async Task HotelWithTheSameEmail_ShouldreturnTrue()
    {
        var hotelEmail = AgentHotel.Email;

        var result = await hotelService.HotelWithTheSameEmailExistAsync(hotelEmail);

        Assert.IsNotNull(result);
        Assert.IsTrue(result);
    }

    [Test]
    public async Task HotelWithTheSameEmail_ShouldReturnFalse()
    {
        var hotelEmail = "nonExisting@email.com";

        var result = await hotelService.HotelWithTheSameEmailExistAsync(hotelEmail);

        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task HotelWithTheSamePhoneNumber_ShouldReturnTrue()
    {
        var hotelNumber = AgentHotel.PhoneNumber;

        var result = await hotelService.HotelWithTheSamePhoneNumberExist(hotelNumber);

        Assert.IsNotNull(result);
        Assert.IsTrue(result);
    }

    [Test]
    public async Task HotelWithTheSamePhoneNumber_ShouldReturnFalse()
    {
        var nonExistingPhoneNumber = "0578654145";

        var result = await hotelService.HotelWithTheSamePhoneNumberExist(nonExistingPhoneNumber);

        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task AllHotelsAsync_ShouldReturnFilteredHotels_ByCategory()
    {
        var queryModel = new AllHotelsQueryModel
        {
            Category = "Luxury",
            CurrentPage = 1,
            HotelsPerPage = 10
        };

        var result = await hotelService.AllHotelsAsync(queryModel);

        Assert.NotNull(result);
        Assert.That(result.Hotels.First().Name, Is.EqualTo("Luxury Palace Hotel"));
    }

    [Test]
    public async Task AllHotelsAsync_ShouldReturnFilteredHotels_BySearchString()
    {
        var queryModel = new AllHotelsQueryModel
        {
            SearchinString = "Luxury",
            CurrentPage = 1,
            HotelsPerPage = 10
        };

        var result = await hotelService.AllHotelsAsync(queryModel);

        Assert.NotNull(result);
        Assert.That(result.Hotels.First().Name, Is.EqualTo("Luxury Palace Hotel"));
    }

    [Test]
    public async Task AllHotelsAsync_ShouldReturnSortedHotels_ByNewest()
    {
        var queryModel = new AllHotelsQueryModel
        {
            HotelSorting = HotelSorting.Newest,
            CurrentPage = 1,
            HotelsPerPage = 10
        };

        var result = await hotelService.AllHotelsAsync(queryModel);

        Assert.NotNull(result);
        Assert.That(result.Hotels.Count(), Is.EqualTo(5));
        Assert.AreEqual("Luxury Palace Hotel", result.Hotels.First().Name);
    }

    [Test]
    public async Task AllHotelsAsync_ShouldReturnPagedHotels()
    {
       
        var queryModel = new AllHotelsQueryModel
        {
            CurrentPage = 1,
            HotelsPerPage = 2
        };

    
        var result = await hotelService.AllHotelsAsync(queryModel);

        Assert.NotNull(result);
        Assert.AreEqual(2, result.Hotels.Count());
    }

    [Test]
    public async Task AllHotelsByAgentId_ShouldReturnCorrectHotels()
    {
        var agentId = DataBaseSeeder.Agent.Id.ToString();

        var result = await hotelService.AllHotelsByAgentIdAsync(agentId);

        Assert.IsNotNull(result);

        var hotelsInDb = dbContext.Hotels
            .Where(h => h.HotelManagerId.ToString() == agentId);
        Assert.That(result.Count(), Is.EqualTo(hotelsInDb.Count()));
    }

    [Test]
    public async Task AllHotelsByAgentId_ShouldReturnEmptyWhenAgentHasNoHotels()
    {
        var newAgent = new Models.Agent
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
        };

        dbContext.Agents.Add(newAgent);
        dbContext.SaveChanges();

        var result = await hotelService.AllHotelsByAgentIdAsync(newAgent.Id.ToString());

        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    [Test]
    public async Task AllHotelsByAgentId_ShouldReturnCorrectHotelDetails()
    {
        var agentId =DataBaseSeeder.Agent.Id.ToString();
        var agentHotels = dbContext.Hotels
            .Where(h => h.HotelManagerId.ToString() == agentId)
            .ToList();

        var result = await hotelService.AllHotelsByAgentIdAsync(agentId);

        foreach (var hotel in result)
        {
            var dbHotel = agentHotels.First(h => h.Id.ToString() == hotel.Id);
            Assert.That(hotel.Name, Is.EqualTo(dbHotel.Name));
            Assert.That(hotel.Address, Is.EqualTo(dbHotel.Address));
            Assert.That(hotel.City, Is.EqualTo(dbHotel.City));
            Assert.That(hotel.Country, Is.EqualTo(dbHotel.Country));
            Assert.That(hotel.imageFile, Is.EqualTo(dbHotel.Image));
        }
    }

    [Test]
    public async Task HotelExist_ShouldReturnTrue()
    {
        var hotelId = AgentHotel.Id.ToString();

        bool result = await hotelService.HotelExistsAsync(hotelId);

        Assert.That(result, Is.True);
        Assert.IsNotNull(result);
    }

    [Test]
    public async Task HotelExist_ShouldReturnFalse()
    {
        var nonExistingHotelId = Guid.NewGuid();

        bool result = await hotelService.HotelExistsAsync(nonExistingHotelId.ToString());

        Assert.That(result, Is.False);
        Assert.IsNotNull(result);
    }

    [Test]
    public async Task HasHotelManagerWithIdAsync_ShouldReturnTrueWhenManagerIdMatches()
    {
        var hotelId = AgentHotel.Id.ToString();
        var managerId = AgentHotel.HotelManagerId.ToString();

        bool result = await hotelService.HasHotelManagerWithIdAsync(hotelId, managerId);

        Assert.IsTrue(result);
        Assert.IsNotNull(result);
    }

    [Test]
    public async Task HasHotelManagerWithIdAsync_ShouldReturnFalseWhenManagerIdDoesNotMatch()
    {
        var hotelId = AgentHotel.Id.ToString();
        var differentManagerId = Guid.NewGuid().ToString();

        bool result = await hotelService.HasHotelManagerWithIdAsync(hotelId, differentManagerId);

        Assert.IsFalse(result);
        Assert.IsNotNull(result);
    }


    [Test]
    public async Task HotelDetailsByIdAsync_ShouldReturnCorrectHotelDetails()
    {
        var hotelId = AgentHotel.Id.ToString();

        var result = await hotelService.HotelDetailsByIdAsync(hotelId);

        var hotelsInDb = dbContext.Hotels
            .Include(x => x.Category)
            .Include(x => x.Reviews)
            .Include(r => r.Rooms)
            .Include(x => x.HotelManager)
            .ThenInclude(x => x.User)
            .First(x => x.Id.ToString() == hotelId);

        Assert.Multiple(() =>
        {
            Assert.That(result.Name, Is.EqualTo(hotelsInDb.Name));
            Assert.That(result.Address, Is.EqualTo(hotelsInDb.Address));
            Assert.That(result.City, Is.EqualTo(hotelsInDb.City));
            Assert.That(result.Country, Is.EqualTo(hotelsInDb.Country));
            Assert.That(result.Email, Is.EqualTo(hotelsInDb.Email));
            Assert.That(result.PhoneNumber, Is.EqualTo(hotelsInDb.PhoneNumber));
            Assert.That(result.Description, Is.EqualTo(hotelsInDb.Description));
            Assert.That(result.NumberOfRooms, Is.EqualTo(hotelsInDb.NumberOfRooms));
            Assert.That(result.CategoryId, Is.EqualTo(hotelsInDb.CategoryId));
            Assert.That(result.CategoryName, Is.EqualTo(hotelsInDb.Category.Name));
            Assert.That(result.HotelManager.FullName, Is.EqualTo($"{hotelsInDb.HotelManager.FirstName} {hotelsInDb.HotelManager.MiddleName} {hotelsInDb.HotelManager.LastName}"));
            Assert.That(result.HotelManager.PhoneNumber, Is.EqualTo(hotelsInDb.HotelManager.PhoneNumber));
            Assert.That(result.HotelManager.Email, Is.EqualTo(hotelsInDb.HotelManager.User.Email));
        });
    }

    [Test]
    public async Task GetHotelForDeleteById_ShouldReturnCorrectHotelForDeleting()
    {
        var hotelId = AgentHotel.Id.ToString();

        var result = await hotelService.GetHotelForDeleteByIdAsync(hotelId);

        var hotelsInDb = dbContext.Hotels
            .First(h => h.Id.ToString() == hotelId);

        Assert.Multiple(() =>
        {
            Assert.That(result.Name, Is.EqualTo(hotelsInDb.Name));
            Assert.That(result.City, Is.EqualTo(hotelsInDb.City));
            Assert.That(result.Address, Is.EqualTo(hotelsInDb.Address));
            Assert.That(result.Image, Is.EqualTo(hotelsInDb.Image));
        });
    }

    [Test]
    public async Task GetHotelForDeleteById_ValidHotelId_ShouldReturnNotNull()
    {
        var hotelId = AgentHotel.Id.ToString();

        var result = await hotelService.GetHotelForDeleteByIdAsync(hotelId);

        Assert.IsNotNull(result);
    }

    [Test]
    public void GetHotelForDeleteById_NonExistentHotelId_ShouldThrowInvalidOperationException()
    {
        var nonExistentHotelId = Guid.NewGuid().ToString();

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await hotelService.GetHotelForDeleteByIdAsync(nonExistentHotelId));
    }

    [Test]
    public async Task GetHotelByIdAsync_ShouldReturnCorrectHotel()
    {
        var hotelId = AgentHotel.Id.ToString();

        var result = await hotelService.GetHotelByIdAsync(hotelId);

        var hotelsInDb = dbContext.Hotels
            .Include(h => h.Reviews)
            .Include(r => r.Rooms)
            .Include(a => a.HotelManager)
            .First(h => h.Id.ToString() == hotelId);

        Assert.Multiple(() =>
        {
            Assert.That(result.Name, Is.EqualTo(hotelsInDb.Name));
            Assert.That(result.Address, Is.EqualTo(hotelsInDb.Address));
            Assert.That(result.City, Is.EqualTo(hotelsInDb.City));
            Assert.That(result.Country, Is.EqualTo(hotelsInDb.Country));
            Assert.That(result.Email, Is.EqualTo(hotelsInDb.Email));
            Assert.That(result.PhoneNumber, Is.EqualTo(hotelsInDb.PhoneNumber));
            Assert.That(result.Description, Is.EqualTo(hotelsInDb.Description));
            Assert.That(result.NumberOfRooms, Is.EqualTo(hotelsInDb.NumberOfRooms));
            Assert.That(result.MaximumCapacity, Is.EqualTo(hotelsInDb.MaximumCapacity));
            Assert.That(result.HotelManagerId, Is.EqualTo(hotelsInDb.HotelManagerId));
        });
    }

    [Test]
    public async Task GetHotelByIdAsync_NonExistentHotelId_ShouldReturnNull()
    {
        var nonExistingHotelId = Guid.NewGuid();

        var result = await hotelService.GetHotelByIdAsync(nonExistingHotelId.ToString());

        Assert.IsNull(result);
    }

    [Test]
    public async Task GetHotelByIdAsync_NullHotelId_ShouldReturnNull()
    {
        var result = await hotelService.GetHotelByIdAsync(null);

        Assert.IsNull(result);
    }

    [Test]
    public async Task DeleteHotelByIdAsync_ShouldDeleteHotelWhenHotelExists()
    {
        var hotelId = AgentHotel.Id.ToString();

        var hotelsCountBefore =  dbContext.Hotels.Count();

        await hotelService.DeleteHotelByIdAsync(hotelId);
        await reviewService.DeleteReviewsByHotelIdAsync(hotelId);
        await hotelService.DeleteRoomsByHotelIdAsync(hotelId);

        var hotelsCountAfterDeleting = dbContext.Hotels.Count();    

        var hotelInDb = await dbContext.Hotels
            .FirstOrDefaultAsync(h => h.Id.ToString() == hotelId);

        Assert.IsNull(hotelInDb);
        Assert.That(hotelsCountAfterDeleting, Is.EqualTo(hotelsCountBefore - 1));
    }

    [Test]
    public async Task GetEditHotelById_ShouldReturnCorrectHotelForEditing()
    {
        var hotelId = AgentHotel.Id.ToString();

        var result = await hotelService.EditHotelByIdAsync(hotelId);

        var hotelInDb = dbContext.Hotels
            .Include(c => c.Category)
            .First(h => h.Id.ToString() == hotelId);

        Assert.Multiple(() =>
        {
            Assert.That(result.Name,Is.EqualTo(hotelInDb.Name));
            Assert.That(result.Address, Is.EqualTo(hotelInDb.Address));
            Assert.That(result.City, Is.EqualTo(hotelInDb.City));
            Assert.That(result.Country, Is.EqualTo(hotelInDb.Country));
            Assert.That(result.Email, Is.EqualTo(hotelInDb.Email));
            Assert.That(result.PhoneNumber, Is.EqualTo(hotelInDb.PhoneNumber));
            Assert.That(result.Description, Is.EqualTo(hotelInDb.Description));
            Assert.That(result.NumberOfRooms, Is.EqualTo(hotelInDb.NumberOfRooms));
        });
    }

    [Test]
    public async Task EditHotelByFormModel_ShoulEditHotelCorrect()
    {
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Original Name",
            Address = "Original Address",
            Description = "Original Description",
            CategoryId = 1,
            City = "Original City",
            Country = "Original Country",
            MaximumCapacity = 100,
            NumberOfRooms = 50,
            Email = "original@example.com",
            PhoneNumber = "1234567890",
            Image = "/images/original.jpg"
        };
        dbContext.Hotels.Add(hotel);
        dbContext.SaveChanges();

        var model = new HotelFormModel
        {
            Name = "Updated Name",
            Address = "Updated Address",
            Description = "Updated Description",
            CategoryId = 2,
            City = "Updated City",
            Country = "Updated Country",
            MaximumCapacity = 200,
            NumberOfRooms = 100,
            Email = "updated@example.com",
            PhoneNumber = "0987654321",
            imageFile = "/images/updated.jpg"
        };

        var imageFileMock = new Mock<IFormFile>();

        repository = new OnlineHotelRoomrentingSystem.Data.Data.Common.Repository(dbContext);
   
        await hotelService.EditHotelByFormModel(model, hotel.Id.ToString(), imageFileMock.Object);

        var updatedHotel = dbContext.Hotels.First(h => h.Id == hotel.Id);
        Assert.Multiple(() =>
        {
            Assert.That(updatedHotel.Name, Is.EqualTo(model.Name));
            Assert.That(updatedHotel.Address, Is.EqualTo(model.Address));
            Assert.That(updatedHotel.Description, Is.EqualTo(model.Description));
            Assert.That(updatedHotel.CategoryId, Is.EqualTo(model.CategoryId));
            Assert.That(updatedHotel.City, Is.EqualTo(model.City));
            Assert.That(updatedHotel.Country, Is.EqualTo(model.Country));
            Assert.That(updatedHotel.MaximumCapacity, Is.EqualTo(model.MaximumCapacity));
            Assert.That(updatedHotel.NumberOfRooms, Is.EqualTo(model.NumberOfRooms));
            Assert.That(updatedHotel.Email, Is.EqualTo(model.Email));
            Assert.That(updatedHotel.PhoneNumber, Is.EqualTo(model.PhoneNumber));
            Assert.That(updatedHotel.Image, Is.EqualTo(model.imageFile));
        });
    }

    [Test]
    public async Task CreateHotelAsync_ShouldAddHotelToDatabase()
    {
        repository = new OnlineHotelRoomrentingSystem.Data.Data.Common.Repository(dbContext);

        var model = new HotelFormModel
        {
            Name = "Test Hotel",
            Address = "123 Test Street",
            City = "Test City",
            Country = "Test Country",
            Email = "test@example.com",
            PhoneNumber = "1234567890",
            Description = "A hotel for testing purposes.",
            imageFile = "/images/test.jpg",
            NumberOfRooms = 10,
            MaximumCapacity = 20,
            CategoryId = 1
        };

        var hotelManagerId = Guid.NewGuid().ToString();

        var imageFileMock = new Mock<IFormFile>();

        var result = await hotelService.CreateHotelAsync(model, hotelManagerId, imageFileMock.Object);

        var hotelInDb = dbContext.Hotels.FirstOrDefault(h => h.Id.ToString() == result);
        Assert.Multiple(() =>
        {
            Assert.IsNotNull(hotelInDb);
            Assert.That(hotelInDb.Name, Is.EqualTo(model.Name));
            Assert.That(hotelInDb.Address, Is.EqualTo(model.Address));
            Assert.That(hotelInDb.City, Is.EqualTo(model.City));
            Assert.That(hotelInDb.Country, Is.EqualTo(model.Country));
            Assert.That(hotelInDb.Email, Is.EqualTo(model.Email));
            Assert.That(hotelInDb.PhoneNumber, Is.EqualTo(model.PhoneNumber));
            Assert.That(hotelInDb.Description, Is.EqualTo(model.Description));
            Assert.That(hotelInDb.Image, Is.EqualTo(model.imageFile));
            Assert.That(hotelInDb.NumberOfRooms, Is.EqualTo(model.NumberOfRooms));
            Assert.That(hotelInDb.MaximumCapacity, Is.EqualTo(model.MaximumCapacity));
            Assert.That(hotelInDb.CategoryId, Is.EqualTo(model.CategoryId));
            Assert.That(hotelInDb.HotelManagerId, Is.EqualTo(Guid.Parse(hotelManagerId)));
            Assert.That(hotelInDb.IsApproved, Is.False);
        });
    }

    [Test]
    public async Task ApproveHotel_ShouldApproveHotelSuccessfully()
    {
        var hotelId = AgentHotel.Id;
        AgentHotel.IsApproved = false;

        await hotelService.ApproveHotelAsync(hotelId);

        var approvedHotel = await repository.GetByIdAsync<Hotel>(hotelId);
        Assert.Multiple(() =>
        {
            Assert.IsNotNull(approvedHotel);
            Assert.IsTrue(approvedHotel.IsApproved);
        });
    }

    [Test]
    public async Task ApproveHotelAsync_ShouldDoNothingIfHotelIsAlreadyApproved()
    {
        var hotelId = AgentHotel.Id;
        AgentHotel.IsApproved = true;

        await hotelService.ApproveHotelAsync(hotelId);

        var approvedHotel = await repository.GetByIdAsync<Hotel>(hotelId);

        Assert.IsNotNull(approvedHotel);
        Assert.IsTrue(approvedHotel.IsApproved);
    }

    [Test]
    public async Task ApproveHotelAsync_ShouldDoNothingIfHotelDoesNotExist()
    {
        var nonExistentHotelId = Guid.NewGuid();
        await hotelService.ApproveHotelAsync(nonExistentHotelId);

        var hotel = await repository.GetByIdAsync<Hotel>(nonExistentHotelId);

        Assert.IsNull(hotel);
    }

    [Test]
    public async Task GetUnApprovedAsync_ShouldReturnOnlyUnApprovedHotels()
    {
        var result = await hotelService.GetUnApprovedAsync();

        Assert.Multiple(() =>
        {
            Assert.IsNotNull(result);
            var unapprovedHotels = result.ToList();
            Assert.That(unapprovedHotels.Count, Is.EqualTo(1));
            var unapprovedHotel = unapprovedHotels.First();
            Assert.That(unapprovedHotel.Name, Is.EqualTo(UnapprovedHotel.Name));
            Assert.That(unapprovedHotel.Address, Is.EqualTo(UnapprovedHotel.Address));
            Assert.That(unapprovedHotel.City, Is.EqualTo(UnapprovedHotel.City));
            Assert.That(unapprovedHotel.Country, Is.EqualTo(UnapprovedHotel.Country));
            Assert.That(unapprovedHotel.imageFile, Is.EqualTo(UnapprovedHotel.Image));
        });
    }

    [Test]
    public async Task HotelHasRoom_ShouldReturnTrueWhenHotelHasRoom()
    {
        var hotelId = AgentHotel.Id.ToString();

        bool result = await hotelService.HasRoomsAsync(hotelId);

        Assert.IsNotNull(result);
        Assert.True(result);
    }

    [Test]
    public async Task HotelHasRoom_ShouldReturnFalseWhenHotelDoesNotHaveRooms()
    {
        var hotelId = Guid.NewGuid().ToString();

        bool result = await hotelService.HasRoomsAsync(hotelId);

        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task HotelHasRoom_ShouldReturnFalseWhenHotelIdIsStringEmpty()
    {
        var hotelId = string.Empty;

        bool result = await hotelService.HasRoomsAsync(hotelId);

        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task HotelHasRoom_ShouldReturnFalseWhenHotelIdIsNull()
    {
        bool result = await hotelService.HasRoomsAsync(null);

        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task HotelHasReview_ShouldReturnTrueWhenHotelHasReviews()
    {
        var hotelId = AgentHotel.Id.ToString();

        bool result = await hotelService.HotelHasReviewsAsync(hotelId);

        Assert.IsNotNull(result);
        Assert.True(result);
    }

    [Test]
    public async Task HotelHasReview_ShouldReturnFalseWhenHotelDoesNotHaveReview()
    {
        var hotelId = Guid.NewGuid().ToString();

        bool result = await hotelService.HotelHasReviewsAsync(hotelId);

        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task HotelHasReview_ShouldReturnFalseWhenHotelIdIsStringEmpty()
    {
        var hotelId = string.Empty;

        bool result = await hotelService.HotelHasReviewsAsync(hotelId);

        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task HotelHasReview_ShouldReturnFalseWhenHotelIdIsNull()
    {
        bool result = await hotelService.HotelHasReviewsAsync(null);

        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [TearDown]
    public void OneTimeTearDown()
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Dispose();
    }
}
