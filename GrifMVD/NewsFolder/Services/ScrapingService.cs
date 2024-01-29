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

            IList<HtmlNode> productHTMLElements = document.DocumentNode.SelectNodes("//div[@class='sl-item']");
            HtmlNode aNodeDate = document.DocumentNode.SelectSingleNode("//div[@class='sl-item-date']");
            HtmlNode aNodeTitle = document.DocumentNode.SelectSingleNode("//div[@class='sl-item-title']");

            if (productHTMLElements != null && aNodeDate != null && aNodeTitle != null)
            {
                foreach (var productHTMLElement in productHTMLElements)
                {
                    HtmlNode aNode = aNodeTitle.SelectSingleNode("a");
                    string url = aNode.GetAttributeValue("href", "");
                    string description = aNode.InnerText.Trim();

                    string divTime = aNodeDate.InnerText.Trim();
                    string dateText = divTime.Split('<')[0].Trim();
                    string parceTime = "Дата " + dateText;

                    NewsDb newDb = new NewsDb()
                    {
                        Url = url,
                        Description = description,
                        ParseTime = parceTime,
                    };
                    _logger.Log(LogLevel.Information, $"Url : {url}" + $"Description : {description}" + $"ParseTime : {parceTime}");
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
