using NewsPortal.Models;

namespace NewsPortal.Repositories
{
    public interface INewsRepository
    {
        Task<IEnumerable<News>> GetAllAsync(int page, int pageSize, string language);
        Task<int> GetCountAsync();
        Task<News?> GetByIdAsync(int id);
        Task<IEnumerable<News>> GetLatestAsync(int count, string language);

        Task AddAsync(News news, NewsTranslation translation);
        Task UpdateAsync(News news, NewsTranslation translation);
        Task DeleteAsync(int id);
    }
}
