using Microsoft.EntityFrameworkCore;
using NewsPortal.Data;
using NewsPortal.Models;

namespace NewsPortal.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly AppDbContext _context;

        public NewsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<News>> GetAllAsync(int page, int pageSize, string language)
        {
            return await _context.News
                .Include(n => n.Translations.Where(t => t.Language == language))
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync() =>
            await _context.News.CountAsync();

        public async Task<News?> GetByIdAsync(int id)
        {
            return await _context.News
                .Include(n => n.Translations)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<IEnumerable<News>> GetLatestAsync(int count, string language)
        {
            return await _context.News
                .Include(n => n.Translations.Where(t => t.Language == language))
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task AddAsync(News news, NewsTranslation translation)
        {
            news.Translations.Add(translation);
            _context.News.Add(news);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(News news, NewsTranslation translation)
        {
            _context.NewsTranslations.Update(translation);
            _context.News.Update(news);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var news = await _context.News
                .Include(n => n.Translations)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (news != null)
            {
                _context.News.Remove(news);
                await _context.SaveChangesAsync();
            }
        }
    }
}
