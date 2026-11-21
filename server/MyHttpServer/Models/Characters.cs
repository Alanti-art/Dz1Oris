using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MyHttpServer.Models
{
   
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Image { get; set; } = "";
        public string Description { get; set; } = "";
        public string Action { get; set; } = "";
        public string Cost { get; set; } = "";
        public string Blocks { get; set; } = "";
        public string BlockedBy { get; set; } = "";

        public Character() { }

        public Character(int id, string name, string image, string description,
                         string action, string cost, string blocks, string blockedBy)
        {
            Id = id;
            Name = name;
            Image = image;
            Description = description;
            Action = action;
            Cost = cost;
            Blocks = blocks;
            BlockedBy = blockedBy;
        }
    }
}
