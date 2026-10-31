using CustomHttpServer.Framework.Attributes;
using MyHttpServer.Applications;
using MyHttpServer.frameworks;
using MyHttpServer.frameworks.Аttributes;
using MyHttpServer.frameworks.HttpResponses;
using System.Net;

namespace MyHttpServer.Controlers
{
    [HttpController("applications")]
    public class ApplicationsController : ControllerBase
    {
        public static ApplicationStore Store = new ApplicationStore();

        
        [GET("")]
        public IHttpResponseTypeResult List(string direction)
        {
            string items = "";
            foreach (Application app in Store.Find(direction))
            {
                items += $"<p><a href=\"/applications/details?id={app.Id}\">{WebUtility.HtmlEncode(app.Applicant)}</a> - {app.Direction}, {app.Status}</p>";
            }

            string html = $"""
                <html>
                <meta charset="utf-8">
                <body>
                    <h1>Заявки</h1>

                    <form method="get" action="/applications">
                        <select name="direction">
                            <option value="">все</option>
                            <option value="design">design</option>
                            <option value="backend">backend</option>
                            <option value="testing">testing</option>
                        </select>
                        <button>Показать</button>
                    </form>

                    {items}

                    <h2>Новая заявка</h2>
                    <form method="post" action="/applications/create">
                        Имя: <input name="applicant">
                        <select name="direction">
                            <option value="design">design</option>
                            <option value="backend">backend</option>
                            <option value="testing">testing</option>
                        </select>
                        <button>Отправить</button>
                    </form>
                </body>
                </html>
                """;

            return Html(html);
        }

       
        [GET("details")]
        public IHttpResponseTypeResult Details(string id)
        {
            int appId;
            if (!int.TryParse(id, out appId))
            {
                return new StatusResult(400, "Некорректный id");
            }

            Application app = Store.GetById(appId);
            if (app == null)
            {
                return new StatusResult(404, "Заявка не найдена");
            }

            string html = $"""
                <html>
                <meta charset="utf-8">
                <body>
                    <h1>Заявка {app.Id}</h1>
                    <p>Заявитель: {WebUtility.HtmlEncode(app.Applicant)}</p>
                    <p>Направление: {app.Direction}</p>
                    <p>Статус: {app.Status}</p>
                    <a href="/applications">К списку</a>
                </body>
                </html>
                """;

            return Html(html);
        }

        
        [POST("create")]
        public IHttpResponseTypeResult Create([FormData] string applicant, [FormData] string direction)
        {
            Application app = Store.Create(applicant, direction);

            if (app == null)
            {
                return new StatusResult(400, "Некорректные данные");
            }

            return new StatusResult(303, "", "/applications/details?id=" + app.Id);
        }

    
        [PUT("update")]
        public IHttpResponseTypeResult Update(string id, [FormData] string applicant, [FormData] string direction, [FormData] string status)
        {
            int appId;
            if (!int.TryParse(id, out appId))
            {
                return new StatusResult(400, "Некорректный id");
            }

            if (Store.GetById(appId) == null)
            {
                return new StatusResult(404, "Заявка не найдена");
            }

            bool ok = Store.Update(appId, applicant, direction, status);
            if (!ok)
            {
                return new StatusResult(400, "Некорректные данные");
            }

            return new StatusResult(204);
        }
    }
}
