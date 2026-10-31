using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CustomHttpServer.Framework.Handlers
{
    abstract class Handler
    {
        public Handler Successor { get; set; }

        public abstract Task HandleRequest(HttpListenerContext context);
    }
}








 