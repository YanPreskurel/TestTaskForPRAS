using NewsPortal.Models;

namespace NewsPortal.Services
{
    public interface IAdminUserService
    {
        Task<AdminUser?> AuthenticateAsync(string email, string password);
        Task<bool> AnyUsersAsync();
        Task CreateAdminAsync(string email, string name, string password);
    }
}
