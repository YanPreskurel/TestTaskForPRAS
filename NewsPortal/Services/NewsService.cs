using NewsPortal.Models;
using NewsPortal.Repositories;
using System.Threading.Tasks;

namespace NewsPortal.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _repository;
        private readonly ITranslationService _translator;

        public NewsService(INewsRepository repository, ITranslationService translator)
        {
            _repository = repository;
            _translator = translator;
        }

        public async Task<IEnumerable<News>> GetPagedNewsAsync(int page, int pageSize, string language)
            => await _repository.GetAllAsync(page, pageSize, language);

        public async Task<News?> GetByIdAsync(int id)
            => await _repository.GetByIdAsync(id);


        public async Task CreateAsync(News news, NewsTranslation translation)
        {
            news.CreatedAt = DateTime.Now;
            await _repository.AddAsync(news, translation);

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

        public async Task UpdateAsync(News news, NewsTranslation editedTranslation)
        {
            await _repository.UpdateAsync(news, editedTranslation);

            string sourceLang = editedTranslation.Language;
            string targetLang = sourceLang == "ru" ? "en" : "ru";

            var secondTranslation = news.Translations
                .FirstOrDefault(t => t.Language == targetLang);

            string translatedTitle =
                await _translator.TranslateAsync(editedTranslation.Title, sourceLang, targetLang);

            string? translatedSubtitle = !string.IsNullOrEmpty(editedTranslation.Subtitle)
                ? await _translator.TranslateAsync(editedTranslation.Subtitle, sourceLang, targetLang)
                : null;

            string translatedBody =
                await _translator.TranslateAsync(editedTranslation.Body, sourceLang, targetLang);

            if (secondTranslation == null)
            {
                secondTranslation = new NewsTranslation
                {
                    NewsId = news.Id,
                    Language = targetLang,
                    Title = translatedTitle,
                    Subtitle = translatedSubtitle,
                    Body = translatedBody
                };

                news.Translations.Add(secondTranslation);

                await _repository.UpdateAsync(news, secondTranslation);
            }
            else
            {
                secondTranslation.Title = translatedTitle;
                secondTranslation.Subtitle = translatedSubtitle;
                secondTranslation.Body = translatedBody;

                await _repository.UpdateAsync(news, secondTranslation);
            }
        }

        public async Task DeleteAsync(int id)
            => await _repository.DeleteAsync(id);

        public async Task<int> GetCountAsync()
            => await _repository.GetCountAsync();

        public async Task<IEnumerable<News>> GetLatestAsync(int count, string language)
            => await _repository.GetLatestAsync(count, language);
    }
}
