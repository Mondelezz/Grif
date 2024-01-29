namespace GrifMVD.NewsFolder.Models
{
    public class PhotosDb
    {
        
        public int Id { get; set; }
        // Изображение представленное массивом байт
        public byte[] Bytes { get; set; } = new byte[0];
        // Размер изображения
        public decimal Size { get; set; }  
        // Расширение изображения
        public string FileExtension { get; set; } = string.Empty;
        public int NewsDbID { get; set; }
        public NewsDb NewsDb { get; set; } = null!;
    }
}
