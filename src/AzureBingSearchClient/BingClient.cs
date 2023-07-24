using Newtonsoft.Json;

namespace AzureBingSearchClient;

public class BingClient
{
    public class SerachResult
    {
        public dynamic? DynamicProperties;
    }

    // Web ページ検索結果
    public class WebPage : SerachResult
    {
        public string? Name;
        public string? Url;
        public string? Snippet;
        public DateTimeOffset DateLastCrawled;
    }

    // 画像検索結果
    public class WebImage : SerachResult
    {
        public string? Name;
        public string? ThumbnailUrl;
        public string? ContentUrl;
        public DateTimeOffset DatePublished;
    }

    // 動画検索結果
    public class WebVideo : SerachResult
    {
        public string? Name;
        public string? ThumbnailUrl;
        public string? ContentUrl;
        public string? Description;
        public string? Publisher;
        public string? Creator;
        public long? ViewCount;
        public DateTimeOffset DatePublished;
    }

    // News 検索結果
    public class WebNews : SerachResult
    {
        public string? Name;
        public string? Url;
        public string? Description;
        public string? ThumbnailUrl;
        public string? Category;
        public DateTimeOffset DatePublished;
    }

    private static readonly HttpClient _httpClient = new HttpClient();

    private readonly string _endpoint = "https://api.bing.microsoft.com/v7.0";
    private readonly string _subscriptionKey;

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

        var pages = json.webPages?.value as IEnumerable<dynamic>;

        if (pages == null)
        {
            return Enumerable.Empty<WebPage>();
        }

        var webPages = pages.Select(page =>
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

    public async Task<IEnumerable<WebImage>> SearchImage(string keyword, string market = "ja-JP", int resultCount = 10)
    {
        //https://docs.microsoft.com/ja-jp/bing/search-apis/bing-image-search/quickstarts/rest/csharp

        string endpoint = $"{_endpoint}/images/search";
        string url = $"{endpoint}?q={Uri.EscapeDataString(keyword)}&mkt={market}&count={resultCount}";

        dynamic json = await this.Search(url, _subscriptionKey);

        var imgs = json.value as IEnumerable<dynamic>;

        if (imgs == null)
        {
            return Enumerable.Empty<WebImage>();
        }

        var images = imgs.Select(img =>
            new WebImage()
            {
                DynamicProperties = img,
                Name = img.name,
                ThumbnailUrl = img.thumbnailUrl,
                ContentUrl = img.contentUrl,
                DatePublished = DateTime.SpecifyKind(img.datePublished.Value, DateTimeKind.Utc),
            });

        return images;
    }

    public async Task<IEnumerable<WebVideo>> SearchVideo(string keyword, string market = "ja-JP", int resultCount = 10)
    {
        //https://docs.microsoft.com/ja-jp/bing/search-apis/bing-video-search/quickstarts/rest/csharp

        string endpoint = $"{_endpoint}/videos/search";
        string url = $"{endpoint}?q={Uri.EscapeDataString(keyword)}&mkt={market}&count={resultCount}";

        dynamic json = await this.Search(url, _subscriptionKey);

        var vdos = json.value as IEnumerable<dynamic>;

        if (vdos == null)
        {
            return Enumerable.Empty<WebVideo>();
        }

        var videos = vdos.Select(vdo =>
            new WebVideo()
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

    public async Task<IEnumerable<WebNews>> SearchNews(string keyword, string market = "ja-JP", int resultCount = 10)
    {
        //https://docs.microsoft.com/ja-jp/bing/search-apis/bing-news-search/quickstarts/rest/csharp

        string endpoint = $"{_endpoint}/news/search";
        string url = $"{endpoint}?q={Uri.EscapeDataString(keyword)}&mkt={market}&count={resultCount}";

        dynamic json = await this.Search(url, _subscriptionKey);

        var someNs = json.value as IEnumerable<dynamic>;

        if (someNs == null)
        {
            return Enumerable.Empty<WebNews>();
        }

        var newsList = someNs.Select(ns =>
            new WebNews()
            {
                DynamicProperties = ns,
                Name = ns.name,
                Url = ns.url,
                Description = ns.description,
                ThumbnailUrl = ns.image?.thumbnail?.contentUrl,
                Category = ns.category,
                DatePublished = DateTime.SpecifyKind(ns.datePublished.Value, DateTimeKind.Utc),
            });

        return newsList;
    }

    private async Task<dynamic> Search(string url, string subscriptionKey)
    {

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
        request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

        HttpResponseMessage response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        string responseString = await response.Content.ReadAsStringAsync();

        dynamic? json = JsonConvert.DeserializeObject<dynamic?>(responseString);

        if (json == null)
        {
            throw new InvalidDataException("The response format is invalid.");
        }

        return json;
    }
}