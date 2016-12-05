using Microsoft.AspNetCore.Hosting;
using Scottie.Database;
using Scottie.Server;

namespace Scottie
{
    public interface IScottieServer
    {
        IWebHost Start();
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
        
        public IWebHost Start()
        {
            ScottiesHole.CreateDatabase();

            var host = _hostBuilder.Build();
            host.Start();
            return host;
        }
    }
}
