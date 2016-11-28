using System;
using Scottie;

namespace Scottie2.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var server = new ScottieServer();

            using (server.Start())
            {
                Console.ReadKey();
            }
        }
    }
}