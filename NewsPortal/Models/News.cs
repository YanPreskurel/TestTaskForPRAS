using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Models
{
    public class News
    {
        public int Id { get; set; }
        [StringLength(300)]
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }

        // Связь с переводами
        public ICollection<NewsTranslation> Translations { get; set; } = new List<NewsTranslation>();
    }

}
