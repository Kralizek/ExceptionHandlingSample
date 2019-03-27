using System;
using System.Net;
using ExceptionHandlingSample.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace ExceptionHandlingSample.Filters
{
    public class ExceptionHandlerFilter : IExceptionFilter
    {
        private readonly ILoggerFactory _loggerFactory;

        public ExceptionHandlerFilter(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public void OnException(ExceptionContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                // we simulate a ILogger<TController>
                var logger = _loggerFactory.CreateLogger(descriptor.ControllerTypeInfo.FullName);

                if (context.Exception.TryExtractExceptionInfo(out var exceptionInfo)) // the exception was enriched with the extra data
                {
                    exceptionInfo.Logger(logger, exceptionInfo.EventId, exceptionInfo.State, context.Exception, exceptionInfo.Formatter);

                    var obj = new
                    {
                        errorId = exceptionInfo.EventId.Id,
                        error = exceptionInfo.EventId.Name,
                        data = exceptionInfo.State,
                        message = exceptionInfo.Formatter(exceptionInfo.State, context.Exception),
                        errorData = context.Exception.Data.PrepareForOutput()
                    };

                    context.Result = new ObjectResult(obj)
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };

                    context.ExceptionHandled = true;
                }
                else // the exception doesn't contain any extra data...
                {
                    logger.LogError(context.Exception, "An error has occurred");

                    var obj = new
                    {
                        message = context.Exception.Message,
                        errorData = context.Exception.Data.PrepareForOutput()
                    };

                    context.Result = new ObjectResult(obj)
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };

                    context.ExceptionHandled = true;
                }
            }
            else
            {
                // not sure if we ever end up here...
            }
        }
    }
}