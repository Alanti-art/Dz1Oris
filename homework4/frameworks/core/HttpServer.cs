using CustomHttpServer.Framework.Handlers;
using MyHttpServer.frameworks.Handlers;
using MyHttpServer.frameworks.Новая_папка;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace MyHttpServer.frameworks.core
{
    internal class HttpServer
    {

        private string SettingsJson;
        private HttpListener server;
        private string urlPrefix;
        private string Path;



        public HttpServer()
        {

            SettingsJson = File.ReadAllText("settings.json");
            Settings setting = JsonSerializer.Deserialize<Settings>(SettingsJson);

            server = new HttpListener();
            urlPrefix = $"http://{setting.Host}:{setting.Port}/{Path}";
            //Console.WriteLine(urlPrefix);
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

                    await ProcessRequestAsync(context);






                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Сервер завершил работу");
            }

        }
        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            Handler staticFileHandler = new StaticFileHandler();
            Handler controllerHandler = new ControllerHandler();

            staticFileHandler.Successor = controllerHandler;

            Console.WriteLine($"Обработан запрос: {context.Request.Url}");

            await staticFileHandler.HandleRequest(context);
        }

    }
}