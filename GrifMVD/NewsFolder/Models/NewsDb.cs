using System.ComponentModel.DataAnnotations;

namespace GrifMVD.NewsFolder.Models
{
    public class NewsDb
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        // Заголовок
        [Required]
        public string Title { get; set; } = string.Empty;
        // Описание
        public string? Description { get; set; } = string.Empty;
        public ICollection<PhotosDb?> Photos { get; set; } = new List<PhotosDb?>();
        // Время полученное путём парсинга
        [Required]
        public DateTime ParseTime { get; set; }
        // Время создания
        public DateTime CreatedTime { get; set; }
    }
}