using System;
using System.Threading.Tasks;
using LeakSearchApp.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LeakSearchApp.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!TryGetBearerToken(context, out var requestToken))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Api Key was not provided"
                };
                return;
            }

            var appSecrets = context.HttpContext.RequestServices.GetRequiredService<IOptions<AppSecrets>>();

            var apiKey = appSecrets.Value.AdminToken;

            if (!apiKey.Equals(requestToken))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Api Key is not valid"
                };
                return;
            }

            await next();
        }

        private bool TryGetBearerToken(ActionExecutingContext context, out string key)
        {
            key = null;
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var rawHeader))
                return false;

            var firstValue = rawHeader.ToArray()[0].Split(" ");
            if (firstValue.Length < 2 || firstValue[0] != "Bearer")
                return false;

            key = firstValue[1];
            return true;
        }
    }
}