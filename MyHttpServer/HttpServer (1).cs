using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace MyHttpServer.frameworks.core
{
    internal class HttpServer
    {

        private string SettingsJson;
        private HttpListener server;
        private string urlPrefix;



        public HttpServer()
        {

            SettingsJson = File.ReadAllText("settings.json");
            Settings setting = JsonSerializer.Deserialize<Settings>(SettingsJson);

            server = new HttpListener();
            urlPrefix = $"http://{setting.Host}:{setting.Port}/{setting.Path}/";
            server.Prefixes.Add(urlPrefix);


        }

        public static void whileServer(string c)
        {
            if (!File.Exists("settings.json"))
            {
                Console.WriteLine("Отсутствует settings.json");
                return;
            }

            if (c == "start")
            {
                HttpServer server = new HttpServer();
                server.ServerStart();

                Console.WriteLine("Чтобы закончить работу сервера введите: exit");

                while (true)
                {
                    string? cmd = Console.ReadLine()?.Trim().ToLower();
                    if (cmd == "exit")
                    {
                        server.ServerStop();
                        return;
                    }
                    Console.WriteLine("Неизвестная команда");
                }
            }
        }
        
        public void ServerStart()
        {
            server.Start();
            StartAsync();
            Console.WriteLine("Сервер работает и слушает " + urlPrefix);
            
        }
        public void ServerStop()
        {
            server.Stop();
            
        }
        async Task StartAsync()
        {

            try
            {
                while (true)
                {
                    var context = await server.GetContextAsync();

                    var response = context.Response;

                    string responseText = File.ReadAllText("SearchEn.html");

                    byte[] buffer = Encoding.UTF8.GetBytes(responseText);

                    response.ContentLength64 = buffer.Length;
                    using Stream output = response.OutputStream;

                    await output.WriteAsync(buffer);
                    await output.FlushAsync();

                    Console.WriteLine("Запрос обработан");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Сервер завершил работу");
            }

        }

    }
}
