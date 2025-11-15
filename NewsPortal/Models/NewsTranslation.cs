using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Models
{
    public class NewsTranslation
    {
        public int Id { get; set; }

        [Required]
        public int NewsId { get; set; }
        public News News { get; set; }

        // "ru", "en", "de" и т.д.
        [Required]
        [StringLength(5)]
        public string Language { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Subtitle { get; set; }

        [Required]
        public string Body { get; set; } = string.Empty;
    }
}
