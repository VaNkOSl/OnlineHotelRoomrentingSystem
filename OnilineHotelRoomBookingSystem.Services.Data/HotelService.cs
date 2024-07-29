namespace OnilineHotelRoomBookingSystem.Services.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Agent;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Home;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Hotel;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Hotel.Enum;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Review;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Room;
using OnlineHotelRoomrentingSystem.Data.Data.Common;
using OnlineHotelRoomrentingSystem.Models;

public class HotelService : IHotelService
{
    private readonly IRepository repository;
    private readonly IReviewService reviewService;

    public HotelService(IRepository _repository, IReviewService _reviewService)
    {
        repository = _repository; 
        reviewService = _reviewService;
    }

    public async Task<IEnumerable<HotelCategoryServiceModel>> AllCategoriesAsync()
    {
        return await repository
             .AllReadOnly<Category>()
             .Select(c => new HotelCategoryServiceModel
             {
                 Id = c.Id,
                 Name = c.Name,
             }).ToListAsync();
    }

    public async Task<HotelQueryServiceModel> AllHotelsAsync(AllHotelsQueryModel model)
    {
        IQueryable<Hotel> hotelQuareble = repository
            .AllReadOnly<Hotel>()
            .Where(h => h.IsApproved == true)
            .AsQueryable();

        if(!string.IsNullOrWhiteSpace(model.Category))
        {
            hotelQuareble = hotelQuareble
                            .Where(h => h.Category.Name == model.Category);
        }

        if(!string.IsNullOrWhiteSpace(model.SearchinString))
        {
            string wildCard = $"%{model.SearchinString.ToLower()}%";

            hotelQuareble = hotelQuareble.
                            Where(x => EF.Functions.Like(x.Name, wildCard) ||
                                       EF.Functions.Like(x.Address, wildCard) ||
                                       EF.Functions.Like(x.City, wildCard) ||
                                       EF.Functions.Like(x.Country, wildCard));
        }

        hotelQuareble = model.HotelSorting switch
        {
            HotelSorting.Newest => hotelQuareble
                                   .OrderByDescending(h => h.CreatedOn),

            HotelSorting.Oldest => hotelQuareble
                                   .OrderBy(h => h.CreatedOn),

            HotelSorting.BestReviews => hotelQuareble
                                    .OrderByDescending(h => h.Reviews.Average(r => r.Rating)),

            HotelSorting.WorstReviews => hotelQuareble
                                    .OrderBy(h => h.Reviews.Average(r => r.Rating)),
            _ => hotelQuareble
        };

        IEnumerable<HotelServiceModel> allHotels = await hotelQuareble
             .Skip((model.CurrentPage - 1) * model.HotelsPerPage)
             .Take(model.HotelsPerPage)
             .Select(h => new HotelServiceModel
             {
                 Id = h.Id.ToString(),
                 Name = h.Name,
                 Address = h.Address,
                 imageFile = h.Image,
                 City = h.City,
                 Country = h.Country,
             })
             .ToArrayAsync();
        int totalHouses = hotelQuareble.Count();

                return new HotelQueryServiceModel()
        {
            TotalHotelsCount = totalHouses,
            Hotels = allHotels
        };
    }

    public async Task<IEnumerable<HotelServiceModel>> AllHotelsByAgentIdAsync(string agentId)
    {
        IEnumerable<HotelServiceModel> hotelManagerHotel = await repository
            .AllReadOnly<Hotel>()
            .Where(h => h.HotelManagerId.ToString() == agentId)
            .Select(h => new HotelServiceModel
            {
                Id = h.Id.ToString(),
                Name = h.Name,
                Address = h.Address,
                imageFile = h.Image,
                City = h.City,
                Country = h.Country,
            }).ToListAsync();

        return hotelManagerHotel;
    }

    public async Task<bool> CategoryExistAsync(int categoryId)
    {
       return await repository
            .AllReadOnly<Category>()
            .AnyAsync(c => c.Id == categoryId);
    }

    public async Task<string> CreateHotelAsync(HotelFormModel model, string hotelManagerId, IFormFile imageFile)
    {
        Hotel hotel = new Hotel()
        {
            Name = model.Name,
            Address = model.Address,
            City = model.City,
            HotelManagerId = Guid.Parse(hotelManagerId),
            Country = model.Country,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Description = model.Description,
            Image = model.imageFile,
            NumberOfRooms = model.NumberOfRooms,
            MaximumCapacity = model.MaximumCapacity,
            CategoryId = model.CategoryId,
        };

        if(hotel != null)
        {
            hotel.IsApproved = false;
            await repository.AddAsync(hotel);
            await repository.SaveChangesAsync();
            return hotel.Id.ToString();
        }

        return string.Empty;
    }

    public async Task DeleteHotelByIdAsync(string hotelId)
    {
        Guid hotelGuid = Guid.Parse(hotelId);

        Hotel hotelToDelete = await repository
            .All<Hotel>()
            .Include(r => r.Reviews)
            .FirstAsync(x => x.Id == hotelGuid);

        if (hotelToDelete != null)
        {
            bool hotelHasReview = await HotelHasReviewsAsync(hotelToDelete.Id.ToString());
            if(hotelHasReview == true)
            {
                await reviewService.DeleteReviewsByHotelIdAsync(hotelToDelete.Id.ToString());
            }
            await repository.DeleteAsync<Hotel>(hotelGuid);
            await repository.SaveChangesAsync();
        }
    }

    public async Task EditHotelByFormModel(HotelFormModel model, string hotelId, IFormFile imageFile)
    {
        Hotel hotel = await repository
        .All<Hotel>()
        .FirstAsync(h => h.Id.ToString() == hotelId);

        if (hotel != null)
        {
            hotel.Name = model.Name;
            hotel.Address = model.Address;
            hotel.Description = model.Description;
            hotel.CategoryId = model.CategoryId;
            hotel.City = model.City;
            hotel.Country = model.Country;
            hotel.MaximumCapacity = model.MaximumCapacity;
            hotel.NumberOfRooms = model.NumberOfRooms;
            hotel.Email = model.Email;
            hotel.Image = model.imageFile;
            hotel.PhoneNumber = model.PhoneNumber;
            hotel.CategoryId = model.CategoryId;

            await repository.SaveChangesAsync();
        }
    }

    public async Task<HotelFormModel> EditHotelByIdAsync(string hotelId)
    {
        Hotel hotel = await repository
             .AllReadOnly<Hotel>()
             .Include(h => h.Category)
             .FirstAsync(x => x.Id.ToString() == hotelId);

        if (hotel != null)
        {
            var categories = await AllCategoriesAsync();

            return new HotelFormModel()
            {
                Name = hotel.Name,
                Address = hotel.Address,
                Description = hotel.Description,
                City = hotel.City,
                Country = hotel.Country,
                MaximumCapacity = hotel.MaximumCapacity,
                NumberOfRooms = hotel.NumberOfRooms,
                Email = hotel.Email,
                imageFile = hotel.Image,
                PhoneNumber = hotel.PhoneNumber,
                CategoryId = hotel.CategoryId,
                Categories = categories
            };
        }
        return null!;
    }

    public async Task<Hotel> GetHotelByIdAsync(string hotelId)
    {
        var hotel = await repository
            .All<Hotel>()
            .Include(h => h.Reviews)
            .Include(r => r.Rooms)
            .Include(a => a.HotelManager)
            .FirstOrDefaultAsync(h => h.Id.ToString() == hotelId);

        if(hotel == null)
        {
            return null!;
        }

        return hotel;
    }

    public async Task<HotelPreDeleteDetailsViewModel> GetHotelForDeleteByIdAsync(string hotelId)
    {
        Hotel hotel = await repository
            .AllReadOnly<Hotel>()
            .FirstAsync(h => h.Id.ToString() == hotelId);

        if(hotel == null)
        {
            return null!;
        }

        return new HotelPreDeleteDetailsViewModel
        {
            Address = hotel.Address,
            City = hotel.City,
            Image = hotel.Image,
            Name = hotel.Name,
        };
    }

    public async Task<IEnumerable<HotelServiceModel>> GetUnApprovedAsync()
    {
        return await repository
            .AllReadOnly<Hotel>()
            .Where(h => h.IsApproved == false)
            .Select(h => new HotelServiceModel
            {
                Address = h.Address,
                City = h.City,
                Country = h.Country,
                imageFile = h.Image,
                Name = h.Name,
                Id = h.Id.ToString()
            }).ToListAsync();
    }

    public async Task ApproveHotelAsync(Guid hotelId)
    {

        var hotel = await repository.GetByIdAsync<Hotel>(hotelId);

        if (hotel != null && hotel.IsApproved == false)
        {
            hotel.IsApproved = true;
            await repository.SaveChangesAsync();
        }
    }
    public async Task<bool> HasHotelManagerWithIdAsync(string hotelId, string currentId)
    {
        Hotel hotel = await repository
             .AllReadOnly<Hotel>()
             .Include(h => h.HotelManager)
             .FirstAsync(h => h.Id.ToString() == hotelId);

        return hotel.HotelManagerId.ToString() == currentId;
    }

    public async Task<HotelDetailsViewModel> HotelDetailsByIdAsync(string hotelId)
    {
        Hotel hotel = await repository
            .AllReadOnly<Hotel>()
            .Include(x => x.Category)
            .Include(x => x.Reviews)
            .Include(r => r.Rooms)
            .Include(x => x.HotelManager)
            .ThenInclude(x => x.User)
            .FirstAsync(x => x.Id.ToString() == hotelId);

        return new HotelDetailsViewModel
        {
            Id = hotel.Id.ToString(),
            Name = hotel.Name,
            PhoneNumber = hotel.PhoneNumber,
            Email = hotel.Email,
            Address = hotel.Address,
            City = hotel.City,
            Country = hotel.Country,
            Description = hotel.Description,
            imageFile = hotel.Image,
            CategoryId = hotel.CategoryId,
            MaximumCapacity = hotel.MaximumCapacity,
            NumberOfRooms = hotel.NumberOfRooms,
            CategoryName = hotel.Category.Name,
            HotelManager = new AgentServiceModel
            {
                FullName = $"{hotel.HotelManager.FirstName} {hotel.HotelManager.MiddleName} {hotel.HotelManager.LastName}",
                PhoneNumber = hotel.HotelManager.PhoneNumber,
                Email = hotel.HotelManager.User.Email!
            },
            Rooms = hotel.Rooms.Select(r => new RoomServiceModel
            {
                Id = r.Id.ToString(),
                HotelId = r.HotelId.ToString(),
                HotelName = r.Hotel.Name,
                imageFile = r.Image,
                IsAvaible = r.IsAvaible,
                PricePerNight = r.PricePerNight
            }).ToList(),
            Reviews = hotel.Reviews.Select(r => new HotelReviewViewModel
            {
                Content = r.Content,
                Rating = r.Rating,
                ReviewDate = r.ReviewDate,
            }).ToList()
        };
    }

    public async Task<bool> HotelExistsAsync(string hotelId)
    {
        return await repository.
            AllReadOnly<Hotel>()
            .AnyAsync(x => x.Id.ToString() == hotelId);
    }

    public async Task<bool> HotelWithTheSameEmailExistAsync(string hotelEmail)
    {
        return await repository
            .AllReadOnly<Hotel>()
            .AnyAsync(h => h.Email == hotelEmail);
    }

    public async Task<bool> HotelWithTheSamePhoneNumberExist(string hotelPhoneNumber)
    {
        return await repository.
            AllReadOnly<Hotel>()
            .AnyAsync(h => h.PhoneNumber == hotelPhoneNumber);
    }

    public async Task<IEnumerable<HotelIndexServiceModel>> LastThreeHotelsAsync()
    {
        return await repository.
            AllReadOnly<Hotel>()
            .OrderByDescending(x => x.Id)
            .Take(3)
            .Select(h => new HotelIndexServiceModel
            {
                Id = h.Id,
                Image = h.Image,
                Title = h.Name
            }).ToListAsync();
    }

    public async Task<string> GetHotelNameByIdAsync(string hotelId)
    {

        var hotel = await repository
            .AllReadOnly<Hotel>()
            .Include(r => r.Rooms)
            .Include(r => r.Reviews)
            .Include(c => c.Category)
            .Include(a => a.HotelManager)
            .FirstOrDefaultAsync(h => h.Id.ToString() == hotelId);

        if(hotel == null)
        {
            return "Unknown Hotel";
        }

        return hotel.Name;
    }

    public async Task DeleteRoomsByHotelIdAsync(string hotelId)
    {
        var roomsToDelete = await repository
            .All<Room>()
            .Include(r => r.Reviews)
            .Where(r => r.HotelId.ToString() == hotelId)
            .ToListAsync();

        foreach (var room in roomsToDelete)
        {
           await reviewService.DeleteReviewsByRoomIdAsync(room.Id.ToString());

           await repository.DeleteRange(roomsToDelete);
        }

        await repository.SaveChangesAsync();
    }

    public async Task<bool> HasRoomsAsync(string hotelId)
    {
        var hotel = await repository
            .AllReadOnly<Hotel>()
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Id.ToString() == hotelId);

        return hotel?.Rooms.Any() ?? false;
    }

    public async Task<bool> HotelHasReviewsAsync(string hotelId)
    {
        var hasReviews = await repository
           .All<Review>()
           .AnyAsync(r => r.HotelId.ToString() == hotelId);

        return hasReviews;
    }
}
