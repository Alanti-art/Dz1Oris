using MyHttpServer.frameworks.HttpResponses;
using System.Net;
using System.Text;
using System.Text.Json;

namespace CustomHttpServer.Framework.HttpResponses
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
            var json = JsonSerializer.Serialize(_html);

            context.Response.ContentType = "text/html";

            byte[] buffer = Encoding.UTF8.GetBytes(json);

            return buffer;
        }
    }
}
