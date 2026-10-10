using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomHttpServer.Framework.Attributes
{

    public class GetAttribute : Attribute
    {
        public string Route { get; init; }

        public GetAttribute(string route)
        {
            Route = route;
        }
    }
}

