using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.frameworks.Аttributes
{
    public class FormDataAttribute : Attribute
    {
        public string? Name { get; }

        public FormDataAttribute() { }
        public FormDataAttribute(string name)
        {
            Name = name;
        }
    }
}
