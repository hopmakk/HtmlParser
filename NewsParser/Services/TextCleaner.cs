using System.Text.RegularExpressions;

namespace NewsParser.Services;

public class TextCleaner
    {
        public string CleanText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            string cleaned = text;
            
            cleaned = DecodeHtmlEntities(cleaned);
            
            cleaned = NormalizeWhitespace(cleaned);
            
            cleaned = cleaned.Trim();
            
            return cleaned;
        }

        private string DecodeHtmlEntities(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var replacements = new (string entity, string replacement)[]
            {
                ("&nbsp;", " "),
                ("&quot;", "\""),
                ("&amp;", "&"),
                ("&lt;", "<"),
                ("&gt;", ">"),
                ("&apos;", "'"),
                ("&copy;", "©"),
                ("&reg;", "®")
            };

            string result = text;
            foreach (var (entity, replacement) in replacements)
            {
                result = result.Replace(entity, replacement);
            }

            result = Regex.Replace(result, @"&#(\d+);", match =>
            {
                if (int.TryParse(match.Groups[1].Value, out int code))
                {
                    return ((char)code).ToString();
                }
                return match.Value;
            });

            return result;
        }

        private string NormalizeWhitespace(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Заменяем множественные пробелы на один
            string result = Regex.Replace(text, @"\s+", " ");
            
            // Удаляем спец символы
            result = Regex.Replace(result, @"[\u0000-\u0008\u000B\u000C\u000E-\u001F\u007F]", "");
            
            return result;
        }
    }