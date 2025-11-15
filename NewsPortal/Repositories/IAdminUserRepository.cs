using NewsPortal.Models;

namespace NewsPortal.Repositories
{
    public interface IAdminUserRepository
    {
        Task<AdminUser?> GetByEmailAsync(string email);
        Task AddAsync(AdminUser user);
        Task<bool> AnyAsync();
    }
}
