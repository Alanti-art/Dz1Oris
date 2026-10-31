using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.frameworks.Аttributes
{

    public class PUTAttribute : Attribute
    {
        public string Route { get; init; }

        public PUTAttribute(string route)
        {
            Route = route;
        }

    }




}
