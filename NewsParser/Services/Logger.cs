using Microsoft.Extensions.Logging;

namespace NewsParser.Services;

public class Logger
{
    private readonly string _logFilePath;

    public Logger(string logFilePath)
    {
        _logFilePath = logFilePath;
        Initialize();
    }

    public void Initialize()
    {
        try
        {
            File.WriteAllText(_logFilePath, $"Лог парсинга новостей - {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n");
            File.AppendAllText(_logFilePath, new string('-', 50) + "\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка инициализации логгера: {ex.Message}");
        }
    }

    public void LogError(string message)
    {
        try
        {
            string logEntry = $"[ERROR] {DateTime.Now:HH:mm:ss} - {message}\n";
            File.AppendAllText(_logFilePath, logEntry);
            Console.WriteLine(logEntry);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка записи в лог: {ex.Message}");
        }
    }

    public void LogSkippedItem(string containerInfo, string reason)
    {
        try
        {
            string logEntry =
                $"[SKIPPED] {DateTime.Now:HH:mm:ss} - Пропущен блок: {containerInfo}. Причина: {reason}\n";
            File.AppendAllText(_logFilePath, logEntry);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка записи в лог: {ex.Message}");
        }
    }

    public void LogInfo(string message)
    {
        try
        {
            string logEntry = $"[INFO] {DateTime.Now:HH:mm:ss} - {message}\n";
            File.AppendAllText(_logFilePath, logEntry);
            Console.WriteLine(logEntry);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка записи в лог: {ex.Message}");
        }
    }
}
