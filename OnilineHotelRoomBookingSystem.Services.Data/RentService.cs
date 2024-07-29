namespace OnilineHotelRoomBookingSystem.Services.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Admin;
using OnlineHotelRoomrentingSystem.Data.Data.Common;
using OnlineHotelRoomrentingSystem.Models;

public class RentService : IRentService
{
    private readonly IRepository repository;

    public RentService(IRepository _repository)
    {
        repository = _repository;  
    }
    public async Task<IEnumerable<RentServiceModel>> AllRentsAsync()
    {
        return await repository
            .AllReadOnly<Room>()
            .Where(r => r.RenterId != null)
            .Include(a => a.Agent)
            .Include(r => r.Renter)
            .Select(r => new RentServiceModel
            {
                AgentEmail = r.Agent.User.Email,
                AgentFullName = $"{r.Agent.User.FirstName} {r.Agent.User.LastName}",
                imageFile = r.Image,
                RoomTitle = r.RoomType.Name,
                RenterEmail = r.Renter != null ? r.Renter.Email : string.Empty,
                RenterFullName = r.Renter != null ? $"{r.Renter.FirstName} {r.Renter.LastName}" : string.Empty
            })
            .ToListAsync();
    }
}
