using GrifMVD.NewsFolder.Models;

namespace GrifMVD.NewsFolder.Interfaces
{
    public interface IHandleNews
    {
        public ICollection<NewsDTO> GetNews();
    }
}
