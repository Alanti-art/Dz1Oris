using System.Net;
using System.Text;

namespace MyHttpServer.frameworks.HttpResponses
{
    public class StatusResult : IHttpResponseTypeResult
    {
        private readonly int _code;
        private readonly string _message;
        private readonly string _location;

        public StatusResult(int code, string message = "", string location = null)
        {
            _code = code;
            _message = message;
            _location = location;
        }

        public byte[] Execute(HttpListenerContext context)
        {
            context.Response.StatusCode = _code;

            if (_location != null)
            {
                context.Response.RedirectLocation = _location;
            }

            if (_message == "")
            {
                return new byte[0];
            }

            context.Response.ContentType = "text/plain; charset=utf-8";
            return Encoding.UTF8.GetBytes(_message);
        }
    }
}
