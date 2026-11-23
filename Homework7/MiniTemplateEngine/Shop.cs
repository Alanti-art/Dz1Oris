using System.Collections.Generic;

namespace ConsoleApp
{
    internal class Shop
    {
        public string Name { get; set; }
        public string City { get; set; }
        public bool IsOpen { get; set; }
        public int Discount { get; set; }
        public List<Bag> Bags { get; set; }
        public List<Category> Categories { get; set; }



        public Shop(string name, string city, bool isOpen, int discount, List<Bag> bags)
        {
            Name = name;
            City = city;
            IsOpen = isOpen;
            Discount = discount;
            Bags = bags;
        }
    }
    public class Category
    {
        public string Name { get; set; }
        public List<Bag> Bags { get; set; }
    }
}
