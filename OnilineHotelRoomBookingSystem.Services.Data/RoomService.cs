namespace OnilineHotelRoomBookingSystem.Services.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Agent;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Review;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Room;
using OnlineHotelRoomrentingSystem.Data.Data.Common;
using OnlineHotelRoomrentingSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RoomService : IRoomService
{
    private readonly IRepository repository;
    private readonly IHotelService hotelService;
    private readonly IReviewService reviewService;

    public RoomService(IRepository _repository, IHotelService _hotelService,
                       IReviewService _reviewService)
    {
        repository = _repository;
        hotelService = _hotelService;
        reviewService = _reviewService;
    }

    public async Task<IEnumerable<RoomServiceModel>> AllRoomsByUsersIdAsync(string userId)
    {
        IEnumerable<RoomServiceModel> roomManager = await repository
             .AllReadOnly<Room>()
             .Where(r => r.RenterId.ToString() == userId)
             .Select(r => new RoomServiceModel
             {
                RoomType = r.RoomType.Name,
                HotelName = r.Hotel.Name,
                imageFile = r.Image,
                IsAvaible = r.IsAvaible,
                PricePerNight = r.PricePerNight,
             }).ToListAsync();

        return roomManager;
    }

    public async Task<IEnumerable<RoomServiceModel>> AllRoomsByAgentIdAsync(string agentId)
    {
        IEnumerable<RoomServiceModel> allRoomByAgentId =  await repository
            .AllReadOnly<Room>()
            .Where(r => r.AgentId.ToString() == agentId)
            .Select(r => new RoomServiceModel
            {
                Id = r.Id.ToString(),
                IsAvaible = r.IsAvaible,
                HotelName = r.Hotel.Name,
                imageFile = r.Image,
                PricePerNight = r.PricePerNight,
            }).ToListAsync();

        return allRoomByAgentId;
    }

    public async Task<IEnumerable<RoomTypeServiceModel>> AllRoomTypesAsync()
    {
        return await repository
            .AllReadOnly<RoomType>()
            .Select(r => new RoomTypeServiceModel
            {
                Id = r.Id,
                Name = r.Name,
            }).ToListAsync();
    }

    public async Task<string> CreateRoomAsync(RoomFormModel model, string agentId, IFormFile imageFile)
    {
        var room = new Room
        {
            Description = model.Description,
            HotelId = Guid.Parse(model.HotelId),
            AgentId = Guid.Parse(agentId),
            IsAvaible = true,
            PricePerNight = model.PricePerNight,
            Image = model.imageFile,
            RoomNumber = model.RoomNumber,
            RoomId = model.RoomTypeId,
        };

        if(room != null)
        {
            await repository.AddAsync(room);

            var hotel = await hotelService.GetHotelByIdAsync(model.HotelId);

            if(hotel != null)
            {
                hotel.NumberOfRooms += 1;
                await repository.UpdateAsync(hotel);
            }
        }

        await repository.SaveChangesAsync();
        return room.Id.ToString();
    }

    public async Task DeleteRoomByIdAsync(string roomId)
    {
        Guid roomToGuid = Guid.Parse(roomId);

        Room room = await repository
            .All<Room>()
            .Include(r => r.Reviews)
            .Include(h => h.Hotel)
            .FirstAsync(r => r.Id == roomToGuid);

        if(room != null)
        {
            bool roomHasReview = await RoomHasReviewAsync(room.Id.ToString());

            if(roomHasReview == true)
            {
                await reviewService.DeleteReviewsByRoomIdAsync(room.Id.ToString());
            }

            await repository.DeleteAsync<Room>(roomToGuid);
            await repository.SaveChangesAsync();

            var hotel = await hotelService.GetHotelByIdAsync(room.HotelId.ToString());

            if (hotel != null)
            {
                hotel.NumberOfRooms -= 1;
                await repository.UpdateAsync(hotel);
                await repository.SaveChangesAsync();
            }
        }
    }

    public async Task<RoomPreDeleteViewModel> DeteleRoomFormModelByIdAsync(string roomId)
    {
        Room room = await repository
            .AllReadOnly<Room>()
            .Include(h => h.Hotel)
            .FirstAsync(r => r.Id.ToString() == roomId);

        if(room == null)
        {
            return null!;
        
        }
        return new RoomPreDeleteViewModel
        {
            HotelName = room.Hotel.Name,
            imageFile = room.Image!,
            PricePerNight = room.PricePerNight,
            HotelId = room.HotelId.ToString()
        };
    }

    public async Task EditRoomByFormModel(RoomFormModel model, string roomId, IFormFile imageFile)
    {
        Room room = await repository
            .All<Room>()
            .FirstAsync(r => r.Id.ToString() == roomId);

        if(room != null)
        {
            room.RoomNumber = model.RoomNumber;
            room.PricePerNight = model.PricePerNight;
            room.Description = model.Description;
            room.HotelId = Guid.Parse(model.HotelId);
            room.RoomId = model.RoomTypeId;

            await repository.SaveChangesAsync();
        }
    }

    public async Task<RoomFormModel> EditRoomByIdAsync(string roomId)
    {
        Room room = await repository
             .AllReadOnly<Room>()
             .Include(rt => rt.RoomType)
             .FirstAsync(r => r.Id.ToString() == roomId);

        if(room != null)
        {
            var roomTypes = await AllRoomTypesAsync();
            var hotels = await GetAllHotelsAsync();

            return new RoomFormModel
            {
                PricePerNight = room.PricePerNight,
                Description = room.Description,
                imageFile = room.Image,
                RoomTypes = roomTypes,
                RoomNumber = room.RoomNumber,
                HotelId = room.HotelId.ToString(),
                Hotels = hotels,
            };
        }

        return null!;
    }

    public async Task<IEnumerable<HotelRoomViewModel>> GetAllHotelsAsync()
    {
        return await repository
            .AllReadOnly<Hotel>()
            .Select(h => new HotelRoomViewModel
            {
                Id = h.Id.ToString(),
                Name = h.Name,
            }).ToListAsync();

    }

    public async Task<IEnumerable<RoomServiceModel>> GetAllRoomsAsync()
    {
        return await repository
            .All<Room>()
            .Select(r => new RoomServiceModel
            {
                Id = r.Id.ToString(),
                HotelName = r.Hotel.Name,
                imageFile = r.Image,
                IsAvaible = r.IsAvaible,
                PricePerNight = r.PricePerNight,
                RoomType = r.RoomType.Name,
                HotelId = r.HotelId.ToString(),
            }).ToListAsync();
    }

    public async Task<RoomServiceModel> GetRoomByIdAsync(string roomId)
    {
        Room room = await repository.AllReadOnly<Room>()
            .Include(x => x.Hotel)
            .Include(x => x.Agent)
            .Include(r => r.Renter)
            .Include(r => r.RoomType)
            .FirstAsync(x => x.Id.ToString() == roomId);

        if(room.Id == null)
        {
            throw new ArgumentException("Room does not exist.");
        }

        return new RoomServiceModel
        {
            Id = roomId,

            HotelId = room.HotelId.ToString(),
            HotelName = room.Hotel.Name,
            PricePerNight = room.PricePerNight,
            imageFile = room.Image,
            IsAvaible = room.IsAvaible,
            RoomType = room.RoomType.Name,
        };
    }

    public async Task<List<RoomServiceModel>> GetRoomsByHotelIdAsync(string hotelId)
    {
        return await repository
       .AllReadOnly<Room>()
       .Where(r => r.HotelId.ToString() == hotelId)
       .Select(r => new RoomServiceModel
       {
           Id = r.Id.ToString(),
           HotelId = r.HotelId.ToString(),
           IsAvaible = r.IsAvaible,
           imageFile = r.Image,
           PricePerNight = r.PricePerNight,
           HotelName = r.Hotel.Name,
           RoomType = r.RoomType.Name
       })
       .ToListAsync();
    }

    public async Task<bool> HasRoomAgenWithIdAsync(string userId, string roomId)
    {
        Room room = await repository
         .AllReadOnly<Room>()
         .FirstAsync(r => r.Id.ToString() == roomId);

        return room.AgentId.ToString() == userId;
    }

    public async Task<bool> IsRentedAsync(string roomId)
    {
        var room = await repository
             .AllReadOnly<Room>()
             .FirstOrDefaultAsync(r => r.Id.ToString() == roomId);

        if(room == null)
        {
            return false;
        }

        room.IsAvaible = false;
        return room.RenterId != null;
    }

    public async Task<bool> IsRentedByUserWithIdAsync(string userId, string roomId)
    {
        var isRentedByUser = await repository
             .AllReadOnly<Room>()
             .FirstOrDefaultAsync(r => r.Id.ToString() == roomId && r.RenterId.ToString() == userId);

        return isRentedByUser != null;
    }

    public async Task LeaveAsync(string userId, string roomId, string hotelId)
    {
        var leaveRoom = await repository
            .All<Room>()
            .FirstOrDefaultAsync(r => r.Id.ToString() == roomId && r.HotelId.ToString() == hotelId);

        if(leaveRoom != null)
        {
            leaveRoom.RenterId = null;
            leaveRoom.IsAvaible = true;
            await repository.SaveChangesAsync();
        }
    }

    public async Task RentAsync(string roomId, string userId, string hotelId)
    {
        var rentRoom = await repository
            .All<Room>()
            .FirstOrDefaultAsync(r => r.Id.ToString() == roomId && r.HotelId.ToString() == hotelId);

        if (rentRoom == null)
        {
            throw new ArgumentException("Room does not exist.");
        }

        if (rentRoom.RenterId != null)
        {
            throw new ArgumentException("Room is already rented.");
        }

        rentRoom.IsAvaible = false;
        rentRoom.RenterId = Guid.Parse(userId);

        await repository.SaveChangesAsync();
    }

    public async Task<RoomDetailsViewModel> RoomDetailsByIdAsync(string roomId)
    {
        Room room = await repository
                .All<Room>()
                .Include(h => h.Hotel)
                .Include(a => a.Agent)
                .Include(r => r.Reviews)
                .Include(rt => rt.RoomType)
                .Include(r => r.Renter)
                .FirstAsync(r => r.Id.ToString() == roomId);

        if(room == null)
        {
            throw new ArgumentException("Room with the provided id not found");
        }

        var roomServiceModel = new RoomDetailsViewModel
        {
            Id = roomId,
            PricePerNight = room.PricePerNight,
            RoomType = room.RoomType?.Name ?? "Unknown",
            HotelName = room.Hotel?.Name ?? "Unknown",
            imageFile = room.Image,
            IsAvaible = room.IsAvaible,
            Description = room.Description,
            HotelId = room.HotelId.ToString(),
            Agent = new AgentServiceModel
            {
                FullName = $"{room.Agent.FirstName} {room.Agent.MiddleName} {room.Agent.LastName}",
                PhoneNumber = room.Agent.PhoneNumber,
                Email = room.Agent.User?.Email ?? "Unknown",
            },
            Reviews = room.Reviews.Select(r => new RoomReviewViewModel
            {
                Content = r.Content,
                Rating = r.Rating,
                ReviewDate = r.ReviewDate,
            }).ToList()
        };

        return roomServiceModel;
    }

    public async Task<bool> RoomNumberAlreadyExistInTheHotelAsync(int roomNumber, string hotelId)
    {
        Guid hotelGuid;
        if (!Guid.TryParse(hotelId, out hotelGuid))
        {
            throw new ArgumentException("Invalid hotel ID format", nameof(hotelId));
        }

        return await repository
             .AllReadOnly<Room>()
             .AnyAsync(r => r.RoomNumber == roomNumber && r.HotelId == hotelGuid);
    }

    public async Task<bool> RoomTypeExistAsync(int roomType)
    {
       return await repository
            .AllReadOnly<RoomType>()
            .AnyAsync(r => r.Id ==  roomType);
    }

    public async Task<bool> RoomExistsAsync(string roomId)
    {
        return await repository
            .AllReadOnly<Room>()
            .AnyAsync(r => r.Id.ToString() == roomId);
    }

    public async Task<bool> RoomHasReviewAsync(string roomId)
    {
        return await repository
            .All<Review>()
            .AnyAsync(r => r.RoomId.ToString() == roomId);
    }

    
}
