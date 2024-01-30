using GrifMVD.NewsFolder.Models;

namespace GrifMVD.NewsFolder.Interfaces
{
    public interface IScraping
    {
        public Task<ICollection<NewsDTO>> ScrapingWebPageAsync();
    }
}
