using GrifMVD.NewsFolder.Models;

namespace GrifMVD.NewsFolder.Interfaces
{
    public interface IScraping
    {
        public ICollection<NewsDb> ScrapingWebPage();
    }
}
