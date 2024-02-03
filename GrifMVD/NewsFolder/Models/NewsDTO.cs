namespace GrifMVD.NewsFolder.Models
{
    public class NewsDTO
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public DateTime ParseTime { get; set; }
        public ICollection<PhotosDTO?> Photos { get; set; } = new List<PhotosDTO?>();
    }
}
