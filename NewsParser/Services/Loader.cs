using System.Text.Json;
using NewsParser.Models;

namespace NewsParser.Services;

public class Loader
{
    private readonly Logger _logger;
    
    public Loader(Logger logger)
    {
        _logger = logger;
    }
    
    public string LoadHtmlFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                _logger.LogError($"Файл {filePath} не найден");
                return null;
            }

            string content = File.ReadAllText(filePath);
            _logger.LogInfo($"Файл {filePath} успешно загружен ({content.Length} символов)");
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка загрузки файла {filePath}: { ex.Message}");
            return null;
        }
    }

    public void SaveNewsToJson(List<NewsItem> newsItems, string outputPath)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string json = JsonSerializer.Serialize(newsItems, options);
            File.WriteAllText(outputPath, json);
            
            _logger.LogInfo($"Результаты сохранены в {Path.GetFullPath(outputPath)}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка сохранения JSON: {ex.Message}");
        }
    }
}