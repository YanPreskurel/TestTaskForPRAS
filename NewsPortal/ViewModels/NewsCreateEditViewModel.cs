using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace NewsPortal.ViewModels
{
    public class NewsCreateEditViewModel : IValidatableObject
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must be less than 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Subtitle must be less than 300 characters")]
        public string? Subtitle { get; set; }

        [Required(ErrorMessage = "Body is required")]
        public string Body { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        public IFormFile? ImageFile { get; set; }

        [Required]
        [StringLength(5)]
        public string Language { get; set; } = "ru";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (Id == null) 
            {
                if (ImageFile == null && string.IsNullOrEmpty(ImagePath))
                    results.Add(new ValidationResult("Image is required.", new[] { nameof(ImageFile) }));
            }

            var lang = (Language ?? "ru").Trim().ToLowerInvariant();
            var allowed = new[] { "ru", "en" };

            if (!allowed.Contains(lang))
            {
                results.Add(new ValidationResult("Unsupported language. Allowed: ru, en.", new[] { nameof(Language) }));
                return results;
            }

            CheckTextField(Title, lang, nameof(Title), results);

            if (!string.IsNullOrEmpty(Subtitle))
                CheckTextField(Subtitle, lang, nameof(Subtitle), results);

            CheckTextField(Body, lang, nameof(Body), results);

            return results;
        }

        private void CheckTextField(string text, string lang, string fieldName, List<ValidationResult> results)
        {
            var metrics = LanguageUtils.AnalyzeTextLanguageMetrics(text);

            const double requiredShare = 0.50;
            const double mixedThreshold = 0.30;

            bool isMostlyCyrillic = metrics.CyrillicLettersShare >= requiredShare;
            bool isMostlyLatin = metrics.LatinLettersShare >= requiredShare;
            bool mixed = metrics.CyrillicLettersShare > mixedThreshold && metrics.LatinLettersShare > mixedThreshold;

            if (lang == "ru")
            {
                if (mixed)
                    results.Add(new ValidationResult("Содержит смешение кириллицы и латиницы — используйте русский язык.", new[] { fieldName }));
                else if (!isMostlyCyrillic)
                    results.Add(new ValidationResult("Похоже, что текст не на русском.", new[] { fieldName }));
            }
            else if (lang == "en")
            {
                if (mixed)
                    results.Add(new ValidationResult("Contains mixed Latin and Cyrillic characters — use English.", new[] { fieldName }));
                else if (!isMostlyLatin)
                    results.Add(new ValidationResult("Text seems not English.", new[] { fieldName }));
            }
        }
    }

    internal static class LanguageUtils
    {
        private static readonly Regex CyrillicLetters = new(@"\p{IsCyrillic}", RegexOptions.Compiled);
        private static readonly Regex LatinLetters = new(@"[A-Za-z]", RegexOptions.Compiled);
        private static readonly Regex Letters = new(@"\p{L}", RegexOptions.Compiled);

        public static LanguageMetrics AnalyzeTextLanguageMetrics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new LanguageMetrics();

            int cyr = 0, lat = 0, totalLetters = 0;

            foreach (Match m in Letters.Matches(text))
            {
                totalLetters++;
                if (CyrillicLetters.IsMatch(m.Value)) cyr++;
                else if (LatinLetters.IsMatch(m.Value)) lat++;
            }

            double cyrShare = totalLetters > 0 ? (double)cyr / totalLetters : 0;
            double latShare = totalLetters > 0 ? (double)lat / totalLetters : 0;

            return new LanguageMetrics
            {
                CyrillicLetters = cyr,
                LatinLetters = lat,
                TotalLetters = totalLetters,
                CyrillicLettersShare = cyrShare,
                LatinLettersShare = latShare
            };
        }
    }

    internal sealed class LanguageMetrics
    {
        public int CyrillicLetters { get; set; }
        public int LatinLetters { get; set; }
        public int TotalLetters { get; set; }
        public double CyrillicLettersShare { get; set; }
        public double LatinLettersShare { get; set; }
    }
}
