# Azure Bing Search Client Library

Azure Bing Search v7 で利用可能なクライアントライブラリです。

```csharp
string subscriptionKey = "<yourKey>";
string keyword = "猫";

BingClient bingClient = new BingClient(subscriptionKey);

var webPages = await bingClient.SearchWebAsync(keyword);
var images = await bingClient.SearchImageAsync(keyword);
var videos = await bingClient.SearchVideoAsync(keyword);
var news = await bingClient.SearchNewsAsync(keyword);

```

詳しくは、以下の記事に記載しています。

[Bing Search v7 クライアント ライブラリがないので、実装してみた](https://qiita.com/hiromasa-masuda/items/84f33e39f8761e0e9323)

