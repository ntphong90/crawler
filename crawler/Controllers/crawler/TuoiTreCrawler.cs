using System;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using HtmlAgilityPack;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace crawler.Controllers.crawler
{
    public class TuoiTreCrawler : IHtmlCrawler
    {
        public string Domain => "https://tuoitre.vn";

        public List<News> ResultList { get; set; }
        public int MaxNum;
        private HttpClient _httpClient;
        public TuoiTreCrawler(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient(); // Use real HttpClient if not passed
        }
        public async Task<List<News>> CrawlAsync(string url, int maxNum)
        {
            ResultList = new List<News>();
            MaxNum = maxNum;
            await getNewsFromCategory(1);

            return ResultList.OrderByDescending(o => o.Vote).ToList();
        }

        async Task getNewsFromCategory(int page)
        {
            try
            {
                string htmlContent = await _httpClient.GetStringAsync("https://tuoitre.vn/timeline/0/" + "/trang-" +page + ".htm");
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlContent);
                var newsNodes = htmlDocument.DocumentNode.SelectNodes("//a[contains(@class, 'box-category-link-title')]");

                if (newsNodes != null)
                {
                    foreach (var node in newsNodes)
                    {
                        string href = node.GetAttributeValue("href", string.Empty);
                        string url = href != "" && !href.Contains("https:") ? this.Domain + href : "";
                        if (url != "")
                        {
                            News news = await ParseNews(url);
                            if (news != null && news.PublicDate > DateTime.Now.AddDays(-7))
                            {
                               if(ResultList.Count < MaxNum)
                                {
                                    ResultList.Add(news);
                                } else
                                {
                                    News temp = ResultList.MinBy(o => o.Vote);
                                    if(temp.Vote < news.Vote)
                                    {
                                        ResultList.Remove(temp);
                                        ResultList.Add(news);
                                    }
                                }
                            } 
                            if(news != null && news.PublicDate < DateTime.Now.AddDays(-7) && news.PublicDate != DateTime.MinValue)
                            {
                                return;
                            }
                        }
                    }
                }
                await getNewsFromCategory(page + 1);
            } catch
            {
            }
        }

        public async Task<News> ParseNews(string url)
        {
            try
            {
                string htmlContent = await _httpClient.GetStringAsync(url);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlContent);
                News res = new News();
                var idNode = url.Replace(".htm","").Split("-").Last();
                var titleNode = htmlDocument.DocumentNode.SelectNodes("//h1[contains(@class, 'detail-title article-title')]");

                //Information
                res.Title = titleNode?.First().GetDirectInnerText();
                res.Url = url;
                var dateNode = htmlDocument.DocumentNode.SelectNodes("//div[@data-role='publishdate']");

                //date
                try
                {         
                    res.PublicDate = dateNode != null ? parseDate(dateNode.First().GetDirectInnerText()) : DateTime.MinValue;
                }
                catch (Exception e)
                {
                    Console.Write("here");
                }
                
                // Votes
                res.Vote = await getVotes(idNode);
                return res;
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> getVotes(string idNode)
        {
            int vote = 0;
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("https://id.tuoitre.vn/api/getlist-comment.api?objId=" + idNode + "&objType=1&objectpopupid=&sort=2&commentid=&command=&appKey=lHLShlUMAshjvNkHmBzNqERFZammKUXB1DjEuXKfWAwkunzW6fFbfrhP%2FIG0Xwp7aPwhwIuucLW1TVC9lzmUoA%3D%3D");
                response.EnsureSuccessStatusCode();
                string responseData = await response.Content.ReadAsStringAsync();
                var responseObject = JsonObject.Parse(responseData);
                JsonArray data = (JsonArray)JsonObject.Parse(responseObject["Data"].ToString());
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var itemData = item["reactions"];
                        foreach (var keynode in (JsonObject)itemData)
                        {
                            vote += int.Parse(keynode.Value.ToString());
                        }

                    }
                }
                return vote;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public DateTime parseDate(string input)
        {
            input = input.Replace("\n", "").Replace("\r", "").Trim();
            input = input.Substring(0, Math.Min(input.Length, 10));
   
            return DateTime.ParseExact(input,
                       "dd/MM/yyyy",
                       CultureInfo.InvariantCulture);
        }
    }

}
