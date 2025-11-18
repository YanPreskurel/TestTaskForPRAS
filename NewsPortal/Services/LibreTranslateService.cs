using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace NewsPortal.Services
{
    public class LibreTranslateService : ITranslationService
    {
        private readonly HttpClient _http;
        private readonly string _apiUrl;

        public LibreTranslateService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiUrl = config["LibreTranslate:Url"] ?? "http://localhost:5000/translate";
        }

        public async Task<string> TranslateAsync(string text, string fromLanguage, string toLanguage)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var request = new
            {
                q = text,
                source = fromLanguage,
                target = toLanguage,
                format = "text"
            };

            var response = await _http.PostAsJsonAsync(_apiUrl, request);
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            return doc.RootElement.GetProperty("translatedText").GetString() ?? text;
        }
    }
}
