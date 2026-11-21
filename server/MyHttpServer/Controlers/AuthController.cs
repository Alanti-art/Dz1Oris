using ConsoleApp;
using MiniHttpServer.ORM;
using MyHttpServer;
using MyHttpServer.Controlers.frameworks;
using MyHttpServer.Controlers.frameworks.Аttributes;
using MyHttpServer.Controlers.frameworks.HttpResponses;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace CustomHttpServer.Controllers
{
    [HttpController("auth")]
    public class AuthController : ControllerBase
    {
        private static readonly IHtmlTemplateRenderer Renderer = new HtmlTemplateRenderer();

        private static string ConnectionString
        {
            get
            {
                var settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText("settings.json"));
                return settings.ConnectionString;
            }
        }

        [GET("login")]
        public IHttpResponseTypeResult login()
        {

            return RenderLoginPage();
        }

        [POST("login")]
        public IHttpResponseTypeResult login([FormData] string login, [FormData] string password)
        {
            Console.WriteLine($"Получен login: {login}; пароль {password}.");

            
            var orm = new OrmContext(ConnectionString);
            var users = orm.GetAllAsync<User>("Users").Result;

            User? user = null;

            foreach (var u in users)
            {
                if (string.Equals(u.Login, login, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(u.Password, password, StringComparison.Ordinal))
                {
                    user = u;
                    break;
                }
            }


            if (user == null)
            {
                return RenderLoginPage(
                    hasError: true,
                    errorMessage: "Неверный логин или пароль",
                    login: login ?? string.Empty);
            }

            string sessionId = SessionService.CreateSession(user.Id);

            var cookie = new Cookie("sessionId", sessionId, "/")
            {
                HttpOnly = true,
                Secure = Context.Request.IsSecureConnection,
                Expires = DateTime.UtcNow.AddMinutes(30)
            };

            Context.Response.SetCookie(cookie);


           
            Context.Response.StatusCode = 302;
            Context.Response.Headers.Add("Location", "/characters/manage");
            return Html("");
        }


        [POST("logout")]
        public IHttpResponseTypeResult Logout()
        {
            string? sessionId = Context.Request.Cookies["sessionId"]?.Value;

            SessionService.RemoveSession(sessionId);

            var cookie = new Cookie("sessionId", string.Empty, "/")
            {
                HttpOnly = true,
                Secure = Context.Request.IsSecureConnection,
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            Context.Response.SetCookie(cookie);


            Context.Response.StatusCode = 302; 
            Context.Response.Headers.Add("Location", "/characters/list");
            return Html("");
        }

        private IHttpResponseTypeResult RenderLoginPage(
           bool hasError = false,
           string errorMessage = "",
           string login = "")
        {
            string html = Renderer.RenderFromFile(
                Path.Combine(AppContext.BaseDirectory, "static", "login.html"),
                new
                {
                    HasError = hasError,
                    ErrorMessage = WebUtility.HtmlEncode(errorMessage ?? string.Empty),
                    Login = WebUtility.HtmlEncode(login ?? string.Empty)
                });

            return Html(html);
        }



    }
}
