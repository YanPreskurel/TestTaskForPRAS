using Google.Cloud.Translation.V2;
using System.Threading.Tasks;

namespace NewsPortal.Services
{
    public class GoogleTranslationService : ITranslationService
    {
        private readonly TranslationClient _client;

        public GoogleTranslationService()
        {
 
        }

        public async Task<string> TranslateAsync(string text, string fromLanguage, string toLanguage)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var result = await Task.Run(() =>
                _client.TranslateText(text, toLanguage, fromLanguage)
            );

            return result.TranslatedText;
        }
    }
}
