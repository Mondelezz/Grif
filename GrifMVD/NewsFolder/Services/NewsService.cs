using AutoMapper;
using GrifMVD.NewsFolder.Data;
using GrifMVD.NewsFolder.Interfaces;
using GrifMVD.NewsFolder.Models;
using Microsoft.EntityFrameworkCore;

namespace GrifMVD.NewsFolder.Services
{
    public class NewsService : IHandleNews
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public NewsService(DataContext dataContext, IMapper mapper) 
        { 
            _dataContext= dataContext;
            _mapper= mapper;
        }
        public ICollection<NewsDTO> GetNews()
        {
            ICollection<NewsDb> newsDb = new List<NewsDb>();
            foreach (var item in _dataContext.News.Include(u => u.Photos))
            {
                newsDb.Add(item);
            }
            ICollection<NewsDTO> newsDTO = _mapper.Map<ICollection<NewsDTO>>(newsDb);
            newsDTO = newsDTO.OrderByDescending(u => u.ParseTime).ToList();
            return newsDTO;
        }
    }
}
