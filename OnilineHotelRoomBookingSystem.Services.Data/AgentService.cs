namespace OnilineHotelRoomBookingSystem.Services.Data;

using Microsoft.EntityFrameworkCore;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Agent;
using OnlineHotelRoomrentingSystem.Data.Data.Common;
using OnlineHotelRoomrentingSystem.Models;
using System.Threading.Tasks;

public class AgentService : IAgentService
{
    private readonly IRepository repository;

    public AgentService(IRepository _repository)
    {
        this.repository = _repository;   
    }



    public async Task CreateAsync(BecomeAgentFormModel model, string userId)
    {
        var agent = new Agent()
        {
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            DateOfBirth = model.DateOfBirth,
            EGN = model.EGN,
            UserId = Guid.Parse(userId),
            IsApproved = false
        };

        if(agent != null)
        {
            await this.repository.AddAsync(agent);
            await this.repository.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByIdAsync(string userId)
    {
        return await this.repository
            .AllReadOnly<Agent>()
            .AnyAsync(a => a.UserId.ToString() == userId);
    }

    public async Task<string?> GetAgentIdAsync(string userId)
    {
        Agent? agent = await repository.
              AllReadOnly<Agent>()
              .FirstOrDefaultAsync(x => x.UserId.ToString() == userId);

        if(agent == null)
        {
            return null;
        }

        return agent.Id.ToString();
    }

    public async Task<List<AgentServiceModel>> GetUnapprovedAgentsAsync()
    {
        return await repository
            .AllReadOnly<Agent>()
            .Where(h => h.IsApproved == false)
            .Select(a => new AgentServiceModel
            {
                Id = a.Id.ToString(),
                FullName = $"{a.FirstName} {a.MiddleName} {a.LastName}",
                PhoneNumber = a.PhoneNumber,
                Email = a.User.Email
            }).ToListAsync();
    }

    public async Task ApproveAgentAsync(Guid agendId)
    {
        var agent = await repository.GetByIdAsync<Agent>(agendId);

        if (agent != null && agent.IsApproved == false)
        {
            agent.IsApproved = true;
            await repository.SaveChangesAsync();
        }
    }

    public async Task<bool> HasHotelWithIdAsync(string? userId, string hotelId)
    {
        var agent = await this.repository
             .AllReadOnly<Agent>()
             .Include(a => a.OwnedHotels)
             .FirstOrDefaultAsync(a => a.UserId.ToString() == userId);

        if(agent == null)
        {
            return false;
        }

        return agent.OwnedHotels.Any(h => h.Id.ToString().ToLower() == hotelId.ToLower());
    }

    public async Task<bool> HasRoomsWithIdAsync(string? userId, string roomId)
    {
       var agent = await this.repository
            .AllReadOnly<Agent>()
            .Include(r => r.OwnedRooms)
            .FirstOrDefaultAsync(r => r.UserId.ToString() == userId);

        if(agent == null)
        {
            return false;
        }

        return agent.OwnedRooms.Any(r => r.Id.ToString().ToLower() == roomId.ToLower());
    }

    public async Task<bool> UserWithEgnExistAsync(string egn)
    {
        return await this.repository
            .AllReadOnly<Agent>()
            .AnyAsync(a => a.EGN == egn);
    }

    public async Task<bool> UserWithPhoneNumberExistAsync(string phoneNumber)
    {
        return await this.repository
            .AllReadOnly<Agent>()
            .AnyAsync(a => a.PhoneNumber == phoneNumber);
    }

    public async Task<bool> IsAgentApprovedAsync(string agentId)
    {
        var agent = await repository.
            AllReadOnly<Agent>()
            .FirstAsync(a => a.UserId.ToString() == agentId);

        if(agent != null && agent.IsApproved == true)
        {
            return true; 
        }

        return false;
    }
}
