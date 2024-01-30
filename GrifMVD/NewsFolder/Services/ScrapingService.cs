using GrifMVD.NewsFolder.Interfaces;
using GrifMVD.NewsFolder.Models;
using HtmlAgilityPack;
using System;
using CsvHelper;
using System.Globalization;
using System.Xml;
using GrifMVD.NewsFolder.Data;

namespace GrifMVD.NewsFolder.Services
{
    public class ScrapingService : IScraping
    {
        private readonly ILogger<ScrapingService> _logger;
        private readonly DataContext _dataContext;
        public ScrapingService(ILogger<ScrapingService> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }
        public async Task<ICollection<NewsDb>> ScrapingWebPageAsync()
        {
          
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load("https://мосу.мвд.рф/");

            ICollection<NewsDb> newsDb = new List<NewsDb>();
            ICollection<PhotosDb> photosDb = new List<PhotosDb>();
            
            foreach (var productHTMLElements in document.DocumentNode.SelectNodes(".//div[@class='sl-item']"))
            {
                HtmlNode aNodeDate = productHTMLElements.SelectSingleNode(".//div[@class='sl-item-date']");
                HtmlNode aNodeTitle = productHTMLElements.SelectSingleNode(".//div[@class='sl-item-title']");
                HtmlNode aNodeDescription = productHTMLElements.SelectSingleNode(".//div[@class='sl-item-text']");
                if (productHTMLElements != null && aNodeDate != null && aNodeTitle != null)
                {
                    HtmlNode aNode = aNodeTitle.SelectSingleNode("a");
                    string url = aNode.GetAttributeValue("href", "");
                    string title = aNode.InnerText.Trim();
                    string description = aNodeDescription.InnerText.Trim();
                    string divTime = aNodeDate.InnerText.Trim();
                    string dateText = divTime.Split('<')[0].Trim();
                    string parseTime = "Дата " + dateText;


                    NewsDb newDb = new NewsDb()
                    {
                        Id = Guid.NewGuid(),
                        Url = url,
                        Title = title,
                        Description = description,
                        ParseTime = parseTime,
                    };
                    _logger.Log(LogLevel.Information,
                        $" Url : {url}" +
                        $"\tTitle : {title}" +
                        $"\tDescription : {description}" +
                        $"\tParseTime : {parseTime}");

                    newsDb.Add(newDb);

                    using (var writer = new StreamWriter("news-products.csv"))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(newsDb);
                    }
                }
                foreach (var item in newsDb)
                {
                    HtmlDocument page = web.Load("https://мосу.мвд.рф" + item.Url);

                    var imageElements = page.DocumentNode.SelectNodes(".//a[@class='cboxElement']/img");

                    if (imageElements != null)
                    {
                        foreach (HtmlNode imageElement in imageElements)
                        {
                            string? imgSrc = imageElement.GetAttributeValue("src", "");

                            PhotosDb photoDb = new PhotosDb()
                            {
                                Id = Guid.NewGuid(),
                                UrlImage = imgSrc,
                                NewsDbID = item.Id,
                            };

                            photosDb.Add(photoDb);
                            _logger.Log(LogLevel.Information, $" UrlImage : {imgSrc}");
                            item.Photos.Add(photoDb);
                            await _dataContext.Photos.AddAsync(photoDb);
                        }
                    }

                    await _dataContext.News.AddAsync(item);
                }

                using (var writer = new StreamWriter("photos-products.csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(photosDb);
                }

            }
            await _dataContext.SaveChangesAsync();
            
            return newsDb;
        }
    }
}
