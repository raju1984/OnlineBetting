using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace QuickBetCore.Models
{
    public static class APKMiddlewareExtensionsExtensions
    {
        public static IApplicationBuilder UseRobots(
            this IApplicationBuilder builder,
            IWebHostEnvironment env,
            string rootPath = null
        )
        {
            return builder.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/QUICKBETV003.apk"), b =>
                b.UseMiddleware<APKMiddleware>(env.EnvironmentName, rootPath ?? env.ContentRootPath));
        }
    }

    public class APKMiddleware
    {
        const string Default =
            @"User-Agent: *\nAllow: /";

        private readonly RequestDelegate next;
        private readonly string environmentName;
        private readonly string rootPath;

        public APKMiddleware(
            RequestDelegate next,
            string environmentName,
            string rootPath
        )
        {
            this.next = next;
            this.environmentName = environmentName;
            this.rootPath = rootPath;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/QUICKBETV003.apk"))
            {
                var generalRobotsTxt = Path.Combine(rootPath, "QUICKBETV003.apk");
                var environmentRobotsTxt = Path.Combine(rootPath, $"QUICKBETV003.{environmentName}.apk");
                string output;

                // try environment first
                if (File.Exists(environmentRobotsTxt))
                {
                    output = await File.ReadAllTextAsync(environmentRobotsTxt);
                }
                // then robots.txt
                else if (File.Exists(generalRobotsTxt))
                {
                    output = await File.ReadAllTextAsync(generalRobotsTxt);
                }
                // then just a general default
                else
                {
                    output = Default;
                }

                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(output);
            }
            else
            {
                await next(context);
            }
        }
    }
}
