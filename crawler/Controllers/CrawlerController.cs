using crawler.Controllers.crawler;
using Microsoft.AspNetCore.Mvc;

namespace crawler.Controllers
{
    public class CrawlerController : Controller
    {
        private readonly IEnumerable<IHtmlCrawler> _crawlers;

        public CrawlerController(IEnumerable<IHtmlCrawler> crawlers)
        {
            _crawlers = crawlers;
        }

        [HttpGet]
        [Route("/api/ranking")]
        public async Task<IActionResult> Crawl([FromQuery] string url, [FromQuery] int limit, [FromQuery] int dateRange = 7)
        {
            var crawler = _crawlers.FirstOrDefault(c => url.Contains(c.Domain));
            if (crawler == null)
            {
                return NotFound($"Crawler of type '{url}' not found.");
            }
            if (limit > 100)
            {
                return NotFound($"Nope :)");
            }

            var result = await crawler.CrawlAsync(url, limit, dateRange);
            return Ok(result);
        }
    }
}
