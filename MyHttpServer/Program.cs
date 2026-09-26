using System.Net;
using System.Net.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MyHttpServer.frameworks.core;


//HttpListener  server = new HttpListener();



//string settingsJson = File.ReadAllText("settings.json");
//Settings setting = JsonSerializer.Deserialize<Settings>(settingsJson);


//string urlPrefix = $"http://{setting.Host}:{setting.Port}/{setting.Path}/";


//server.Prefixes.Add(urlPrefix);
//server.Start();
//Console.WriteLine("Сервер работает и слушает " + urlPrefix);


//var context = await server.GetContextAsync();

//var response = context.Response;

//string responseText = File.ReadAllText("hello.html");

//byte[] buffer = Encoding.UTF8.GetBytes(responseText);

//response.ContentLength64 = buffer.Length;
//using Stream output = response.OutputStream;

//await output.WriteAsync(buffer);
//await  output.FlushAsync();

//Console.WriteLine("Запрос обработан");
//server.Stop();
//Console.WriteLine("Сервер завершил работу");
//Console.ReadLine();



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