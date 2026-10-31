using CustomHttpServer.Framework.HttpResponses;
using MyHttpServer.frameworks.HttpResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CustomHttpServer.Framework.HttpResponses;
using System.Threading.Tasks;

namespace MyHttpServer.frameworks
{
    public class ControllerBase
    {
        protected HttpListenerContext Context { get; private set; }

        internal void SetContext(HttpListenerContext context)
        {
            Context = context;
        }



        protected IHttpResponseTypeResult Json(object obj) => new JsonResult(obj);
        protected IHttpResponseTypeResult Html(string html) => new HtmlResult(html);



}
}
