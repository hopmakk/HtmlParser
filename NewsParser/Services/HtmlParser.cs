using HtmlAgilityPack;
using NewsParser.Models;
using NewsParser.Utilities;

namespace NewsParser.Services;

public class HtmlParser
    {
        private readonly TextCleaner _textCleaner;
        private readonly Logger _logger;

        public HtmlParser(Logger logger)
        {
            _textCleaner = new TextCleaner();
            _logger = logger;
        }

        public List<NewsItem> ParseNews(string htmlContent)
        {
            var newsItems = new List<NewsItem>();
            
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);

                var newsContainers = FindNewsContainers(doc);
                _logger.LogInfo($"Найдено потенциальных контейнеров новостей: {newsContainers.Count}");

                foreach (var container in newsContainers)
                {
                    var newsItem = ParseSingleNewsItem(container);
                    
                    if (ValidateNewsItem(newsItem, container))
                    {
                        newsItems.Add(newsItem);
                    }
                }

                _logger.LogInfo($"Успешно собрано новостей: {newsItems.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при парсинге HTML: {ex.Message}");
            }

            return newsItems;
        }

        private List<HtmlNode> FindNewsContainers(HtmlDocument doc)
        {
            var containers = new List<HtmlNode>();

            foreach (var selector in Constants.NewsContainerSelectors)
            {
                try
                {
                    var nodes = doc.DocumentNode.SelectNodes($"{selector}");
                    if (nodes != null)
                    {
                        containers.AddRange(nodes);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при поиске по селектору '{selector}': {ex.Message}");
                }
            }

            return containers.Distinct().ToList();
        }

        private NewsItem ParseSingleNewsItem(HtmlNode container)
        {
            return new NewsItem
            {
                Title = ExtractTitle(container),
                Url = ExtractUrl(container),
                Date = ExtractDate(container)
            };
        }

        private string ExtractTitle(HtmlNode container)
        {
            try
            {
                // Ищем по стандартной структуре .news-title -> h4 -> a
                var titleNode = container.SelectSingleNode(".//*[contains(@class, 'news-title')]//a");

                if (titleNode != null && !string.IsNullOrWhiteSpace(titleNode.InnerText))
                {
                    return _textCleaner.CleanText(titleNode.InnerText);
                }

                // Ищем любой текст в заголовочных тегах
                var headerNodes = container.SelectNodes(".//h1|.//h2|.//h3|.//h4|.//h5|.//h6");
                if (headerNodes != null)
                {
                    foreach (var header in headerNodes)
                    {
                        var text = header.InnerText;
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            return _textCleaner.CleanText(text);
                        }
                    }
                }

                // Ищем первую ссылку с текстом
                var firstLink = container.SelectSingleNode(".//a[text()]");
                if (firstLink != null && !string.IsNullOrWhiteSpace(firstLink.InnerText))
                {
                    return _textCleaner.CleanText(firstLink.InnerText);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при извлечении заголовка: {ex.Message}");
            }

            return string.Empty;
        }

        private string ExtractUrl(HtmlNode container)
        {
            try
            {
                // Ищем ссылку с атрибутом href
                var linkNode = container.SelectSingleNode(".//a[@href]");
                if (linkNode != null)
                {
                    var href = linkNode.GetAttributeValue("href", string.Empty);
                    if (!string.IsNullOrWhiteSpace(href))
                    {
                        // Преобразуем в абсолютный URL
                        if (href.StartsWith("/"))
                        {
                            return Constants.BaseUrl + href;
                        }
                        return _textCleaner.CleanText(href);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при извлечении URL: {ex.Message}");
            }

            return string.Empty;
        }

        private string ExtractDate(HtmlNode container)
        {
            try
            {
                // Ищем тег time с атрибутом datetime
                var timeNode = container.SelectSingleNode(".//time[@datetime]");
                if (timeNode != null)
                {
                    var datetime = timeNode.GetAttributeValue("datetime", string.Empty);
                    if (!string.IsNullOrWhiteSpace(datetime))
                    {
                        // Парсим дату в нужном формате
                        if (DateTime.TryParse(datetime, out DateTime date))
                        {
                            return date.ToString("yyyy-MM-dd");
                        }
                    }
                }

                // ищем текст в теге time
                timeNode = container.SelectSingleNode(".//time");
                if (timeNode != null && !string.IsNullOrWhiteSpace(timeNode.InnerText))
                {
                    var dateText = _textCleaner.CleanText(timeNode.InnerText);
                    if (DateTime.TryParse(dateText, out DateTime date))
                    {
                        return date.ToString("yyyy-MM-dd");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при извлечении даты: {ex.Message}");
            }

            return string.Empty;
        }

        private bool ValidateNewsItem(NewsItem item, HtmlNode container)
        {
            // Проверяем на мусорные блоки
            if (IsJunkContainer(container))
            {
                _logger.LogSkippedItem(GetContainerInfo(container), "Мусорный блок (ad-banner, footer и т.д.)");
                return false;
            }

            // Проверяем обязательные поля
            if (string.IsNullOrWhiteSpace(item.Title))
            {
                _logger.LogSkippedItem(GetContainerInfo(container), "Отсутствует заголовок");
                return false;
            }

            if (string.IsNullOrWhiteSpace(item.Url))
            {
                _logger.LogSkippedItem(GetContainerInfo(container), "Отсутствует ссылка");
                return false;
            }

            if (string.IsNullOrWhiteSpace(item.Date))
            {
                _logger.LogSkippedItem(GetContainerInfo(container), "Отсутствует или невалидная дата");
                return false;
            }

            // Дополнительная проверка на пустые или мусорные заголовки
            if (item.Title.Length < 2 || item.Title.All(char.IsWhiteSpace))
            {
                _logger.LogSkippedItem(GetContainerInfo(container), "Заголовок слишком короткий или состоит из пробелов");
                return false;
            }

            return true;
        }

        private bool IsJunkContainer(HtmlNode container)
        {
            var classAttr = container.GetAttributeValue("class", string.Empty);
            if (!string.IsNullOrWhiteSpace(classAttr))
            {
                foreach (var junkClass in Constants.JunkClasses)
                {
                    if (classAttr.Contains(junkClass, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            
            var text = container.InnerText;

            foreach (var junkWord in Constants.JunkWords)
            {
                if (text.Contains(junkWord, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private string GetContainerInfo(HtmlNode container)
        {
            try
            {
                var classAttr = container.GetAttributeValue("class", "no-class");
                var textPreview = container.InnerText.Trim();
                if (textPreview.Length > 50)
                    textPreview = textPreview.Substring(0, 50).Trim() + "...";
                
                return $"class: '{classAttr}', text: '{textPreview}'";
            }
            catch
            {
                return "неизвестный контейнер";
            }
        }
    }