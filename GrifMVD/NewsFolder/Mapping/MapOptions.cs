using AutoMapper;
using GrifMVD.NewsFolder.Models;

namespace GrifMVD.NewsFolder.Mapping
{
    public class MapOptions : Profile
    {
        public MapOptions()
        {
            CreateMap<NewsDb, NewsDTO>().ReverseMap();
        }
    }
}
