using GrifMVD.NewsFolder.Interfaces;
using GrifMVD.NewsFolder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GrifMVD.NewsFolder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScrapingController : ControllerBase
    {
        private readonly IScraping _scraping;

        public ScrapingController(IScraping scraping)
        {
            _scraping = scraping;
        }
        [HttpGet("scrap")]
        public async Task<ActionResult<ICollection<NewsDTO>>> ScrapingWebPage()
        {
            var result = await _scraping.ScrapingWebPageAsync();
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
