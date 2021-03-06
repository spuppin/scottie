using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scottie.Database;
using Swashbuckle.Swagger.Model;

namespace Scottie.Server
{
    public class ScottieServerStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSwaggerGen();

            const string dbName = "scottie.db";
            ISessionStore sqlLiteSessionStore = new SqlLiteSessionStore(dbName);
            INodeStore nodeStore = new SqlLiteNodeStore(dbName);

            sqlLiteSessionStore.Init();
            nodeStore.Init();
            services.AddSingleton(sqlLiteSessionStore);
            services.AddSingleton(nodeStore);

            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "Scottie API",
                    Description = "A RESTful API for Scottie",
                    TermsOfService = "None"
                });
                options.DescribeAllEnumsAsStrings();
            });
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

            app.UseSwagger();
            app.UseSwaggerUi();

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
}