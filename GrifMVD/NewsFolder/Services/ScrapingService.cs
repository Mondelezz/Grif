using GrifMVD.NewsFolder.Interfaces;
using GrifMVD.NewsFolder.Models;
using HtmlAgilityPack;
using System;
using CsvHelper;
using System.Globalization;

namespace GrifMVD.NewsFolder.Services
{
    public class ScrapingService : IScraping
    {
        private readonly ILogger<NewsDb> _logger;

        public ScrapingService(ILogger<NewsDb> logger)
        {
            _logger = logger;
        }
        public ICollection<NewsDb> ScrapingWebPage()
        {
          
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load("https://мосу.мвд.рф/");

            ICollection<NewsDb> newsDb = new List<NewsDb>();

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
                    string parceTime = "Дата " + dateText;


                    NewsDb newDb = new NewsDb()
                    {
                        Url = url,
                        Title = title,
                        Description = description,
                        ParseTime = parceTime,
                    };
                    _logger.Log(LogLevel.Information,
                        $" Url : {url}" +
                        $"\tTitle : {title}" +
                        $"\tDescription : {description}" +
                        $"\tParseTime : {parceTime}");

                    newsDb.Add(newDb);

                    using (var writer = new StreamWriter("news-products.csv"))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(newsDb);
                    }
                }
            }
            
            return newsDb;
        }
    }
}
