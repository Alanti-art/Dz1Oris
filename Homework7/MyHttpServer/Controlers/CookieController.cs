using CustomHttpServer.Framework.Attributes;
using MyHttpServer.frameworks;
using MyHttpServer.frameworks.HttpResponses;
using MyHttpServer.frameworks.Аttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Controlers
{
    [HttpController("cookie")]
    public class CookieController : ControllerBase
    {
        [GET("set")]
        public IHttpResponseTypeResult Set()
        {
            var cookie = new Cookie(
                "testCookie",
                "HelloWorld",
                "/")
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(30)
            };

            Context.Response.SetCookie(cookie);

            return Html("<html lang=\"ru\"><meta charset=\"utf-8\"><h1>Cookie установлена</h1>");
        }

        [GET("get")]
        public IHttpResponseTypeResult Get()
        {
            string? value =
                Context.Request.Cookies["testCookie"]?.Value;

            return Json(new
            {
                Value = value
            });
        }

        [GET("delete")]
        public IHttpResponseTypeResult Delete()
        {
            var cookie = new Cookie(
                "testCookie",
                string.Empty,
                "/")
            {
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            Context.Response.SetCookie(cookie);

            return Html("<html lang=\"ru\"><meta charset=\"utf-8\"><h1>Cookie удалена</h1>");
        }
    }
}
