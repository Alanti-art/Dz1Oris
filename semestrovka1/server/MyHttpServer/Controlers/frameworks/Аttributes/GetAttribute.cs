using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Controlers.frameworks.Аttributes
{

    public class GETAttribute : Attribute
    {
        public string Route { get; init; }

        public GETAttribute(string route)
        {
            Route = route;
        }
    }
}

