# «Переворот» — веб-приложение (MyHttpServer)

Учебный проект: сервер на C# (`HttpListener`) с собственным роутингом/шаблонизатором и страницами на чистом HTML/CSS.  
Тема: настольная игра «Переворот» (описание, правила, персонажи/карты, комментарии, админ-панель).

## Состав проекта

- `server/MyHttpServer.sln` — решение (.NET 8).
- `server/MyHttpServer/` — веб-приложение (статические страницы в `static/`, контроллеры, настройки `settings.json`).
- `server/MiniHttpServer.ORM/` — простой ORM для работы с SQL Server.
- `server/MiniTemplateEngine/` — шаблонизатор HTML.
- `server/MyHttpServer.UnitTests/`, `server/HttpTemplateRenderTestUnit/` — тесты.
- `db/init/` — SQL-скрипты создания БД и начальных данных.
- `Dockerfile`, `docker-compose.yml`, `docker/` — запуск приложения в контейнере.

## Требования

- **.NET SDK 8.0** (для локального запуска и тестов)
- **Microsoft SQL Server** (локально или в Docker). Для тестов ORM/интеграционных тестов используется LocalDB `(localdb)\MSSQLLocalDB` (Windows/VS).
- **Docker Desktop** (опционально, для запуска в контейнерах)

## База данных

Приложение использует базу данных **`CardGame`** и таблицы: `Users`, `Characters`, `Comments`, `Rules`.

### SQL-скрипты

- `db/init/01-create-db.sql` — создаёт БД `CardGame` (если нет) и таблицы.
- `db/init/02-seed.sql` — заполняет начальными данными (пользователь, персонажи, правила, комментарии).

Выполнение вручную (через `sqlcmd` или SSMS):

```bash
# пример через sqlcmd (подставьте свой сервер/учётные данные)
sqlcmd -S localhost,1433 -U sa -P "<PASSWORD>" -C -i db/init/01-create-db.sql
sqlcmd -S localhost,1433 -U sa -P "<PASSWORD>" -C -i db/init/02-seed.sql
```

> Учётная запись по умолчанию из seed-скрипта: логин `admin`, пароль `admin` (для входа в админ-панель).

## Запуск в Docker (рекомендуется)

Поднимаются два контейнера: SQL Server и приложение.

```bash
docker compose up --build
```

- Приложение: http://localhost:8888/
- Главная страница: http://localhost:8888/main.html (или http://localhost:8888/home)
- Список персонажей: http://localhost:8888/characters/list
- Админ-панель (после входа): http://localhost:8888/auth/login → `/characters/manage`
- SQL Server (для подключения извне): `localhost,1433`, логин `sa`, пароль `YourStrong!Passw0rd` (см. `docker-compose.yml` и `docker/settings.docker.json`)

При первом старте скрипты из `db/init/` автоматически применяются контейнером БД (через `docker/init-db.sh`).

Остановка:

```bash
docker compose down
# чтобы удалить данные БД:
# docker compose down -v
```

> **Примечание:** текущее приложение интерактивное — при запуске ожидает ввод команды `start` в консоли (исходный код не менялся). В `docker-compose.yml` это обрабатывается автоматически (`echo start` подаётся на stdin, ввод держится открытым). При самостоятельном запуске образа учитывайте это поведение.

## Локальный запуск (без Docker)

1. Убедитесь, что SQL Server запущен и доступен.
2. Создайте БД и таблицы, выполните seed-скрипты из `db/init/` (см. выше).
3. Настройте строку подключения в `server/MyHttpServer/settings.json`:
   - `Host` — адрес привязки (например, `127.0.0.1`)
   - `Port` — порт (например, `8888`)
   - `ConnectionString` — строка подключения к вашей БД `CardGame`
4. Запустите приложение:

```bash
dotnet run --project server/MyHttpServer/MyHttpServer.csproj
```

5. В консоли приложения введите команду:

```
start
```

6. Откройте в браузере: `http://127.0.0.1:8888/` (или `http://127.0.0.1:8888/home`, `/main.html`).

Остановка сервера: введите `exit` в консоли приложения.

## Тесты

```bash
dotnet test server/HttpTemplateRenderTestUnit/HttpTemplateRenderTestUnit.csproj
dotnet test server/MyHttpServer.UnitTests/MyHttpServer.UnitTests.csproj
```

- Часть тестов (ORM/интеграционные) требует доступный **SQL Server LocalDB**: `(localdb)\MSSQLLocalDB`.
- Запускать из корня репозитория или с корректной рабочей директорией (некоторым тестам/логике нужны файлы настроек и `static/`).

## Основные страницы и маршруты

- `/` или `/home` — главная (описание, правила, комментарии)
- `/characters/list` — список/поиск персонажей
- `/characters/card?id={id}` — карточка персонажа
- `/characters/manage`, `/characters/new`, `/characters/edit?id={id}` — админ-панель (требуется вход)
- `/auth/login` — вход (`admin` / `admin` при использовании seed-данных)
- `/comment/add` — добавление комментария (POST, форма на главной)

## Примечания

- Статические файлы раздаются из `server/MyHttpServer/static/` (копируются в output при сборке).
- Настройки приложения читаются из `settings.json` в рабочей директории запуска.
- В Docker используется отдельный конфиг `docker/settings.docker.json` (копируется как `settings.json` в образе). При необходимости измените пароль `MSSQL_SA_PASSWORD` согласованно в `docker-compose.yml`, `docker/settings.docker.json` и `docker/init-db.sh`.
