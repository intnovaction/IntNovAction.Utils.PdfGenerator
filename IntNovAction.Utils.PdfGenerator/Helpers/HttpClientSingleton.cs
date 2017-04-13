using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IntNovAction.Utils.PdfGenerator.Helpers
{
    /// <summary>
    /// Ingleton para el HttpClient.
    /// Fusilado de @luisruizpavon : https://github.com/lurumad/aspnetcore-health/blob/master/src/AspNetCore.Health/Internal/HttpClientSingleton.cs
    /// </summary>
    public class HttpClientSingleton : HttpClient
    {
        private HttpClientSingleton(HttpClientHandler handler) : base(handler)
        {

        }

        private static readonly Lazy<HttpClientSingleton> Lazy =
        new Lazy<HttpClientSingleton>(() =>
        {
            var handler = new HttpClientHandler { UseCookies = false };
            return new HttpClientSingleton(handler);
        });

        public static HttpClientSingleton Instance => Lazy.Value;

        private HttpClientSingleton()
        {
            DefaultRequestHeaders.Add("cache-control", "no-cache");
        }
    }
}
