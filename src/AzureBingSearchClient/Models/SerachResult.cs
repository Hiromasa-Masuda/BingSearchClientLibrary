using System.Text.Json.Nodes;

namespace AzureBingSearchClient.Models;

public class SerachResult
{
    public JsonNode? JsonNode;
}

// Web ページ検索結果
public class WebPage : SerachResult
{
    public string? Name;
    public string? Url;
    public string? Snippet;
    public DateTimeOffset? DateLastCrawled;
}

// 画像検索結果
public class WebImage : SerachResult
{
    public string? Name;
    public string? ThumbnailUrl;
    public string? ContentUrl;
    public DateTimeOffset? DatePublished;
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
    public DateTimeOffset? DatePublished;
}

// News 検索結果
public class WebNews : SerachResult
{
    public string? Name;
    public string? Url;
    public string? Description;
    public string? ThumbnailUrl;
    public string? Category;
    public DateTimeOffset? DatePublished;
}
