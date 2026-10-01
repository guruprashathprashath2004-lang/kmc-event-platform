using KMC_Client_Guruprashath.Models;

namespace KMC_Client_Guruprashath.Services
{
    public interface IApiService
    {
        // Public / read (Admin-approved + published events only)
        Task<List<CategoryVM>> GetCategoriesAsync();
        Task<List<EventVM>> GetUpcomingEventsAsync();
        Task<List<EventVM>> GetFeaturedEventsAsync();
        Task<List<EventVM>> GetEventsByCategoryAsync(int categoryId);
        Task<List<EventVM>> GetAllEventsAsync();
        Task<List<EventVM>> SearchEventsAsync(string keyword);
        Task<EventVM?> GetEventByIdAsync(int id);

        // Auth (all roles)
        Task<(bool Success, string? Token, string? FullName, string? Role, string Message)> LoginAsync(string username, string password);
        Task<(bool Success, string? Token, string? FullName, string? Role, string Message)> RegisterAccountAsync(RegisterVM vm);

        // Citizen - requires a logged-in Citizen token
        Task<(bool Success, string Message)> RegisterForEventAsync(RegistrationVM vm, string token);
        Task<List<RegistrationVM>> GetMyRegistrationsAsync(string token);

        // Organizer - requires a logged-in Organizer token, restricted to their own events
        Task<List<EventVM>> GetMyEventsAsync(string token);
        Task<(bool Success, string Message)> AddEventAsync(EventWriteVM vm, string token);
        Task<(bool Success, string Message)> UpdateEventAsync(EventWriteVM vm, string token);
        Task<bool> DeleteEventAsync(int id, string token);
        Task<(string? Url, string Message)> UploadEventImageAsync(Stream fileStream, string fileName, string contentType, string token);
        Task<List<CategoryVM>> GetCategoriesAdminAsync(string token);

        // Admin - approval queue
        Task<List<EventVM>> GetPendingEventsAsync(string token);
        Task<bool> ApproveEventAsync(int id, string token);
        Task<bool> RejectEventAsync(int id, string token);

        // Admin - user management (identify + manage Citizens/Organizers/Admins)
        Task<List<UserVM>> GetUsersAsync(string token);
        Task<(bool Success, string Message)> DeleteUserAsync(int userId, string token);
    }
}
