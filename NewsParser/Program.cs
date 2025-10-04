using System.Text.Json;
using Microsoft.Extensions.Logging;
using NewsParser.Models;
using NewsParser.Services;
using NewsParser.Utilities;

namespace NewsParser;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var logger = new Logger(Constants.LogFilePath);
            var loader = new Loader(logger);
            var parser = new HtmlParser(logger);
            logger.LogInfo("Запуск парсера новостей");
            
            // Загрузка HTML файла
            string htmlContent = loader.LoadHtmlFile("../../../Resources/corrupted-news.html");
            if (string.IsNullOrEmpty(htmlContent))
            {
                logger.LogError("Не удалось загрузить HTML файл");
                return;
            }

            // Парсинг новостей
            List<NewsItem> newsItems = parser.ParseNews(htmlContent);

            // Сохранение результатов
            loader.SaveNewsToJson(newsItems, Constants.OutputFilePath);
            logger.LogInfo($"Парсинг завершен. Сохранено новостей: {newsItems.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критическая ошибка: {ex.Message}");
        }
    }
}