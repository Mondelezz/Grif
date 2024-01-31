using GrifMVD.NewsFolder.Interfaces;
using GrifMVD.NewsFolder.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using GrifMVD.NewsFolder.Data;
using AutoMapper;

namespace GrifMVD.NewsFolder.Services
{
    public class ScrapingService : IScraping
    {
        private readonly ILogger<ScrapingService> _logger;
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public ScrapingService(ILogger<ScrapingService> logger, DataContext dataContext, IMapper mapper)
        {
            _logger = logger;
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<ICollection<NewsDTO>> ScrapingWebPageAsync()
        {
            var newsDb = await ScrapNewsAsync();
            await SaveToCsvAsync(newsDb, "news-products.csv");

            ICollection<NewsDTO> newsDTO = _mapper.Map<ICollection<NewsDTO>>(newsDb);
            return newsDTO;
        }

        private async Task<ConcurrentBag<NewsDb>> ScrapNewsAsync()
        {
            var web = new HtmlWeb();
            var newsDb = new ConcurrentBag<NewsDb>();

            var pagesToScrape = new ConcurrentBag<string>
            {
                "https://мосу.мвд.рф/Press-sluzhba/Novosti/1/",
                "https://мосу.мвд.рф/Press-sluzhba/Novosti/2/",
            };

            Parallel.ForEach(pagesToScrape, new ParallelOptions { MaxDegreeOfParallelism = 4 }, currentPage =>
            {
                var document = web.Load(currentPage);

                foreach (var productHTMLElements in document.DocumentNode.SelectNodes(".//div[@class='sl-item']"))
                {
                    var newsItem = ScrapeNewsItem(productHTMLElements);
                    if (newsItem != null)
                    {
                        newsDb.Add(newsItem);
                        await SavePhotosAsync(newsItem);
                    }
                }
            });

            await _dataContext.SaveChangesAsync();
            return newsDb;
        }

        private NewsDb? ScrapeNewsItem(HtmlNode productHTMLElements)
        {
            var aNodeDate = productHTMLElements.SelectSingleNode(".//div[@class='sl-item-date']");
            var aNodeTitle = productHTMLElements.SelectSingleNode(".//div[@class='sl-item-title']");
            var aNodeDescription = productHTMLElements.SelectSingleNode(".//div[@class='sl-item-text']");

            if (aNodeDate != null && aNodeTitle != null)
            {
                var aNode = aNodeTitle.SelectSingleNode("a");
                string url = aNode.GetAttributeValue("href", "");
                string title = aNode.InnerText.Trim();
                string description = aNodeDescription.InnerText.Trim();
                string divTime = aNodeDate.InnerText.Trim();
                string dateText = divTime.Split('<')[0].Trim();
                string parseTime = "Дата " + dateText;

                var newsDbItem = new NewsDb()
                {
                    Id = Guid.NewGuid(),
                    Url = url,
                    Title = title,
                    Description = description,
                    ParseTime = parseTime,
                    CreatedTime = DateTime.UtcNow
                };

                _logger.Log(LogLevel.Information,
                    $" Url : {url}" +
                    $"\tTitle : {title}" +
                    $"\tDescription : {description}" +
                    $"\tParseTime : {parseTime}");

                return newsDbItem;
            }

            return null;
        }

        private async Task SavePhotosAsync(NewsDb newsItem)
        {
            var web = new HtmlWeb();
            var page = web.Load("https://мосу.мвд.рф" + newsItem.Url);
            var imageElements = page.DocumentNode.SelectNodes(".//a[@class='cboxElement']/img");

            if (imageElements != null)
            {
                foreach (var imageElement in imageElements)
                {
                    string imgSrc = imageElement.GetAttributeValue("src", "");
                    var photoDb = new PhotosDb()
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

            await _dataContext.News.AddAsync(newsItem);
        }

        private async Task SaveToCsvAsync<T>(IEnumerable<T> records, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }
        }
    }
}
