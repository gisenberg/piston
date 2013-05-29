//     RestHandlerErrorAttribute.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Piston.Push.Web
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RestHandleErrorAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(HttpActionExecutedContext filterContext)
        {
            Trace.TraceError(filterContext.Exception.ToString());
            filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.InternalServerError, new
            {
                success = false,
                error = filterContext.Exception.GetType().Name,
                message = filterContext.Exception.Message,
                stack = filterContext.Exception.StackTrace
            });
        }

        public Task ExecuteExceptionFilterAsync(HttpActionExecutedContext filterContext, CancellationToken cancellationToken)
        {
            Trace.TraceError(filterContext.Exception.ToString());
            filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.InternalServerError, new
            {
                success = false,
                error = filterContext.Exception.GetType().Name,
                message = filterContext.Exception.Message,
                stack = filterContext.Exception.StackTrace
            });
            return Task.Run(() => { });
        }
    }
}
