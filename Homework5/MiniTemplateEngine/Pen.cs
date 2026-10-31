using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{

    public class Pen
    {


        public string Name { get; set; }

        public string Color { get; set; }

        public int Width { get; set; }

        public decimal Price { get; set; }

        public Pen(string name, string color, int widht, decimal price)
        { 
            Name = name;
            Color = color;
            Width = widht;
            Price = price;
        }
        
    }

}
