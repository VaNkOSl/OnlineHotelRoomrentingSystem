namespace OnilineHotelRoomBookingSystem.Services.Data;

using Microsoft.EntityFrameworkCore;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Admin.User;
using OnlineHotelRoomrentingSystem.Data.Data.Common;
using OnlineHotelRoomrentingSystem.Models;
using System.Collections.Generic;

public class UserService : IUserService
{
    private readonly IRepository repository;

    public UserService(IRepository _repository)
    {
        repository = _repository; 
    }

    public async Task<IEnumerable<UserServiceModel>> AllUsersAsync()
    {
        return await repository.AllReadOnly<ApplicationUser>()
           .Include(u => u.Agent)
           .Select(u => new UserServiceModel()
           {
               Email = u.Email,
               FullName = $"{u.FirstName} {u.LastName}",
               PhoneNumber = u.Agent != null ? u.Agent.PhoneNumber : null,
               IsAgent = u.Agent != null
           })
           .ToListAsync();
    }

    public async Task<string> GetUserFullNameAsync(string userId)
    {
        ApplicationUser? user = await repository
            .AllReadOnly<ApplicationUser>()
            .FirstOrDefaultAsync(a => a.Id.ToString() == userId);

        if(user == null)
        {
            return string.Empty;
        }

        return $"{user.FirstName} {user.LastName}";
    }
}
