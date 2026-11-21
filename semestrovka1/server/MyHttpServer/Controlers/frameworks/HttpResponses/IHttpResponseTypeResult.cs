using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Controlers.frameworks.HttpResponses
{
    public interface IHttpResponseTypeResult
    {
        byte[] Execute(HttpListenerContext context);

    }
}
