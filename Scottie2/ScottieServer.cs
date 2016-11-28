using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Scottie
{
    public interface IScottieServer
    {
        IWebHost Start();
    }

    public class ScottieServerStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            loggerFactory
                .WithFilter(new FilterLoggerSettings
                {
                    { "Microsoft", LogLevel.Trace },
                    { "System", LogLevel.Trace }
                })
                .AddConsole();


            // add Trace Source logging
            var testSwitch = new SourceSwitch("sourceSwitch", "Logging Sample");
            testSwitch.Level = SourceLevels.Warning;
            loggerFactory.AddTraceSource(testSwitch,
                new TextWriterTraceListener(writer: Console.Out));

            app.UseMvcWithDefaultRoute();

            app.Run(async (context) =>
            {
                if (context.Request.Path.Value.Contains("boom"))
                {
                    throw new Exception("boom!");
                }
                var logger = loggerFactory.CreateLogger("Catchall Endpoint");
                logger.LogInformation("No endpoint found for request {path}", context.Request.Path);
                await context.Response.WriteAsync("No endpoint found - try /api/todo.");
            });


        }
    }

    public class ScottieServer : IScottieServer
    {
        private readonly IWebHostBuilder _hostBuilder;

        public ScottieServer(IWebHostBuilder builder = null)
        {
            _hostBuilder = builder ?? DefaultWebHost();
        }
        
        public static IWebHostBuilder DefaultWebHost()
        {
            return new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://127.0.0.1:2323")
                .UseStartup<ScottieServerStartup>();
        }


        // Document aPI with Swagger.
        // https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger
        // https://damienbod.com/2015/12/13/asp-net-5-mvc-6-api-documentation-using-swagger/

        public IWebHost Start()
        {
            var host = _hostBuilder.Build();
            host.Start();
            return host;
        }
    }
}
