using System.Net.Http;
using System.Text.Json;
using System.Web;

namespace NewsPortal.Services
{
    public interface ITranslationService
    {
        Task<string> TranslateAsync(string text, string fromLanguage, string toLanguage);
    }

    public class GoogleTranslateService : ITranslationService
    {
        private readonly HttpClient _http;

        public GoogleTranslateService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> TranslateAsync(string text, string fromLanguage, string toLanguage)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            var urlText = HttpUtility.UrlEncode(text);
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={urlText}";

            var response = await _http.GetStringAsync(url);

            // Парсим JSON-массив [[["translatedText", ...]]]
            var json = JsonSerializer.Deserialize<JsonElement>(response);
            var translated = json[0][0][0].GetString();

            return translated ?? text;
        }
    }
}
