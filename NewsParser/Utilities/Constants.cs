namespace NewsParser.Utilities;

public static class Constants
{
    public const string BaseUrl = "https://brokennews.net";
    public const string OutputFilePath = "../../../Resources/clean-news.json";
    public const string LogFilePath = "../../../Resources/log.txt";
        
    public static readonly string[] NewsContainerSelectors = new[]
    {
        "//li[contains(@class, 'news-item')]",
    };
        
    public static readonly string[] JunkClasses = new[]
    {
        "ad-banner",
        "footer",
        "banner",
        "advertisement",
        "ads"
    };

    public static readonly string[] JunkWords = new []
    {
        "advertisement", 
        "banner", 
        "buy now", 
        "sponsored",
    };
}