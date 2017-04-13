using IntNovAction.Utils.PdfGenerator.Helpers;
using iTextSharp.text;
using iTextSharp.tool.xml.pipeline.html;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;

namespace IntNovAction.Utils.PdfGenerator.Providers
{
    public class DownloadImageProvider : AbstractImageProvider
    {
        private HttpClient httpClient;

        public DownloadImageProvider()
        {
            httpClient = HttpClientSingleton.Instance;
        }

        public DownloadImageProvider(HttpClient customConfiguredClient)
        {
            httpClient = customConfiguredClient;
        }

        public override string GetImageRootPath()
        {
            return null;
        }

        public override Image Retrieve(string src)
        {
            Image image = null;
            Uri uri;

            if (Uri.TryCreate(src, UriKind.RelativeOrAbsolute, out uri))
            {
                // intentar obtener la imagen de forma local
                if (!uri.IsAbsoluteUri || uri.LocalPath.Equals(src, StringComparison.InvariantCultureIgnoreCase))
                {
                    image = RetrieveLocalImage(src);
                }

                // si no existe, intentar hacer la descarga de la imagen
                if (image == null)
                {
                    try
                    {
                        image = RetrieveRemoteImage(src);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.ToString());
                    }
                }
            }
            else
            {
                Trace.TraceError("Image uri error: {0}", src);
            }

            return image;
        }

        private static void AddCookiesToMessage(string src, HttpRequestMessage message)
        {
            var uri = new Uri(src);
            if (HttpContext.Current.Request.Url.Authority.Equals(uri.Authority, StringComparison.InvariantCultureIgnoreCase))
            {
                var cookies = new List<string>();

                var sessionCookieName = "ASP.NET_SessionId";
                SessionStateSection sessionStateSection = (SessionStateSection)ConfigurationManager.GetSection("system.web/sessionState");
                if (sessionStateSection != null)
                {
                    sessionCookieName = sessionStateSection.CookieName;
                }


                foreach (var cookie in HttpContext.Current.Request.Cookies.AllKeys)
                {
                    if (cookie != sessionCookieName)
                    {
                        cookies.Add(string.Format("{0}={1}", cookie, HttpContext.Current.Request.Cookies[cookie].Value));
                    }
                }

                message.Headers.Add("Cookie", string.Join("; ", cookies));
            }
        }



        /// <summary>
        /// Obtiene una imagen de un path local
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private Image RetrieveLocalImage(string src)
        {
            var localPath = HostingEnvironment.MapPath(src);
            if (File.Exists(localPath))
            {
                return Image.GetInstance(localPath);
            }
            Trace.TraceError("Local file {0} not found", src);
            return null;
        }

        /// <summary>
        /// Obtiene una imagen de un path remoto
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private Image RetrieveRemoteImage(string src)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, src);
            AddCookiesToMessage(src, message);

            var response = HttpClientSingleton.Instance.SendAsync(message).Result;

            if (response.IsSuccessStatusCode)
            {
                var bytes = response.Content.ReadAsByteArrayAsync().Result;

                Trace.TraceInformation("File downloaded: {0} {1} bytes", src, bytes.LongLength);

                try
                {
                    return Image.GetInstance(bytes);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Error creating image {0}: {1}", src, ex.Message);
                }
            }
            else
            {
                Trace.TraceInformation("Error downloading: {0} {1}", src, response.StatusCode);
            }


            return null;

        }
    }
}