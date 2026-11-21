using ConsoleApp;
using MiniHttpServer.ORM;
using MyHttpServer.Controlers.frameworks;
using MyHttpServer.Controlers.frameworks.Аttributes;
using MyHttpServer.Controlers.frameworks.HttpResponses;
using MyHttpServer.Models;
using System;
using System.IO;
using System.Net;
using System.Text.Json;

namespace MyHttpServer.Controllers
{
    [HttpController("characters")]
    public class CharactersController : ControllerBase
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



        [GET("list")]
        public IHttpResponseTypeResult List()
        {
            var orm = new OrmContext(ConnectionString);
            var characters = orm.GetAllAsync<Character>("Characters").Result;

            var encoded = new List<Character>();
            foreach (Character character in characters)
            {
                encoded.Add(EncodeCharacter(character));
            }

            string template = ReadTemplate("characters.html");
            string html = Renderer.RenderFromString(template, new
            {
                IsAuthenticated = IsAuthenticated(),
                Characters = encoded
            });

            return Html(html);
        }

        
        [GET("card")]
        public IHttpResponseTypeResult Card(int id)
        {
            var orm = new OrmContext(ConnectionString);
            Character character = orm.GetByIdAsync<Character>("Characters", id).Result;

            if (character == null)
            {
                return NotFound();
            }

            Character c = EncodeCharacter(character);

            string template = ReadTemplate("card-template.html");
            string html = Renderer.RenderFromString(template, new
            {
                IsAuthenticated = IsAuthenticated(),
                Id = c.Id,
                Name = c.Name,
                Image = c.Image,
                Description = c.Description,
                Action = c.Action,
                Cost = c.Cost,
                Blocks = c.Blocks,
                BlockedBy = c.BlockedBy
            });

            return Html(html);
        }

       
        [GET("manage")]
        [Authorize]
        public IHttpResponseTypeResult Manage()
        {
            var orm = new OrmContext(ConnectionString);
            var characters = orm.GetAllAsync<Character>("Characters").Result;

            var encoded = new List<Character>();
            foreach (Character character in characters)
            {
                encoded.Add(EncodeCharacter(character));
            }

            string template = ReadTemplate("cards.html");
            string html = Renderer.RenderFromString(template, new
            {
                CardsCount = encoded.Count,
                Cards = encoded
            });

            return Html(html);
        }


   
        [GET("new")]
        [Authorize]
        public IHttpResponseTypeResult New()
        {
            return Html(ReadTemplate("add-card.html"));
        }

        [POST("create")]
        [Authorize]
        public IHttpResponseTypeResult Create([FormData] string name, [FormData] string image,
            [FormData] string description, [FormData] string action, [FormData] string cost,
            [FormData] string blocks, [FormData] string blockedBy)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Имя карточки обязательно");
            }

            Character character = new Character();
            character.Name = name;
            character.Image = image ?? "";
            character.Description = description ?? "";
            character.Action = action ?? "";
            character.Cost = cost ?? "";
            character.Blocks = blocks ?? "";
            character.BlockedBy = blockedBy ?? "";

            var orm = new OrmContext(ConnectionString);
            orm.InsertAsync("Characters", character).Wait();

            return Redirect("/characters/manage");
        }


    
        [GET("edit")]
        [Authorize]
        public IHttpResponseTypeResult Edit(int id)
        {
            var orm = new OrmContext(ConnectionString);
            Character character = orm.GetByIdAsync<Character>("Characters", id).Result;

            if (character == null)
            {
                return NotFound();
            }

            string template = ReadTemplate("edit-card.html");
            string html = Renderer.RenderFromString(template, EncodeCharacter(character));

            return Html(html);
        }

        [POST("update")]
        [Authorize]
        public IHttpResponseTypeResult Update([FormData] string id, [FormData] string name,
            [FormData] string image, [FormData] string description, [FormData] string action,
            [FormData] string cost, [FormData] string blocks, [FormData] string blockedBy)
        {
            int characterId;
            if (!int.TryParse(id, out characterId))
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Имя карточки обязательно");
            }

            Character character = new Character();
            character.Id = characterId;
            character.Name = name;
            character.Image = image ?? "";
            character.Description = description ?? "";
            character.Action = action ?? "";
            character.Cost = cost ?? "";
            character.Blocks = blocks ?? "";
            character.BlockedBy = blockedBy ?? "";

            var orm = new OrmContext(ConnectionString);
            int countOfUp = orm.UpdateAsync("Characters", characterId, character).Result;

            if (countOfUp == 0)
            {
                return NotFound();
            }

            return Redirect("/characters/manage");
        }


        [POST("delete")]
        [Authorize]
        public IHttpResponseTypeResult Delete([FormData] string id)
        {
            int characterId;
            if (!int.TryParse(id, out characterId))
            {
                return NotFound();
            }

            var orm = new OrmContext(ConnectionString);
            int countOfDel = orm.DeleteAsync("Characters", characterId).Result;

            if (countOfDel == 0)
            {
                return NotFound();
            }

            return Redirect("/characters/manage");
        }
        [GET("search")]
        public IHttpResponseTypeResult Search(string name)
        {
            var orm = new OrmContext(ConnectionString);
            var characters = orm.GetAllAsync<Character>("Characters").Result;

            string search = name ?? "";
            var filtered = new List<object>();

            foreach (var c in characters)
            {
                string characterName = c.Name ?? "";

                if (string.IsNullOrWhiteSpace(search) ||
                    characterName.Contains(search, StringComparison.OrdinalIgnoreCase))
                {
                    filtered.Add(new
                    {
                        c.Id,
                        c.Name,
                        c.Image,
                        c.Description
                    });
                }
            }

            return Json(new
            {
                Characters = filtered
            });
        }

        private Character EncodeCharacter(Character c)
        {
            Character result = new Character();
            result.Id = c.Id;
            result.Name = WebUtility.HtmlEncode(c.Name);
            result.Image = WebUtility.HtmlEncode(c.Image);
            result.Description = WebUtility.HtmlEncode(c.Description);
            result.Action = WebUtility.HtmlEncode(c.Action);
            result.Cost = WebUtility.HtmlEncode(c.Cost);
            result.Blocks = WebUtility.HtmlEncode(c.Blocks);
            result.BlockedBy = WebUtility.HtmlEncode(c.BlockedBy);
            return result;
        }

        private bool IsAuthenticated()
        {
            string sessionId = Context.Request.Cookies["sessionId"]?.Value;
            return SessionService.GetUserId(sessionId).HasValue;
        }

        private string ReadTemplate(string fileName)
        {
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "static", fileName));
        }

        private IHttpResponseTypeResult Redirect(string url)
        {
            Context.Response.StatusCode = 302;
            Context.Response.Headers.Add("Location", url);
            return Html("");
        }

        private IHttpResponseTypeResult NotFound()
        {
            Context.Response.StatusCode = 404;
            return Html(ReadTemplate("404.html"));
        }

        private IHttpResponseTypeResult BadRequest(string message)
        {
            Context.Response.StatusCode = 400;
            return Html(WebUtility.HtmlEncode(message));
        }
    }
}
