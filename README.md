# News Parser

Проект для парсинга новостей из HTML-файлов с ошибками разметки. Приложение извлекает корректные новости, фильтрует мусорные блоки и сохраняет результат в JSON формате.

## Требования

- .NET SDK 8.0
- Rider / Visual Studio

## Результаты работы

После успешного выполнения программа создаст следующие файлы в папке Resources:

- clean-news.json
```json
[
  {
    "title": "Global Summit Results",
    "url": "https://brokennews.net/news/global-summit",
    "date": "2025-06-22"
  },
  {
    "title": "Economic Crisis 2025",
    "url": "https://brokennews.net/news/economy-crisis",
    "date": "2025-06-20"
  }
]
```

- log.txt
```text
Лог парсинга новостей - 2025-10-04 19:19:55
--------------------------------------------------
[INFO] 19:19:55 - Запуск парсера новостей
[INFO] 19:19:55 - Файл ../../../Resources/corrupted-news.html успешно загружен (3026 символов)
[INFO] 19:19:55 - Найдено потенциальных контейнеров новостей: 10
[SKIPPED] 19:19:55 - Пропущен блок: class: 'news-item', text: 'Buy crypto now!'. Причина: Отсутствует заголовок
[SKIPPED] 19:19:55 - Пропущен блок: class: 'news-item', text: 'Political Elections 2025'. Причина: Отсутствует или невалидная дата
[SKIPPED] 19:19:55 - Пропущен блок: class: 'news-item', text: '18 June 2025...'. Причина: Отсутствует ссылка
[SKIPPED] 19:19:55 - Пропущен блок: class: 'news-item', text: '15 June 2025'. Причина: Отсутствует заголовок
[SKIPPED] 19:19:55 - Пропущен блок: class: 'news-item', text: 'This is not a real news item'. Причина: Отсутствует заголовок
[INFO] 19:19:55 - Успешно собрано новостей: 5
[INFO] 19:19:55 - Результаты сохранены в /home/hopmakk/RiderProjects/NewsParser/NewsParser/Resources/clean-news.json
[INFO] 19:19:55 - Парсинг завершен. Сохранено новостей: 5
```

## Структура проекта
```text
NewsParser/
├── Program.cs                # Вход
├── Models/
│   └── NewsItem.cs           # Модель новости
├── Resources/
│   ├── corrupted-news.html   # Входной HTML файл (уже есть в проекте)
│   ├── clean-news.json       # Выходной файл (создается автоматически)
│   └── log.txt               # Лог-файл (создается автоматически)
├── Services/
│   ├── HtmlParser.cs         # Основной парсер
│   ├── TextCleaner.cs        # Очистка текста
│   ├── Loader.cs             # Загрузка html и выгрузка json
│   └── Logger.cs             # Логирование
├── Utilities/
│   └── Constants.cs          # Константы и настройки
└── NewsParser.csproj         

```
