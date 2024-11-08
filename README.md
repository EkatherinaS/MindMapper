# Запуск .NET проекта через Docker Compose и Visual Studio

## Запуск .NET проекта через Docker Compose

1. Убедитесь, что Docker установлен на вашем компьютере.
   
2. Перейдите в корневую папку MindMapper.WebAp, где находится `compose.yaml`.

3. Выполните команду для сборки контейнеров:
   ```bash
   docker compose build
   docker compose up
   ```

## Запуск .NET проекта локально через Visual Studio

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

