using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.frameworks.Аttributes
{
    public class QueryDataAttribute : Attribute
    {
        public string? Name { get; }

        public QueryDataAttribute() { }
        public QueryDataAttribute(string name)
        {
            Name = name;
        }
    }
}
