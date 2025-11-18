using NewsPortal.Models;
using NewsPortal.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace NewsPortal.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IAdminUserRepository _repository;

        public AdminUserService(IAdminUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<AdminUser?> AuthenticateAsync(string email, string password)
        {
            var user = await _repository.GetByEmailAsync(email);
            if (user == null) return null;

            string hash = ComputeSha256Hash(password);
            return user.PasswordHash == hash ? user : null;
        }

        public async Task<bool> AnyUsersAsync()
        {
            return await _repository.AnyAsync();
        }

        public async Task CreateAdminAsync(string email, string name, string password)
        {
            var user = new AdminUser
            {
                Email = email,
                Name = name,
                PasswordHash = ComputeSha256Hash(password)
            };

            await _repository.AddAsync(user);
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();

            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            
            var sb = new StringBuilder();

            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}
