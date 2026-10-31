using System.Net;
using System.Net.Security;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MyHttpServer.frameworks.core;


namespace MyHttpServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Чтобы начать работу сервера введите: start");
            string? command = Console.ReadLine()?.Trim().ToLower();
            HttpServer.whileServer(command);
        }
       
            
     
    }
}