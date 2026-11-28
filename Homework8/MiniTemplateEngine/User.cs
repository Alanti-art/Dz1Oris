using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class User
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public int Age { get; set; }
        public bool Sex { get; set; }

        public User(string name, string group, int age, bool sex )
        {
            Name = name;
            Group = group;
            Age = age;
            Sex = sex;

        }


    }
}
