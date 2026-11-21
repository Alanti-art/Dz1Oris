using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyHttpServer.Controlers.frameworks.HttpResponses
{
    public class JsonResult : IHttpResponseTypeResult
    {
        private readonly object _data;
        private readonly object _statusCode;
        public JsonResult(object data, int statusCode = 200)
        {
            _data = data;
            _statusCode = statusCode;
        }
        public byte[] Execute(HttpListenerContext context)
        {
            var json = JsonSerializer.Serialize(_data);
            context.Response.ContentType = "application/json";
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            return buffer;
        }

    }
}
