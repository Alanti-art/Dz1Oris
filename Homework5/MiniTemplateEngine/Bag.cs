using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class Bag
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public Size Size { get; set; }
        public Color Color { get; set; }
        public Madein Madein { get; set; }
        public List<string> Tags { get; set; }

        public Bag(string name, int price, Size size, Color color, Madein madein)
        {
            Name = name;
            Price = price;
            Size = size;
            Color = color;
            Madein = madein;
            Tags = new List<string>();
        }
    }
    public class Size
    { 
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
        

        public Size(int width, int height, int depth)
        { 
            Width = width;
            Height = height;
            Depth = depth;
        }
    }
    public class Color
    { 
        public string Colorname { get; set; }

        public Color (string colorName)
        {
            Colorname = colorName;
        }
    }
    public class Madein
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string City { get; set; }




        public Madein(int code, string name, string city)
        {
            Code = code;
            Name = name;
            City = city;

            if (city != null || name != null)
            { 
            }
        }
    }
}
