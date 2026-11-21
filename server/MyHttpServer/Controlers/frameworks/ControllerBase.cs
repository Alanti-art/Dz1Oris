using MyHttpServer.Controlers.frameworks.Authentication;
using MyHttpServer.Controlers.frameworks.HttpResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Controlers.frameworks
{
    public class ControllerBase
    {
        protected HttpListenerContext Context { get; private set; }
        protected IUserSessionService SessionService
        { get; private set; } = null!;


        internal void Initialize(HttpListenerContext context, IUserSessionService sessionService)
        {
            Context = context;
            SessionService = sessionService;
        }
       


        protected IHttpResponseTypeResult Json(object obj) => new JsonResult(obj);
        protected IHttpResponseTypeResult Html(string html) => new HtmlResult(html);



}
}
