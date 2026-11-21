using System.Net;
using System.Text;
using System.Text.Json;

namespace MyHttpServer.Controlers.frameworks.HttpResponses
{
    public class HtmlResult : IHttpResponseTypeResult
    {
        private readonly string _html;

        public HtmlResult(string html)
        {
            _html = html;
        }

        public byte[] Execute(HttpListenerContext context)
        {
           

            context.Response.ContentType = "text/html";

            byte[] buffer = Encoding.UTF8.GetBytes(_html);

            return buffer;
        }
    }
}
