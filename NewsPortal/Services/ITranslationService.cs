using System.Net.Http;
using System.Text.Json;
using System.Web;

namespace NewsPortal.Services
{
    public interface ITranslationService
    {
        Task<string> TranslateAsync(string text, string fromLanguage, string toLanguage);
    }
}
