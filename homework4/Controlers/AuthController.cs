using CustomHttpServer.Framework.Attributes;
using MyHttpServer.frameworks.Аttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CustomHttpServer.Controllers
{
    [HttpController("auth")]
    public class AuthController
    {
        [Get("login")]
        public string login()
        {


            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "static", "login.html"));
        }

        [Post("login")]
       
        public void login([FormData] string login, [FormData] string password)
        {
            Console.WriteLine($"Получен login: {login}; пароль {password}.");
        }
    }
}
