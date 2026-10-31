using CustomHttpServer.Framework.Attributes;
using CustomHttpServer.Framework.HttpResponses;
using MyHttpServer.frameworks;
using MyHttpServer.frameworks.HttpResponses;
using MyHttpServer.frameworks.Аttributes;
using System;
using System.IO;
using CustomHttpServer.Framework;

namespace CustomHttpServer.Controllers
{
    [HttpController("auth")]
    public class AuthController : ControllerBase
    {
        [GET("login")]
        public string login()
        {
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "static", "login.html"));
        }

        [POST("login")]
        public void login([FormData] string login, [FormData] string password)
        {
            Console.WriteLine($"Получен login: {login}; пароль {password}.");
        }

        [GET("html")]
        public IHttpResponseTypeResult Html()
        {
            string html = """
                <!doctype html>
                <html lang="ru"><meta charset="utf-8"><title>Результат формы</title>
                <body>
                </body></html>
                """;

            return Html(html);
        }

        [GET("json")]
        public IHttpResponseTypeResult Json()
        {
            var json = new { Username = "user", Password = "12345", Age = 13 };

            return Json(json);
        }

        [GET("json2")]
        public IHttpResponseTypeResult Json2()
        {
            var json = new { Username = "user", Age = 13 };   
            return Json(json);
        }
    }
}