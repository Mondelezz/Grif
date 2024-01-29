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
        public ActionResult<ICollection<NewsDb>> ScrapingWebPage()
        {
            var result = _scraping.ScrapingWebPage();
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
