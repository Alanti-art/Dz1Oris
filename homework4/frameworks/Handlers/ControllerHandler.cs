using CustomHttpServer.Framework.Attributes;
using CustomHttpServer.Framework.Handlers;
using MyHttpServer.frameworks.Аttributes;
using MyHttpServer.frameworks.Новая_папка;
using System;
using System.Collections.Generic; // ИСПРАВЛЕНО: нужен для Dictionary
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.frameworks.Handlers
{
    internal class ControllerHandler : Handler
    {
        public override async Task HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            string path = request.Url.LocalPath;
          
            string filePath = Directory.GetCurrentDirectory() + $"/static{path}";
            var response = context.Response;

            bool isFile = !Path.HasExtension(path);

            Console.WriteLine($"Внимание запрос: {path}");

            if (isFile)
            {
                string[] strParams = request.Url
                    .Segments
                    .Skip(1)
                    .Select(s => s.Replace("/", ""))
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();

                if (strParams.Length < 2)
                {
                    response.StatusCode = 400;
                    response.Close();
                    return;
                }

                string controllerRoute = strParams[0];
                string methodRoute = strParams[1];



                var assembly = Assembly.GetExecutingAssembly();
                Type controller = null;

                foreach (var type in assembly.GetTypes())
                {
                    var attribute = type.GetCustomAttribute<HttpController>();
                    if (attribute != null && attribute.Route.ToLower() == controllerRoute.ToLower())
                    {
                        controller = type;
                        break;
                    }
                }

                if (controller == null)
                {
                    response.StatusCode = 404;
                    response.Close();
                    return;
                }

                MethodInfo method = null;

                foreach (MethodInfo candidate in controller.GetMethods())
                {
                    if (request != null && request.HttpMethod.ToUpper() == "GET")
                    {
                        var attribute = candidate.GetCustomAttribute<GetAttribute>();
                        if (attribute != null && attribute.Route.ToLower() == methodRoute.ToLower())
                        {
                            method = candidate;
                            break;
                        }
                    }
                    else if (request != null && request.HttpMethod.ToUpper() == "POST")
                    {
                        var attribute = candidate.GetCustomAttribute<PostAttribute>();
                        if (attribute != null && attribute.Route.ToLower() == methodRoute.ToLower())
                        {
                            method = candidate;
                            break;
                        }
                    }
                }

                if (method == null)
                {
                    response.StatusCode = 404;
                    response.Close();
                    return;
                }










               
                string body = "";

                if (request.HttpMethod == "POST")
                {
                    byte[] bytes = new byte[request.ContentLength64];
                    request.InputStream.Read(bytes, 0, bytes.Length);
                    body = Encoding.UTF8.GetString(bytes);
                }

                Dictionary<string, string> formData = new Dictionary<string, string>();

            
                string[] pairs = body.Split('&');

                foreach (string pair in pairs)
                {
                   
                    string[] parts = pair.Split('=');

                    if (parts.Length == 2)
                    {
                        
                        string key = Uri.UnescapeDataString(parts[0].Replace("+", " "));
                        string value = Uri.UnescapeDataString(parts[1].Replace("+", " "));

    
                        formData[key] = value;
                    }
                }

                ParameterInfo[] methodParameters = method.GetParameters();
                object[] queryParams = new object[methodParameters.Length];
                int urlParamIndex = 2;

                for (int i = 0; i < methodParameters.Length; i++)
                {

                   if (methodParameters[i].GetCustomAttribute<QueryDataAttribute>() != null)
                    {
                        if (formData.ContainsKey(methodParameters[i].Name))
                        {
                            queryParams[i] = formData[methodParameters[i].Name];
                        }
                        else
                        {
                            queryParams[i] = null;
                        }
                    }

                    else
                    {
                        if (urlParamIndex < strParams.Length)
                        {
                            queryParams[i] = Convert.ChangeType(strParams[urlParamIndex], methodParameters[i].ParameterType);
                            urlParamIndex++;
                        }
                        else
                        {
                            queryParams[i] = methodParameters[i].ParameterType.IsValueType ? Activator.CreateInstance(methodParameters[i].ParameterType) : null;
                        }
                    }
                }

                var result = method.Invoke(Activator.CreateInstance(controller), queryParams);


                if (method.ReturnType == typeof(void))
                {
                    response.Close();
                }



            }
            else if (Successor != null)
            {
                await Successor.HandleRequest(context);
            }
        }


    }
}