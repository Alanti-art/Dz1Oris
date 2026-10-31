using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.frameworks.Аttributes
{

    public class HttpController : Attribute
    {
        public string Route { get; init; }

        public HttpController(string route)
        {
            Route = route;
        }

    }
}
