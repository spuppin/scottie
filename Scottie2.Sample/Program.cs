using System;
using System.Collections.Generic;
using Scottie;
using Scottie.Server;

namespace Scottie2.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var server = new ScottieServer();
            
            using (server.Start())
            {
                using (var client = new ScottieClient())
                {
                    client.Connect();
                    string path = client.Create(@"/foo/bar", "my data", CreateMode.Ephemeral);

                    client.Update(path, "updated data", 1);
                    client.Delete(path, 1);
                    var multi = new List<MultiOpParams>
                    {
                        new MultiOpParams { CheckVersion = new CheckVersionParams()},
                        new MultiOpParams { Create = new CreateParams()},
                        new MultiOpParams { Delete = new DeleteParams()},
                        new MultiOpParams { Update = new UpdateParams()}
                    };
                    client.Multi(multi);

                    path = client.Get(path);
                    client.GetChildren(path);

                    Console.ReadKey();
                }
            }
        }
    }
}