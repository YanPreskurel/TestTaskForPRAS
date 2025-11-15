using NewsPortal.Models;
using NewsPortal.Repositories;
using System.Threading.Tasks;

namespace NewsPortal.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _repository;
        private readonly ITranslationService _translationService;
        private readonly ITranslationService _translator;

        public NewsService(INewsRepository repository, ITranslationService translationService, ITranslationService translator)
        {
            _repository = repository;
            _translationService = translationService;
            _translator = translator;
        }

        public async Task<IEnumerable<News>> GetPagedNewsAsync(int page, int pageSize, string language)
            => await _repository.GetAllAsync(page, pageSize, language);

        public async Task<News?> GetByIdAsync(int id, string language)
            => await _repository.GetByIdAsync(id, language);


        public async Task CreateAsync(News news, NewsTranslation translation)
        {
            news.CreatedAt = DateTime.Now;
            await _repository.AddAsync(news, translation);

            // Автоматически создаём перевод
            string targetLanguage = translation.Language == "ru" ? "en" : "ru";

            var translated = new NewsTranslation
            {
                NewsId = news.Id,
                Language = targetLanguage,
                Title = await _translator.TranslateAsync(translation.Title, translation.Language, targetLanguage),
                Subtitle = translation.Subtitle != null
                    ? await _translator.TranslateAsync(translation.Subtitle, translation.Language, targetLanguage)
                    : null,
                Body = await _translator.TranslateAsync(translation.Body, translation.Language, targetLanguage)
            };

            news.Translations.Add(translated);
            await _repository.UpdateAsync(news, translated);
        }

        public async Task UpdateAsync(News news, NewsTranslation translation)
            => await _repository.UpdateAsync(news, translation);

        public async Task DeleteAsync(int id)
            => await _repository.DeleteAsync(id);

        public async Task<int> GetCountAsync()
            => await _repository.GetCountAsync();

        public async Task<IEnumerable<News>> GetLatestAsync(int count, string language)
            => await _repository.GetLatestAsync(count, language);

        /// <summary>
        /// Метод для добавления перевода к существующей новости, если его нет.
        /// Можно использовать для старых записей.
        /// </summary>
        public async Task EnsureTranslationAsync(News news, string targetLanguage)
        {
            if (news.Translations.Any(t => t.Language == targetLanguage))
                return; // перевод уже есть

            var existingTranslation = news.Translations.FirstOrDefault();
            if (existingTranslation != null)
            {
                var newTranslation = new NewsTranslation
                {
                    NewsId = news.Id,
                    Language = targetLanguage,
                    Title = await _translationService.TranslateAsync(existingTranslation.Title, existingTranslation.Language, targetLanguage),
                    Subtitle = existingTranslation.Subtitle != null
                        ? await _translationService.TranslateAsync(existingTranslation.Subtitle, existingTranslation.Language, targetLanguage)
                        : null,
                    Body = await _translationService.TranslateAsync(existingTranslation.Body, existingTranslation.Language, targetLanguage)
                };

                news.Translations.Add(newTranslation);
                await _repository.AddAsync(news, newTranslation);
            }
        }
    }
}
