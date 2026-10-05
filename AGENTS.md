# AGENTS.md

## Проект

Экранный переводчик для Windows 10 2004+ / Windows 11. C#, .NET 10, Windows Forms;
интерфейс на русском языке, стандартное оформление и системный шрифт Windows.
Основные действия — отдельные кнопки «Выделить область» и «Весь экран»; горячая
клавиша по умолчанию предлагает выделение. Исходный язык перевода
определяется автоматически. OCR выполняется локально через `Windows.Media.Ocr`.

## Структура

- `simplePC_screen_translate/simplePC_screen_translate.slnx` — решение.
- `simplePC_screen_translate/simplePC_screen_translate` — приложение `ScreenTranslator`.
- `Form1.cs` — сценарий перевода, трей, отмена и жизненный цикл.
- `Form1.Designer.cs` — разметка всех вкладок, совместимая с конструктором Windows Forms.
- `Form1.Settings.cs` — чтение/применение настроек и предпросмотр.
- `Models` — настройки, языки и прямоугольники текста в физических пикселях.
- `Services` — настройки JSON, захват экрана, OCR, перевод и кеш.
- `Native` — Win32 для глобальных горячих клавиш и layered window.
- `UI` — выделение области и отрисовка оверлея с альфа-каналом.
- `UI/HotkeyTextBox.cs` — запись клавиши/сочетания при фокусе в поле; Esc отменяет изменение.
- `simplePC_screen_translate/ScreenTranslator.Tests` — исполняемые проверки без тестовых NuGet-пакетов.
- `artifacts` — игнорируемые Git локальные сборки и изображения проверок.

## Команды из корня репозитория

```powershell
dotnet restore simplePC_screen_translate/simplePC_screen_translate.slnx
dotnet build simplePC_screen_translate/simplePC_screen_translate.slnx -c Release --no-restore
dotnet run --project simplePC_screen_translate/ScreenTranslator.Tests -c Release --no-restore
dotnet run --project simplePC_screen_translate/ScreenTranslator.Tests -c Release --no-restore -- --live-only
dotnet publish simplePC_screen_translate/simplePC_screen_translate -c Release --no-restore -o artifacts/app
```

Если nuget.org недоступен, можно использовать официальный источник Microsoft:
`dotnet restore simplePC_screen_translate/simplePC_screen_translate.slnx --source https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-public/nuget/v3/index.json`.
Проверки `--live-only` отправляют только подготовленные тестовые фразы во внешние сервисы.
Обычный набор проверяет реальные OCR и WinForms на Windows, без сети.

## Правила изменения

- Сохранять стандартное оформление; не добавлять темы или веб-интерфейс.
- Задавать горячую клавишу нажатием в одном поле, без списка клавиш и флажков модификаторов.
- В `InitializeComponent` использовать обычные конструкции, поддерживаемые конструктором Visual Studio.
  Не добавлять вычисления, `??`, сокращённые `new(...)` или запуск служб в эту разметку.
  В режиме проектирования не читать настройки и не запускать HTTP, OCR, трей или глобальные клавиши.
- Любые параметры оформления оверлея должны быть доступны в окне настроек и сохраняться.
- Снимки обрабатываются в памяти; внешним переводчикам передаётся только распознанный текст.
- Не сохранять токены, распознанный текст и снимки в логах или репозитории.
- Настройки находятся в `%LOCALAPPDATA%/ScreenTranslator/settings.json`; запись атомарная, с резервной копией.
- OCR и HTTP не должны блокировать поток интерфейса. Поддерживать отмену и освобождение ресурсов.
- Координаты OCR/захвата/оверлея — физические пиксели; учитывать несколько мониторов и отрицательные координаты.
- Фон имеет собственную прозрачность; текст остаётся непрозрачным. Оверлей пропускает клики и не получает фокус.
- Веб-авторизация Google/Яндекса не является стабильным публичным API. Сохранять ограниченные повторы,
  обновление истёкших токенов, понятные ошибки и явно включаемый резервный переводчик.
- Соблюдать MPL 2.0 для `WebTranslationClient.cs`; атрибуция и лицензия в `THIRD_PARTY_NOTICES.md` и `licenses`.
- После изменений ядра запускать подходящие проверки. После правок интерфейса просматривать PNG всех вкладок в `artifacts/checks`.
