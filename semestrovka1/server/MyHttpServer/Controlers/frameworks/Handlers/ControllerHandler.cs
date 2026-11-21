using MyHttpServer.Controlers.frameworks;
using MyHttpServer.Controlers.frameworks.Authentication;
using MyHttpServer.Controlers.frameworks.HttpResponses;
using MyHttpServer.Controlers.frameworks.Аttributes;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyHttpServer.Controlers.frameworks.Handlers
{
    
    internal class ControllerHandler : Handler
    {
        private IUserSessionService SessionService;
        public ControllerHandler(IUserSessionService sessionService)
        {
            SessionService = sessionService;
        } 


        public override async Task HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            string requestPath = request.Url.LocalPath;

            string staticFilePath = Directory.GetCurrentDirectory() + $"/static{requestPath}";
            var response = context.Response;

            bool isHandleByController = !Path.HasExtension(requestPath);

            Console.WriteLine($"Внимание запрос: {requestPath}");

            if (isHandleByController)
            {
                
                string[] urlParts = request.Url
                    .Segments
                    .Skip(1)
                    .Select(part => part.Replace("/", ""))
                    .Where(part => !string.IsNullOrEmpty(part))
                    .ToArray();

                if (urlParts.Length < 1)
                {
                    urlParts = new[] { "home" };
                }

                string controllerRoute = urlParts[0];
                string methodRoute = "";
                if (urlParts.Length >= 2)
                {
                    methodRoute = urlParts[1];
                }



                var assembly = Assembly.GetExecutingAssembly();
                Type controllerClass = null;

                foreach (var type in assembly.GetTypes())
                {
                    var controllerAttr = type.GetCustomAttribute<HttpController>();
                    if (controllerAttr != null && controllerAttr.Route.ToLower() == controllerRoute.ToLower())
                    {
                        controllerClass = type;
                        break;
                    }
                }

                if (controllerClass == null)
                {
                    await Return404(response);
                    return;
                }

                MethodInfo methodToCall = null;


                foreach (MethodInfo method in controllerClass.GetMethods())
                {
                    if (request != null && request.HttpMethod == "GET")
                    {
                        var methodAttr = method.GetCustomAttribute<GETAttribute>();
                        if (methodAttr != null && methodAttr.Route == methodRoute)
                        {
                            methodToCall = method;
                            break;
                        }
                    }

                    else if (request != null && request.HttpMethod == "POST")
                    {
                        var methodAttr = method.GetCustomAttribute<POSTAttribute>();
                        if (methodAttr != null && methodAttr.Route == methodRoute)
                        {
                            methodToCall = method;
                            break;
                        }
                    }
                }

                if (methodToCall == null)
                {
                    response.StatusCode = 404;
                    response.Close();
                    return;
                }












                Dictionary<string, string> form = new Dictionary<string, string>();
                if (request.HttpMethod == "POST")
                {
                    StreamReader reader = new StreamReader(request.InputStream, Encoding.UTF8);
                    string body = reader.ReadToEnd();

                    string[] pairs = body.Split('&');

                    foreach (string pair in pairs)
                    {

                        string[] parts = pair.Split('=');

                        if (parts.Length == 2)
                        {

                            string name = Uri.UnescapeDataString(parts[0].Replace("+", " "));
                            string value = Uri.UnescapeDataString(parts[1].Replace("+", " "));


                            form[name] = value;
                        }
                    }

                }





                ParameterInfo[] methodParams = methodToCall.GetParameters();
                object[] args = new object[methodParams.Length];
                int urlPartIndex = 2;

                ParameterInfo param;
                string paramName;

                for (int i = 0; i < methodParams.Length; i++)
                {
                    param = methodParams[i];
                    paramName = param.Name;
                    if (param.GetCustomAttribute<FormDataAttribute>() != null)
                    {
                        if (form.ContainsKey(param.Name))
                        {
                            args[i] = form[param.Name];
                        }
                        else
                        {
                            args[i] = null;
                        }
                    }
                    else if (request.QueryString[paramName] != null)
                    {
                        args[i] = Convert.ChangeType(request.QueryString[paramName], param.ParameterType);
                    }

                    else
                    {
                        if (urlPartIndex < urlParts.Length)
                        {
                            args[i] = Convert.ChangeType(urlParts[urlPartIndex], param.ParameterType);
                            urlPartIndex++;
                        }
                        else
                        {
                            args[i] = param.ParameterType.IsValueType ? Activator.CreateInstance(methodParams[i].ParameterType) : null;
                        }
                    }
                }

                var controller = (ControllerBase)Activator.CreateInstance(controllerClass)!;
                controller.Initialize(context, SessionService);


                if (!IsAuthorized(context, controllerClass, methodToCall))
                {
                    context.Response.StatusCode = 401;
                    context.Response.Close();

                    return;
                }


                var result = methodToCall.Invoke(controller, args);

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

        private bool IsAuthorized(HttpListenerContext context, Type controllerType, MethodInfo method)
        {
            bool requiresAuthorization =
                controllerType.IsDefined(typeof(AuthorizeAttribute), true)
                || method.IsDefined(typeof(AuthorizeAttribute), true);

            if (!requiresAuthorization)
                return true;

            string? sessionId = context.Request.Cookies["sessionId"]?.Value;
            return SessionService.GetUserId(sessionId).HasValue;
        }
        private async Task Return404(HttpListenerResponse response)
        {
            response.StatusCode = 404;
            response.ContentType = "text/html; charset=utf-8";

            string filePath = Path.Combine(AppContext.BaseDirectory,"static","404.html");

            byte[] buffer = await File.ReadAllBytesAsync(filePath);

            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer);

            response.Close();
        }


    }
}
