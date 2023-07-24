using AzureBingSearchClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BingSearchDemo001;

class Program
{
    static async Task Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();

        var subscriptionKey = configuration["bingSearch:subscriptionKey"];
        
        string keyword = "猫";

        BingClient bingClient = new BingClient(subscriptionKey);

        var webPages = await bingClient.SearchWeb(keyword);
        webPages.ToList().ForEach(w => 
        {
            Console.WriteLine($"ページ タイトル: {w.Name}");
            Console.WriteLine($"概要: {w.Snippet}");
            Console.WriteLine($"URL: {w.Url}");
            Console.WriteLine();
        });            

        var images = await bingClient.SearchImage(keyword);
        images.ToList().ForEach(img =>
        {
            Console.WriteLine($"画像タイトル: {img.Name}");
            Console.WriteLine($"URL: {img.ContentUrl}");
            Console.WriteLine();
        });

        await Task.Delay(1000);

        var videos = await bingClient.SearchVideo(keyword);
        videos.ToList().ForEach(v =>
        {
            Console.WriteLine($"動画タイトル: {v.Name}");
            Console.WriteLine($"概要: {v.Description}");
            Console.WriteLine($"サムネイル URL: {v.ThumbnailUrl}");
            Console.WriteLine($"URL: {v.ContentUrl}");
            Console.WriteLine($"視聴数: {v.ViewCount}");
            Console.WriteLine();
        });

        var news = await bingClient.SearchNews(keyword);
        news.ToList().ForEach(n =>
        {
            Console.WriteLine($"ニュース タイトル: {n.Name}");
            Console.WriteLine($"概要: {n.Description}");
            Console.WriteLine($"サムネイル URL: {n.ThumbnailUrl}");
            Console.WriteLine($"URL: {n.Url}");                
            Console.WriteLine();
        });

        Console.ReadKey();
    }
}
