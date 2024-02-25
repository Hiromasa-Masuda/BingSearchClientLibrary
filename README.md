# Azure Bing Search Client Library

The Azure Bing Search Client Library is a client library for Azure Bing Search v7 resources.

```csharp
string subscriptionKey = "<yourKey>";
// Market code: en-US, ja-JP... 
// https://learn.microsoft.com/rest/api/cognitiveservices-bingsearch/bing-web-api-v7-reference#market-codes
string marketCode = "en-Us";
int resultCount = 10;
string keyword = "cat";

BingClient bingClient = new BingClient(subscriptionKey, marketCode, resultCount);

var webPages = await bingClient.SearchWebAsync(keyword);
var images = await bingClient.SearchImageAsync(keyword);
var videos = await bingClient.SearchVideoAsync(keyword);
var news = await bingClient.SearchNewsAsync(keyword);
```

Please refer to the following article as well:
- https://qiita.com/hiromasa-masuda/items/84f33e39f8761e0e9323