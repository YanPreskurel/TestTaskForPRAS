using Microsoft.EntityFrameworkCore;
using NewsPortal.Data;
using NewsPortal.Models;

namespace NewsPortal.Repositories
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly AppDbContext _context;

        public AdminUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdminUser?> GetByEmailAsync(string email)
        {
            return await _context.AdminUsers.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(AdminUser user)
        {
            _context.AdminUsers.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AnyAsync()
        {
            return await _context.AdminUsers.AnyAsync();
        }
    }
}
