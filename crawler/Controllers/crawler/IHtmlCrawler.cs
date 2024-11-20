namespace crawler.Controllers.crawler
{
    public interface IHtmlCrawler
    {
        string Domain { get; }
        Task<List<News>> CrawlAsync(string url, int maxNum, int dateRange);
    }
}
