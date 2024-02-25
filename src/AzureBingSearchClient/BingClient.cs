using AzureBingSearchClient.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AzureBingSearchClient;

public class BingClient
{
    private static readonly HttpClient _httpClient = new HttpClient();

    private readonly string _endpoint = "https://api.bing.microsoft.com/v7.0";
    private readonly string _subscriptionKey;

    public BingClient(string subscriptionKey)
    {
        this._subscriptionKey = subscriptionKey;
    }

    public async Task<IEnumerable<WebPage>> SearchWebAsync(string keyword, string market = "ja-JP", int resultCount = 10)
    {
        //https://docs.microsoft.com/ja-jp/bing/search-apis/bing-image-search/quickstarts/rest/csharp

        string endpoint = $"{_endpoint}/search";
        string url = $"{endpoint}?q={Uri.EscapeDataString(keyword)}&mkt={market}&responseFilter=Webpages&count={resultCount}";

        var json = await this.SearchAsync(url, _subscriptionKey).ConfigureAwait(false);

        var pages = json["webPages"]?["value"]?.AsArray();

        if (pages == null)
        {
            return Enumerable.Empty<WebPage>();
        }

        var webPages = pages.Select(page =>
            page == null ? null :
            new WebPage()
            {
                JsonNode = page,
                Name = page["name"]?.GetValue<string>(),
                Url = page["url"]?.GetValue<string>(),
                Snippet = page["snippet"]?.GetValue<string>(),
                DateLastCrawled = page["dateLastCrawled"]?.GetValue<DateTimeOffset>(),
            })
            .Where(page => page != null);

        return webPages!;
    }

    public async Task<IEnumerable<WebImage>> SearchImageAsync(string keyword, string market = "ja-JP", int resultCount = 10)
    {
        //https://docs.microsoft.com/ja-jp/bing/search-apis/bing-image-search/quickstarts/rest/csharp

        string endpoint = $"{_endpoint}/images/search";
        string url = $"{endpoint}?q={Uri.EscapeDataString(keyword)}&mkt={market}&count={resultCount}";

        var json = await this.SearchAsync(url, _subscriptionKey).ConfigureAwait(false);

        var imgs = json["value"]?.AsArray();

        if (imgs == null)
        {
            return Enumerable.Empty<WebImage>();
        }

        var images = imgs.Select(img =>
            img == null ? null :
            new WebImage()
            {
                JsonNode = img,
                Name = img["name"]?.GetValue<string>(),
                ThumbnailUrl = img["thumbnailUrl"]?.GetValue<string>(),
                ContentUrl = img["contentUrl"]?.GetValue<string>(),
                DatePublished = img["datePublished"]?.GetValue<DateTimeOffset>(),
            })
            .Where(img => img != null);

        return images!;
    }

    public async Task<IEnumerable<WebVideo>> SearchVideoAsync(string keyword, string market = "ja-JP", int resultCount = 10)
    {
        //https://docs.microsoft.com/ja-jp/bing/search-apis/bing-video-search/quickstarts/rest/csharp

        string endpoint = $"{_endpoint}/videos/search";
        string url = $"{endpoint}?q={Uri.EscapeDataString(keyword)}&mkt={market}&count={resultCount}";

        var json = await this.SearchAsync(url, _subscriptionKey).ConfigureAwait(false);

        var vdos = json["value"]?.AsArray();

        if (vdos == null)
        {
            return Enumerable.Empty<WebVideo>();
        }

        var videos = vdos.Select(vdo =>
            vdo == null ? null :
            new WebVideo()
            {
                JsonNode = vdo,
                Name = vdo["name"]?.GetValue<string>(),
                ThumbnailUrl = vdo["thumbnailUrl"]?.GetValue<string>(),
                ContentUrl = vdo["contentUrl"]?.GetValue<string>(),
                Description = vdo["description"]?.GetValue<string>(),
                Publisher = vdo["publisher"]?[0]?["name"]?.GetValue<string>(),
                Creator = vdo["creator"]?["name"]?.GetValue<string>(),
                ViewCount = vdo["viewCount"]?.GetValue<long>(),
                DatePublished = vdo["datePublished"]?.GetValue<DateTimeOffset>(),
            })
            .Where(vdo => vdo != null);

        return videos!;
    }

    public async Task<IEnumerable<WebNews>> SearchNewsAsync(string keyword, string market = "ja-JP", int resultCount = 10)
    {
        //https://docs.microsoft.com/ja-jp/bing/search-apis/bing-news-search/quickstarts/rest/csharp

        string endpoint = $"{_endpoint}/news/search";
        string url = $"{endpoint}?q={Uri.EscapeDataString(keyword)}&mkt={market}&count={resultCount}";

        var json = await this.SearchAsync(url, _subscriptionKey).ConfigureAwait(false);

        var someNs = json["value"]?.AsArray();

        if (someNs == null)
        {
            return Enumerable.Empty<WebNews>();
        }

        var newsList = someNs.Select(ns =>
            ns == null ? null :
            new WebNews()
            {
                JsonNode = ns,
                Name = ns["name"]?.GetValue<string>(),
                Url = ns["url"]?.GetValue<string>(),
                Description = ns["description"]?.GetValue<string>(),
                ThumbnailUrl = ns["image"]?["thumbnail"]?["contentUrl"]?.GetValue<string>(),
                Category = ns["category"]?.GetValue<string>(),
                DatePublished = ns["datePublished"]?.GetValue<DateTimeOffset>(),
            })
            .Where(ns => ns != null);

        return newsList!;
    }

    private async Task<JsonNode> SearchAsync(string url, string subscriptionKey)
    {

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
        request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

        HttpResponseMessage response = await _httpClient.SendAsync(request).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        string responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var jsonNode = JsonSerializer.Deserialize<JsonNode>(responseString);

        if (jsonNode == null)
        {
            throw new InvalidDataException("The response format is invalid.");
        }

        return jsonNode;
    }
}