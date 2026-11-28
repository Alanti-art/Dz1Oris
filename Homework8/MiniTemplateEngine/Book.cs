using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Book
    {
        public string Name { get; set; }
        public string Year { get; set; }
        public string Author { get; set; }


        public Book(string name, string year, string author)
        {
            Name = name;
            Year = year;
            Author = author;
        }
         


    }
}
