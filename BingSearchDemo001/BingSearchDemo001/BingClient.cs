using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BingSearchDemo001
{
    public class BingClient
    {
        public class SerachResult
        {
            public dynamic DynamicProperties;
        }

        // Web ページ検索結果
        public class WebPage : SerachResult
        {
            public string Name;
            public string Url;
            public string Snippet;
            public DateTimeOffset DateLastCrawled;
        }

        // 画像検索結果
        public class Image : SerachResult
        {
            public string Name;
            public string ThumbnailUrl;
            public string ContentUrl;
            public DateTimeOffset DatePublished;
        }

        // 動画検索結果
        public class Video : SerachResult
        {
            public string Name;
            public string ThumbnailUrl;
            public string ContentUrl;
            public string Description;
            public string Publisher;
            public string Creator;
            public long? ViewCount;
            public DateTimeOffset DatePublished;
        }

        public class News : SerachResult
        {
            public string Name;
            public string Url;
            public string Description;
            public string ThumbnailUrl;
            public string Category;
            public DateTimeOffset DatePublished;
        }

        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly string _endpoint = "https://api.bing.microsoft.com/v7.0";
        private readonly string _subscriptionKey = null;

        public BingClient(string subscriptionKey)
        {
            this._subscriptionKey = subscriptionKey;
        }

        public async Task<IEnumerable<WebPage>> SearchWeb(string keyword, string market = "ja-JP", int resultCount = 10)
        {
            //https://docs.microsoft.com/ja-jp/bing/search-apis/bing-image-search/quickstarts/rest/csharp

            string endpoint = $"{_endpoint}/search";
            string url = $"{endpoint}?q={Uri.EscapeDataString(keyword)}&mkt={market}&responseFilter=Webpages&count={resultCount}";

            dynamic json = await this.Search(url, _subscriptionKey);

            var webPages = (json.webPages.value as IEnumerable<dynamic>).Select(page =>
                new WebPage()
                {
                    DynamicProperties = page,
                    Name = page.name,
                    Url = page.url,
                    Snippet = page.snippet,
                    DateLastCrawled = DateTime.SpecifyKind(page.dateLastCrawled.Value, DateTimeKind.Utc),
                });

            return webPages;
        }

        public async Task<IEnumerable<Image>> SearchImage(string keyword, string market = "ja-JP", int resultCount = 10)
        {
            //https://docs.microsoft.com/ja-jp/bing/search-apis/bing-image-search/quickstarts/rest/csharp

            string endpoint = $"{_endpoint}/images/search";
            string url = $"{endpoint}?q={Uri.EscapeDataString(keyword)}&mkt={market}&count={resultCount}";
            
            dynamic json = await this.Search(url, _subscriptionKey);

            var images = (json.value as IEnumerable<dynamic>).Select(img => 
                new Image()
                {
                    DynamicProperties = img,
                    Name = img.name,
                    ThumbnailUrl = img.thumbnailUrl,
                    ContentUrl = img.contentUrl,
                    DatePublished = DateTime.SpecifyKind(img.datePublished.Value, DateTimeKind.Utc),
                });

            return images;
        }

        public async Task<IEnumerable<Video>> SearchVideo(string keyword, string market = "ja-JP", int resultCount = 10)
        {
            //https://docs.microsoft.com/ja-jp/bing/search-apis/bing-video-search/quickstarts/rest/csharp

            string endpoint = $"{_endpoint}/videos/search";
            string url = $"{endpoint}?q={Uri.EscapeDataString(keyword)}&mkt={market}&count={resultCount}";

            dynamic json = await this.Search(url, _subscriptionKey);

            var videos = (json.value as IEnumerable<dynamic>).Select(vdo =>
                new Video()
                {
                    DynamicProperties = vdo,
                    Name = vdo.name,
                    ThumbnailUrl = vdo.thumbnailUrl,
                    ContentUrl = vdo.contentUrl,
                    Description = vdo.description,
                    Publisher = vdo.publisher?[0]?.name,
                    Creator = vdo.creator?.name,
                    ViewCount = vdo.viewCount,
                    DatePublished = DateTime.SpecifyKind(vdo.datePublished.Value, DateTimeKind.Utc),
                });           

            return videos;
        }

        public async Task<IEnumerable<News>> SearchNews(string keyword, string market = "ja-JP", int resultCount = 10)
        {
            //https://docs.microsoft.com/ja-jp/bing/search-apis/bing-news-search/quickstarts/rest/csharp

            string endpoint = $"{_endpoint}/news/search";
            string url = $"{endpoint}?q={Uri.EscapeDataString(keyword)}&mkt={market}&count={resultCount}";

            dynamic json = await this.Search(url, _subscriptionKey);

            var news = (json.value as IEnumerable<dynamic>).Select(ns =>
                new News()
                {
                    DynamicProperties = ns,
                    Name = ns.name,
                    Url = ns.url,
                    Description = ns.description,
                    ThumbnailUrl = ns.image?.thumbnail?.contentUrl,
                    Category = ns.category,
                    DatePublished = DateTime.SpecifyKind(ns.datePublished.Value, DateTimeKind.Utc),
                });

            return news;
        }

        private async Task<dynamic> Search(string url, string subscriptionKey)
        {

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            dynamic json = JsonConvert.DeserializeObject<dynamic>(responseString);

            return json;
        }
    }
}
