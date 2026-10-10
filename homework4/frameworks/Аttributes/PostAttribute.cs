using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.frameworks.Аttributes
{

    public class PostAttribute : Attribute
    {
        public string Route { get; init; }

        public PostAttribute(string route)
        {
            Route = route;
        }

    }




}
