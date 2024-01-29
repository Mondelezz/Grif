namespace GrifMVD.NewsFolder.Models
{
    public class NewsDTO
    {
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string ParseTime { get; set; } = string.Empty;
        public ICollection<PhotosDTO?> Photos { get; set; } = new List<PhotosDTO?>();
    }
}
