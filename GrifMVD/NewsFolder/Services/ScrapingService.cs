using GrifMVD.NewsFolder.Interfaces;
using GrifMVD.NewsFolder.Models;
using HtmlAgilityPack;
using CsvHelper;
using System.Globalization;
using GrifMVD.NewsFolder.Data;
using AutoMapper;
using System.Collections.Concurrent;

namespace GrifMVD.NewsFolder.Services
{
    public class ScrapingService : IScraping
    {
        private readonly ILogger<ScrapingService> _logger;
        private readonly DateTreatmentService _date;
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        
        public ScrapingService(ILogger<ScrapingService> logger, DataContext dataContext, IMapper mapper, DateTreatmentService date)
        {
            _logger = logger;
            _dataContext = dataContext;
            _mapper = mapper;
            _date = date;
        }
        public async Task<ICollection<NewsDTO>> ScrapingWebPageAsync()
        {
            var newsDb = await ScrapingNewsAsync();
            await SaveToCsvAsync(newsDb, "news-products.csv");

            ICollection<NewsDTO> newsDTO = _mapper.Map<ICollection<NewsDTO>>(newsDb);
            newsDTO  = newsDTO.OrderByDescending(u => u.ParseTime).ToList();
            return newsDTO;
        }
        private async Task<ConcurrentBag<NewsDb>> ScrapingNewsAsync()
        {
            HtmlWeb web = new HtmlWeb();
            ConcurrentBag<NewsDb> newsDb = new ConcurrentBag<NewsDb>();

            ConcurrentBag<string> pagesToScrape = new ConcurrentBag<string>
            {   
                "https://мосу.мвд.рф/Press-sluzhba/Novosti/1/",
            };
            foreach (var currentPage in pagesToScrape)
            {
                HtmlDocument document = web.Load(currentPage);
                foreach (var producHtmlElements in document.DocumentNode.SelectNodes(".//div[@class='sl-item']"))
                {
                    NewsDb? newsItem = await ScrapingNewsItemAsync(producHtmlElements);
                    if (newsItem != null)
                    {
                        newsDb.Add(newsItem);
                        await SavePhotosAsync(newsItem);
                    }
                }
            }
            await _dataContext.SaveChangesAsync();
            return newsDb;
        }      
        private async Task<NewsDb?> ScrapingNewsItemAsync(HtmlNode productHTMLElement)
        {
            if (productHTMLElement != null)
            {
                HtmlNode aNodeDate = productHTMLElement.SelectSingleNode(".//div[@class='sl-item-date']");
                HtmlNode aNodeTitle = productHTMLElement.SelectSingleNode(".//div[@class='sl-item-title']");
                HtmlNode aNodeDescription = productHTMLElement.SelectSingleNode(".//div[@class='sl-item-text']");
                if (aNodeDate != null && aNodeTitle != null)
                {
                    HtmlNode aNode = aNodeTitle.SelectSingleNode("a");
                    string url = aNode.GetAttributeValue("href", "");
                    var result = _dataContext.News.FirstOrDefault(i => i.Url == url);
                    if (result != null)
                    {
                        return null;
                    }
                    string title = aNode.InnerText.Trim();
                    string description = aNodeDescription.InnerText.Trim();
                    string divTime = aNodeDate.InnerText.Trim();

                    DateTime parsedTime = _date.ClearingDate(divTime);


                    NewsDb newDb = new NewsDb()
                    {
                        Id = Guid.NewGuid(),
                        Url = url,
                        Title = title,
                        Description = description,
                        ParseTime = parsedTime,
                        CreatedTime = DateTime.UtcNow
                    };
                    

                    _logger.Log(LogLevel.Information,
                        $" Url : {url}" +
                        $"\tTitle : {title}" +
                        $"\tDescription : {description}" +
                        $"\tParseTime : {parsedTime}");

                    await _dataContext.News.AddAsync(newDb);
                    return newDb;                  
                }
                else
                {
                    throw new Exception("Дата и заголовок не найдены");
                }
            }
            else
            {
                throw new Exception("Страница не найдена");
            }
        }
        private async Task SavePhotosAsync(NewsDb newsItem)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument page = web.Load("https://мосу.мвд.рф/" + newsItem.Url);
            HtmlNodeCollection imageElements = page.DocumentNode.SelectNodes(".//a[@class='cboxElement']/img");
            if (imageElements != null)
            {
                foreach (var imageElement in imageElements)
                {
                    string? imgSrc = imageElement.GetAttributeValue("src", "");

                    PhotosDb photoDb = new PhotosDb()
                    {
                        Id = Guid.NewGuid(),
                        UrlImage = imgSrc,
                        NewsDbID = newsItem.Id,
                    };
                    newsItem.Photos.Add(photoDb);
                    _logger.Log(LogLevel.Information, $" UrlImage : {imgSrc}");
                    await _dataContext.Photos.AddAsync(photoDb);
                }
            }
        }

        private async Task SaveToCsvAsync<T>(IEnumerable<T> records, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                await csv.WriteRecordsAsync(records);
            }
        }       
    }
}