using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Text { get; set; } = "";
        public string Rating { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public bool HasRating()
        {
           return !string.IsNullOrEmpty(Rating);
        }

        public Comment() { }
    }
}
