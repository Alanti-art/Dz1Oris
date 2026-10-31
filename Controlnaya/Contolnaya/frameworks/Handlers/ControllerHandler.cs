using CustomHttpServer.Framework.Attributes;
using CustomHttpServer.Framework.Handlers;
using MyHttpServer.frameworks.Аttributes;
using MyHttpServer.frameworks.Новая_папка;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MyHttpServer.frameworks.HttpResponses;

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

                if (strParams.Length < 1)
                {
                    response.StatusCode = 400;
                    response.Close();
                    return;
                }

                string controllerRoute = strParams[0];
                string methodRoute = "";
                if (strParams.Length >= 2)
                {
                    methodRoute = strParams[1];
                }



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
                    if (request != null && request.HttpMethod == "GET")
                    {
                        var attribute = candidate.GetCustomAttribute<GETAttribute>();
                        if (attribute != null && attribute.Route == methodRoute)
                        {
                            method = candidate;
                            break;
                        }
                    }
                    else if (request != null && request.HttpMethod == "PUT")
                    {
                        var attribute = candidate.GetCustomAttribute<PUTAttribute>();
                        if (attribute != null && attribute.Route == methodRoute)
                        {
                            method = candidate;
                            break;
                        }
                    }
                    else if (request != null && request.HttpMethod == "POST")
                    {
                        var attribute = candidate.GetCustomAttribute<POSTAttribute>();
                        if (attribute != null && attribute.Route == methodRoute)
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










              

                Dictionary<string, string> formData = new Dictionary<string, string>();
                if (request.HttpMethod == "POST" || request.HttpMethod == "PUT")
                {
                    StreamReader reader = new StreamReader(request.InputStream, Encoding.UTF8);
                    string body = reader.ReadToEnd();

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

                }





                ParameterInfo[] methodParameters = method.GetParameters();
                object[] queryParams = new object[methodParameters.Length];
                int urlParamIndex = 2;

                ParameterInfo parament;
                string parName;

                for (int i = 0; i < methodParameters.Length; i++)
                {
                    parament = methodParameters[i];
                    parName = parament.Name;
                    if (parament.GetCustomAttribute<FormDataAttribute>() != null)
                    {
                        if (formData.ContainsKey(parament.Name))
                        {
                            queryParams[i] = formData[parament.Name];
                        }
                        else
                        {
                            queryParams[i] = null;
                        }
                    }
                    else if (request.QueryString[parName] != null)
                    {
                        queryParams[i] = Convert.ChangeType(request.QueryString[parName], parament.ParameterType);
                    }

                    else
                    {
                        if (urlParamIndex < strParams.Length)
                        {
                            queryParams[i] = Convert.ChangeType(strParams[urlParamIndex], parament.ParameterType);
                            urlParamIndex++;
                        }
                        else
                        {
                            queryParams[i] = parament.ParameterType.IsValueType ? Activator.CreateInstance(methodParameters[i].ParameterType) : null;
                        }
                    }
                }

                var result = method.Invoke(Activator.CreateInstance(controller), queryParams);

                var typedResult = result as IHttpResponseTypeResult;
                if (typedResult != null)
                {
                    byte[] buffer = typedResult.Execute(context);

                    if (buffer.Length > 0)
                    {
                        response.ContentLength64 = buffer.Length;
                        await response.OutputStream.WriteAsync(buffer);
                    }
                }

                response.Close();



            }
            else if (Successor != null)
            {
                response.StatusCode = 404;
                response.Close();
                return;
            }
        }


    }
}