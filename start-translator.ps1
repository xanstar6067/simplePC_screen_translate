$ErrorActionPreference = 'Stop'
$translatorExe = Join-Path $PSScriptRoot 'artifacts\app\ScreenTranslator.exe'
if (-not (Test-Path -LiteralPath $translatorExe)) {
    throw 'Сначала соберите приложение: dotnet publish simplePC_screen_translate/simplePC_screen_translate -c Release -o artifacts/app'
}
Start-Process -FilePath $translatorExe -WorkingDirectory (Split-Path -Parent $translatorExe) -WindowStyle Normal
