using System.Net.Http;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace crawler.Controllers.crawler
{
    public class VNExpressCrawler : IHtmlCrawler
    {
        public string Domain => "https://vnexpress.net";

        public List<News> ResultList { get; set; }
        public int MaxNum;
        private HttpClient _httpClient;
        public VNExpressCrawler(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient(); // Use real HttpClient if not passed
        }


        public async Task<List<News>> CrawlAsync(string url, int maxNum)
        {
            ResultList = new List<News>();
            MaxNum = maxNum;
            //await getNewsFromCategory(1);
            int latestID = 0;
            using (XmlReader reader = XmlReader.Create("https://vnexpress.net/rss/tin-moi-nhat.rss"))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "link")
                        {
                            XElement el = XNode.ReadFrom(reader) as XElement;
                            if (el != null)
                            {
                                if(el.Value.Contains(".html"))
                                {
                                    string id = el.Value.Replace(".html", "").Split("-").Last();
                                    try
                                    {
                                        latestID = int.Parse(id);
                                        break;
                                    } catch
                                    {

                                    }   
                                }
                            }
                        }
                    }
                }
            }
            await getNews(latestID);
            return ResultList.OrderByDescending(o => o.Vote).ToList();
        }

        public async Task getNews(int id)
        {
            //using HttpClient client = new HttpClient();
            try
            {
                string articleData = await _httpClient.GetStringAsync("https://gw.vnexpress.net/ar/get_basic?article_id=" + id + "&data_select=title,lead,short_lead,privacy,share_url,article_type,publish_time,thumbnail_url");
                var responseObject = JsonObject.Parse(articleData);
                var data = JsonObject.Parse(responseObject["data"].ToString());
                if (data != null && data is JsonArray)
                {
                    foreach (var item in (JsonArray)data)
                    {
                        News news = new News();
                        news.Title = item["title"].GetValue<string>();
                        news.Url = item["share_url"].GetValue<string>();
                        news.image = item["thumbnail_url"].GetValue<string>();
                        news.ID = id.ToString();
                        news.PublicDate = DateTimeOffset.FromUnixTimeMilliseconds(item["publish_time"].GetValue<long>() * 1000).Date;
                        news.Vote = await getVote(id);

                        if (news.PublicDate < DateTime.Now.AddDays(-7))
                        {
                            return;
                        }
                        if (ResultList.Count < MaxNum)
                        {
                            ResultList.Add(news);
                        }
                        else
                        {
                            News temp = ResultList.MinBy(o => o.Vote);
                            if (temp.Vote < news.Vote)
                            {
                                ResultList.Remove(temp);
                                ResultList.Add(news);
                            }
                        }

                    }
                }
                await getNews(id - 1);
            } catch (Exception ex) {

            }

        }

        public async Task<int> getVote(int id)
        {
            //using HttpClient client = new HttpClient();
            try
            {
                bool isRemain = true;
                int offset = 0;
                int total = -1;
                int votesTotal = 0;
                while (isRemain)
                {
                    String detail = await _httpClient.GetStringAsync("https://usi-saas.vnexpress.net/index/get?offset=0&limit=100&objectid=" + id + "&objecttype=1&siteid=1000000");
                    var responseObject = JsonObject.Parse(detail);
                    var data = JsonObject.Parse(responseObject["data"].ToString());
                    if (data != null)
                    {
                        if (total == -1)
                        {
                            total = data["total"].GetValue<int>();
                        }
                        var items = data["items"];
                        if (items is JsonArray)
                        {
                            foreach (var item in (JsonArray)items)
                            {
                                votesTotal += item["userlike"].GetValue<int>();
                            }
                        }
                    }
                    if (offset * 100 + 100 >= total)
                    {
                        isRemain = false;
                    }
                    else
                    {
                        offset = offset + 100;
                    }
                }
                return votesTotal;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
