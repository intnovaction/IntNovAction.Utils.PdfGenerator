using iTextSharp.tool.xml.pipeline.html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using System.Web.Hosting;
using System.IO;
using System.Net;

namespace IntNovAction.Utils.PdfGenerator.Providers
{
    public class DownloadImageProvider : AbstractImageProvider
    {
        public override string GetImageRootPath()
        {
            return null;
        }

        public override Image Retrieve(string src)
        {
            Image image = null;

            if (Uri.TryCreate(src, UriKind.RelativeOrAbsolute, out Uri uri))
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
                        return null;
                    }
                }

            }

            return image;

        }

        private Image RetrieveLocalImage(string src)
        {
            var localPath = HostingEnvironment.MapPath(src);
            if (File.Exists(localPath))
            {
                return Image.GetInstance(localPath);
            }
            return null;
        }

        private Image RetrieveRemoteImage(string src)
        {
            using (WebClient client = new WebClient())
            {
                var imageBytes = client.DownloadData(src);

                if (imageBytes != null)
                {
                    return Image.GetInstance(imageBytes);
                }

                return null;
            }
        }
    }
}
