using NewsPortal.Models;

namespace NewsPortal.Services
{
    public interface INewsService
    {
        Task<IEnumerable<News>> GetPagedNewsAsync(int page, int pageSize, string language);
        Task<News?> GetByIdAsync(int id);
        Task CreateAsync(News news, NewsTranslation translation);
        Task UpdateAsync(News news, NewsTranslation translation);
        Task DeleteAsync(int id);
        Task<int> GetCountAsync();
        Task<IEnumerable<News>> GetLatestAsync(int count, string language);
    }
}
