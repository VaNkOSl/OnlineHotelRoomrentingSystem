using OnlineHotelRoomBookingSystem.Web.ViewModels.Agent;

namespace OnilineHotelRoomBookingSystem.Services.Data.Contacts;

public interface IAgentService
{
    Task<bool> ExistsByIdAsync(string userId);
    Task<bool> UserWithPhoneNumberExistAsync(string phoneNumber);
    Task<bool> UserWithEgnExistAsync(string egn);
    Task<bool> HasHotelWithIdAsync(string? userId,string hotelId);
    Task<bool> HasRoomsWithIdAsync(string? userId, string roomId);
    Task<string?> GetAgentIdAsync(string userId);
    Task<List<AgentServiceModel>> GetUnapprovedAgentsAsync();
    Task ApproveAgentAsync(Guid agendId);
    Task CreateAsync(BecomeAgentFormModel model,string userId);
    Task<bool> IsAgentApprovedAsync(string agentId);
}
