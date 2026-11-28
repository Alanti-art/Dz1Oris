using ConsoleApp;
using CustomHttpServer.Framework.Attributes;
using HtmlAgilityPack;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;
using MiniHttpServer.ORM;
using MyHttpServer.frameworks;
using MyHttpServer.frameworks.HttpResponses;
using MyHttpServer.frameworks.Аttributes;
using MyHttpServer.Models;
using System.Data.SqlClient;
using System.Globalization;
using System.Numerics;
using System.Xml.Linq;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using User = MyHttpServer.Models.User;

namespace MiniHttpServer.Controllers;

[HttpController("user")]
public sealed class UserController : ControllerBase
{
    static string connectionString = "Server=localhost,1433;Database=OrisDb;User Id=sa;Password=*Sveta22052006*;Encrypt=True;TrustServerCertificate=True;";
    



    [GET("list")]
    public IHttpResponseTypeResult List()
    {
        var orm = new OrmContext(connectionString);
        var users = orm.GetAllAsync<User>("Users").Result;

        HtmlTemplateRenderer HtmlTemplateRenderer = new HtmlTemplateRenderer();

        string template = File.ReadAllText(@"\Temlates\User\List.html");

        //"<html lang=\"ru\"><meta charset=\"utf-8\"><h2>Пользователи</h2>\n" +
        //  "$foreach (user in users)\n" +
        //  "<p>${user.Id}: ${user.Name}: ${user.Email}: ${user.Age}</p>\n" +
        //  "$endforeach";


        string result = HtmlTemplateRenderer.RenderFromString(template, new { users });

        return Html(result);



    }
    [GET("delete")]
    public IHttpResponseTypeResult Delete(int id)
    {

        var orm = new OrmContext(connectionString);
        int countOfDel = orm.DeleteAsync("Users", id).Result;


        return Json(countOfDel);
    }

    [GET("update")]
    public IHttpResponseTypeResult Update(int id, string name, string email, int age)
    {
        User user = new User(id, name, email, age);

        var orm = new OrmContext(connectionString);
        int countOfUp = orm.UpdateAsync("Users", id, user).Result;

        return Json(countOfUp);
    }
    [GET("insert")]
    public IHttpResponseTypeResult Insert(int id, string name, string email, int age)
    {
        User user = new User(id, name, email, age);

        var orm = new OrmContext(connectionString);
        int countOfIns = orm.InsertAsync("Users", user).Result;

        return Json(countOfIns);
    }
    [GET("profile")]
    [Authorize]
    public IHttpResponseTypeResult Profile()
    {
        //return Html("<html lang=\"ru\"><meta charset=\"utf-8\"><h1>Личный кабинет</h1>");
        string html = """
        <html lang="ru"><meta charset="utf-8"><h1>Личный кабинет</h1>
        <h1>Личный кабинет</h1>
        <form method="post" action="/auth/logout">
            <button type="submit">Выйти</button>
        </form>
        """;

        return Html(html);
    }
}

