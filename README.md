# Запуск .NET проекта

## Запуск СУБД PostgreSQL с помощью Docker Compose

1. Убедитесь, что Docker установлен на вашем компьютере.
   
2. Перейдите в корневую папку MindMapper.WebApp, где находится `compose.yaml`.

3. Выполните команду для сборки контейнеров:
   ```bash
   docker compose build
   docker compose up -d
   ```

## Перед запуском .NET проекта

Необходимо ввести переменные в файл appsettings.json.
```json
{
   "ConnectionStrings": {
      "DefaultConnection": "Host=localhost;Port=54401;Username=pguser;Password=pguserpass;Database=mindmapper_db;"
   },
   "YandexGptOptions": {
      "IamToken": "<IamToken из Yandex Cloud>",
      "Folder": "<параметр Folder в Yandex Cloud>",
      "Temperature": 0.4
   },
   "FileOptions": {
      "SavePath": "<путь до папки с сохраненными pdf документами>"
   }
}
```

## Запуск .NET проекта локально через Visual Studio

Для работы проекта требуется полезная нагрузка `ASP.NET Core`, ее необходимо добавить в Visual Studio Installer. После этого выполните следующие шаги:

1. Откройте Visual Studio.

2. Выберите **File** > **Open** > **Project/Solution...** и найдите файл `.sln` вашего проекта.

3. Убедитесь, что проект компилируется без ошибок:
   - В главном меню выберите **Build** > **Build Solution** или нажмите `Ctrl + Shift + B`.

4. Настройте конфигурацию запуска (например, `Debug` или `Release`), если это необходимо:
   - Выберите конфигурацию из выпадающего списка в верхней части окна Visual Studio.

5. Запустите проект:
   - Нажмите **Start** (зелёная кнопка "Play") или используйте клавишу `F5`.
   - Если не требуется отладка, нажмите **Debug** > **Start Without Debugging** или `Ctrl + F5`.

6. После запуска Visual Studio откроет браузер и направит вас на локальный адрес приложения, `http://localhost:5174`.

## Альтарнативный вариант запуска без IDE

1. Установите и установите .NET 8 SDK с [официального сайта](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) или используя ваш пакетный менеджер 
   - Для Windows 10: `winget install Microsoft.Dotnet.Sdk.8`
   - MacOS: `brew install --cask dotnet-sdk`
   - Linux (Debian): `sudo apt-get install -y dotnet-sdk-8.0`
2. Убедитесь что у вас в системе установлен .NET 8 SDK командой `dotnet --list-sdks`
3. Перейдите в папку `MindMapper.WebApi`, введите команду `dotnet run`
4. Проект запущен и доступен на порте 5147

