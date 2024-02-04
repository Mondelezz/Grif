using GrifMVD.NewsFolder.Interfaces;
using GrifMVD.NewsFolder.Models;

using Microsoft.AspNetCore.Mvc;

namespace GrifMVD.NewsFolder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScrapingController : ControllerBase
    {
        private readonly IScraping _scraping;
        private readonly IHandleNews _handleNews;

        public ScrapingController(IScraping scraping, IHandleNews handleNews)
        {
            _scraping = scraping;
            _handleNews = handleNews;
        }
        [HttpGet("news/scrap")]
        public async Task<ActionResult<ICollection<NewsDTO>>> ScrapingWebPage()
        {
            var result = await _scraping.ScrapingWebPageAsync();
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [HttpGet("news/get")]
        public async Task<ActionResult<ICollection<NewsDTO>>> GetNews()
        {
            ICollection<NewsDTO> result = _handleNews.GetNews();
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
