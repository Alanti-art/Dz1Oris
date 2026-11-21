using System.Collections.Generic;

namespace ConsoleApp
{
    public class User
    {
        public string? Name { get; set; }
        public string? Group { get; set; }
        public int Age { get; set; }
        public bool Sex { get; set; }

        public User() { }

        public User(string? name, string? group, int age, bool sex)
        {
            Name = name;
            Group = group;
            Age = age;
            Sex = sex;
        }
    }

    public class Bag
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public Size? Size { get; set; }
        public Color? Color { get; set; }
        public Madein? Madein { get; set; }
        public List<string> Tags { get; set; } = new List<string>();

        public Bag() { }

        public Bag(string? name, decimal price, Size? size, Color? color, Madein? madein)
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

        public Size() { }
        public Size(int width, int height, int depth)
        {
            Width = width;
            Height = height;
            Depth = depth;
        }
    }

    public class Color
    {
        public string? ColorName { get; set; }

        public Color() { }
        public Color(string? colorName)
        {
            ColorName = colorName;
        }
    }

    public class Madein
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? City { get; set; }

        public Madein() { }
        public Madein(int id, string? name, string? city)
        {
            Id = id;
            Name = name;
            City = city;
        }
    }

    public class Category
    {
        public string? Name { get; set; }
        public List<Bag>? Bags { get; set; }
    }
}
