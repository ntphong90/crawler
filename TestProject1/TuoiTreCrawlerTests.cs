using crawler.Controllers.crawler;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Crawler.Tests
{
    public class TuoiTreTests
    {
        private TuoiTreCrawler _crawler;

        public TuoiTreTests()
        {

        }

        [Fact]
        public async Task ParseNews_ShouldReturnValidNewsObject_WhenValidUrlIsProvided()
        {
            // Arrange
            string url = "https://tuoitre.vn/some-news.htm";
            string htmlContent = "<html><body><h1 class='detail-title article-title'>Test News</h1></body></html>";
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
             .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
             .ReturnsAsync(new HttpResponseMessage
             {
                 StatusCode = System.Net.HttpStatusCode.OK,
                 Content = new StringContent(htmlContent)
             });
            var client = new HttpClient(mockHandler.Object);
            _crawler = new TuoiTreCrawler(client);
            // Act
            var result = await _crawler.ParseNews(url);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test News", result.Title);
        }

        [Fact]
        public async Task GetVotes_ShouldReturnCorrectVoteCount_WhenValidIdIsProvided()
        {
            // Arrange
            string newsId = "12345";
            string jsonResponse = "{\"Data\":[{\"reactions\":{\"like\":10, \"love\":5, \"haha\":3}}]}";
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
             .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
             .ReturnsAsync(new HttpResponseMessage
             {
                 StatusCode = System.Net.HttpStatusCode.OK,
                 Content = new StringContent(jsonResponse)
             });
            var client = new HttpClient(mockHandler.Object);
            _crawler = new TuoiTreCrawler(client);
            // Act
            var result = await _crawler.getVotes(newsId);

            // Assert
            Assert.Equal(18, result); // 10 + 5 + 3
        }

        [Fact]
        public void ParseDate_ShouldReturnValidDate_WhenCorrectDateStringIsProvided()
        {
            // Arrange
            string dateInput = "10/10/2023";

            // Act
            _crawler = new TuoiTreCrawler();
            var result = _crawler.parseDate(dateInput);

            // Assert
            Assert.Equal(new DateTime(2023, 10, 10), result);
        }
    }
}
