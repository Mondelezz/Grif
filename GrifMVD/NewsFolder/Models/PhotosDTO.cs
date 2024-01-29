namespace GrifMVD.NewsFolder.Models
{
    public class PhotosDTO
    {
        public int Id { get; set; }
        public byte[] Bytes { get; set; } = new byte[0];
        public decimal Size { get; set; }
        public string FileExtension { get; set; } = string.Empty;
    }
}
