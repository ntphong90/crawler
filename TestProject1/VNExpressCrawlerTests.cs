using crawler.Controllers.crawler;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Crawler.Tests
{
    public class VNExpressCrawlerTests
    {
        private VNExpressCrawler _crawler;

        public VNExpressCrawlerTests()
        {
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
             .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
             .ReturnsAsync(new HttpResponseMessage
             {
                 StatusCode = System.Net.HttpStatusCode.OK,
                 Content = new StringContent("<rss><channel><item><link>https://vnexpress.net/some-article.html</link></item></channel></rss>")
             });
            var client = new HttpClient(mockHandler.Object);
            _crawler = new VNExpressCrawler(client); 
        }

        [Fact]
        public async Task CrawlAsync_FetchesNewsSuccessfully_ReturnsOrderedList()
        {
            // Arrange: Setup the mock response for the article JSON data
            var articleJson = "{ \"data\": [{ \"title\": \"Test Article\", \"share_url\": \"https://vnexpress.net/some-article.html\", \"publish_time\": "+ new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + ", \"userlike\": 100 }] }";

            // Mock the response for the article data
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
             .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(articleJson)
                });

            var client = new HttpClient(mockHandler.Object);
            _crawler = new VNExpressCrawler(client); 
            var result = await _crawler.CrawlAsync("https://vnexpress.net", 5);

            Assert.NotNull(result);
            Assert.Equal(1, result.Count); 
            Assert.Equal("Test Article", result[0].Title);
        }

        [Fact]
        public async Task CrawlAsync_GetVotes()
        {
            // Arrange: Setup the mock response for the article JSON data
            var votesJson = "{ \"error\": 0, \"errorDescription\": \"\", \"iscomment\": 1, \"data\": { \"total\": 37, \"totalitem\": 94, \"items\": [ {\"userlike\": 6}, {\"userlike\": 6}, {\"userlike\": 5}, {\"userlike\": 5}, {\"userlike\": 5}, {\"userlike\": 4}, {\"userlike\": 3}, {\"userlike\": 2} ], \"items_pin\": [], \"offset\": 29 } }";
            // Mock the response for the article data
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
             .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(votesJson)
                });

            var client = new HttpClient(mockHandler.Object);
            _crawler = new VNExpressCrawler(client);  // Initialize crawler with the mocked HttpClient

            // Act: Call CrawlAsync to test fetching the news
            var result = await _crawler.getVote(1);

            // Assert: Check that the result is correct
            Assert.NotNull(result);
            Assert.Equal(36, result);
           
        }
    }
}
