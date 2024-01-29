using GrifMVD.NewsFolder.Interfaces;
using GrifMVD.NewsFolder.Models;

namespace GrifMVD.NewsFolder.Services
{
    public class ScrapingService : IScraping
    {
        public Task<ICollection<NewsDTO>> ScrapingWebPage()
        {
            throw new NotImplementedException();
        }
    }
}
