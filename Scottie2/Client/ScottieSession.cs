using System;
using System.Net.Http;

namespace Scottie.Client
{
    public class ScottieSession : IScottieSession
    {
        //public static IScottieSession Connect(HttpClient client)
        //{
        //    //client.BaseAddress = new Uri("http://localhost:2323/");
        //    //client.DefaultRequestHeaders.Accept.Clear();
        //    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    new JsonContent {}
        //    client.PostAsJsonAsync()

        //    Product product = null;
        //    HttpResponseMessage response = await client.GetAsync(path);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        product = await response.Content.ReadAsAsync<Product>();
        //    }
        //    return product;
        //}

        public ScottieSession(long id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            Id = id;
        }

        // I guess it makes sense that the ScottieSession classs sends a heartbeat.

        public long Id { get; }
    }
}