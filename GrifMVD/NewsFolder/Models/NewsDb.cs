using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrifMVD.NewsFolder.Models
{
    public class NewsDb
    {
        public int Id { get; set; }
        // Заголовок
        [Required]
        public string Title { get; set; } = string.Empty;
        // Описание
        [Required]
        public string Description { get; set; } = string.Empty;
        public ICollection<PhotosDb?> Photos { get; set; } = new List<PhotosDb?>();
        // Время полученное путём парсинга
        [Required]
        public string? ParseTime { get; set; } = string.Empty; 
        // Время создания
        public DateTime CreatedTime { get; set; }

        [FromForm]
        [NotMapped]
        public IFormFileCollection Files { get; set; } = new FormFileCollection();
    }
}
