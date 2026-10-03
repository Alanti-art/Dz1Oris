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



                    var request = context.Request;
                    string path = request.Url.LocalPath;
                    var response = context.Response;

                    if (string.IsNullOrEmpty(path) || path == "/")
                    {
                        path = "/SearchEn.html";
                    }


                    //Console.WriteLine("Внимание запрос");





                    string filePath = Directory.GetCurrentDirectory() + $"/static{path}";

                    FileInfo fileInfo = new FileInfo(filePath);


                    //Console.WriteLine(filePath);
                    //Console.WriteLine(path);
                  

                    if (fileInfo.Extension == "")
                    {
                        string[] extensions = { ".html", ".png"};

                        foreach (string ext in extensions)
                        {
                            if (File.Exists(filePath + ext))
                            {
                                filePath += ext;
                                fileInfo = new FileInfo(filePath);
                                break;
                            }
                        }
                    }
                    //Console.WriteLine(filePath);










                    if (!fileInfo.Exists)
                    {
                        response.StatusCode = 404;
                        filePath = Directory.GetCurrentDirectory() + $"/static/404.html";
                    }

                    switch (fileInfo.Extension)
                    {
                        
                        case ".html":
                        case ".htm":
                            response.ContentType = "text/html; charset=utf-8";
                            break;
                        case ".css":
                            response.ContentType = "text/css; charset=utf-8";
                            break;
                        case ".js":
                        case ".mjs":
                            response.ContentType = "text/javascript; charset=utf-8";
                            break;
                        case ".json":
                        case ".map":
                            response.ContentType = "application/json; charset=utf-8";
                            break;
                        case ".xml":
                            response.ContentType = "application/xml; charset=utf-8";
                            break;
                        case ".txt":
                            response.ContentType = "text/plain; charset=utf-8";
                            break;
                        case ".csv":
                            response.ContentType = "text/csv; charset=utf-8";
                            break;
                        case ".md":
                            response.ContentType = "text/markdown; charset=utf-8";
                            break;

                       
                        case ".png":
                            response.ContentType = "image/png";
                            break;
                        case ".jpg":
                        case ".jpeg":
                        case ".jpe":
                        case ".jfif":
                            response.ContentType = "image/jpeg";
                            break;
                        case ".gif":
                            response.ContentType = "image/gif";
                            break;
                        case ".svg":
                            response.ContentType = "image/svg+xml";
                            break;
                        case ".ico":
                            response.ContentType = "image/x-icon";
                            break;
                        case ".bmp":
                            response.ContentType = "image/bmp";
                            break;
                        case ".webp":
                            response.ContentType = "image/webp";
                            break;
                        case ".avif":
                            response.ContentType = "image/avif";
                            break;
                        case ".tif":
                        case ".tiff":
                            response.ContentType = "image/tiff";
                            break;

                        
                        case ".woff":
                            response.ContentType = "font/woff";
                            break;
                        case ".woff2":
                            response.ContentType = "font/woff2";
                            break;
                        case ".ttf":
                            response.ContentType = "font/ttf";
                            break;
                        case ".otf":
                            response.ContentType = "font/otf";
                            break;
                        case ".eot":
                            response.ContentType = "application/vnd.ms-fontobject";
                            break;

                        
                        case ".mp3":
                            response.ContentType = "audio/mpeg";
                            break;
                        case ".wav":
                            response.ContentType = "audio/wav";
                            break;
                        case ".ogg":
                        case ".oga":
                            response.ContentType = "audio/ogg";
                            break;
                        case ".m4a":
                            response.ContentType = "audio/mp4";
                            break;
                        case ".flac":
                            response.ContentType = "audio/flac";
                            break;

                       
                        case ".mp4":
                            response.ContentType = "video/mp4";
                            break;
                        case ".webm":
                            response.ContentType = "video/webm";
                            break;
                        case ".ogv":
                            response.ContentType = "video/ogg";
                            break;
                        case ".avi":
                            response.ContentType = "video/x-msvideo";
                            break;
                        case ".mov":
                            response.ContentType = "video/quicktime";
                            break;
                        case ".mkv":
                            response.ContentType = "video/x-matroska";
                            break;

                       
                        case ".pdf":
                            response.ContentType = "application/pdf";
                            break;
                        case ".zip":
                            response.ContentType = "application/zip";
                            break;
                        case ".gz":
                            response.ContentType = "application/gzip";
                            break;
                        case ".7z":
                            response.ContentType = "application/x-7z-compressed";
                            break;
                        case ".rar":
                            response.ContentType = "application/vnd.rar";
                            break;
                        case ".wasm":
                            response.ContentType = "application/wasm";
                            break;
                        case ".webmanifest":
                            response.ContentType = "application/manifest+json";
                            break;
                        case ".doc":
                            response.ContentType = "application/msword";
                            break;
                        case ".docx":
                            response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                            break;
                        case ".xls":
                            response.ContentType = "application/vnd.ms-excel";
                            break;
                        case ".xlsx":
                            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            break;
                        case ".ppt":
                            response.ContentType = "application/vnd.ms-powerpoint";
                            break;
                        case ".pptx":
                            response.ContentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                            break;

                        
                    }



                    byte[] buffer = File.ReadAllBytes(filePath);

                    response.ContentLength64 = buffer.Length;
                    using Stream output = response.OutputStream;

                    await output.WriteAsync(buffer);
                    await output.FlushAsync();

                    Console.WriteLine("Запрос обработан");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Сервер завершил работу");
            }

        }

    }
}