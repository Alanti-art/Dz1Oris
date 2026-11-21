using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Controlers.frameworks.Аttributes
{

    public class POSTAttribute : Attribute
    {
        public string Route { get; init; }

        public POSTAttribute(string route)
        {
            Route = route;
        }

    }




}
