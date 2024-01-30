namespace GrifMVD.NewsFolder.Models
{
    public class PhotosDb
    {
        
        public Guid Id { get; set; }
        // Изображение представленное массивом байт
        public string? UrlImage { get; set; } = string.Empty;
        public Guid NewsDbID { get; set; }
        public NewsDb NewsDb { get; set; } = null!;
    }
}
