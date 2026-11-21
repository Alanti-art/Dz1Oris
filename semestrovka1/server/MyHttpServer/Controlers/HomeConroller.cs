using ConsoleApp;
using MiniHttpServer.ORM;
using MyHttpServer.Controlers.frameworks;
using MyHttpServer.Controlers.frameworks.HttpResponses;
using MyHttpServer.Controlers.frameworks.Аttributes;
using MyHttpServer.Models;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text.Json;

namespace MyHttpServer.Controllers
{
    [HttpController("home")]
    public class HomeController : ControllerBase
    {
        private static readonly HtmlTemplateRenderer Renderer = new HtmlTemplateRenderer();
        private static string ConnectionString
        {
            get
            {
                var settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText("settings.json"));
                return settings.ConnectionString;
            }
        }


        [GET("")]
        public IHttpResponseTypeResult Main()
        {
            var orm = new OrmContext(ConnectionString);
            var comments = orm.GetAllAsync<Comment>("Comments").Result;
            var rules = orm.GetAllAsync<Models.Rule>("Rules").Result;
            Console.WriteLine($"Количество правил из БД: {rules.Count}");

            var commentsView = new List<object>();
            foreach (Comment comment in comments)
            {
                commentsView.Add(new
                {
                    Name = WebUtility.HtmlEncode(comment.Name),
                    Text = WebUtility.HtmlEncode(comment.Text),
                    Rating = WebUtility.HtmlEncode(comment.Rating),
                    HasRating = comment.HasRating(),
                    CreatedAt = comment.CreatedAt.ToString("dd.MM.yyyy HH:mm")
                });
            }


            var rulesView = new List<object>();
            foreach (Models.Rule rule in rules)
            {
                rulesView.Add(new
                {
                    SortOrder = rule.SortOrder,
                    Title = WebUtility.HtmlEncode(rule.Title),
                    Description = WebUtility.HtmlEncode(rule.Description)

                });
            }



            string template = File.ReadAllText(
                Path.Combine(AppContext.BaseDirectory, "static", "main.html"));

            string html = Renderer.RenderFromString(template, new
            {
                Comments = commentsView,
                HasComments = commentsView.Count > 0,
                Rules = rulesView,
                HasRules = rulesView.Count > 0
            });

            return Html(html);
        }
    }
}

