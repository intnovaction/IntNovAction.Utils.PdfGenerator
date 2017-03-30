using IntNovAction.Utils.PdfGenerator.Configuration;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace IntNovAction.Utils.PdfGenerator.Helpers
{
    public static class DocumentHelper
    {
        internal static byte[] GetDocumentFromHtml(DocumentConfiguration config, string htmlString)
        {
            byte[] documentBytes = null;

            using (var memStream = new MemoryStream())
            {
                var document = new Document(config.ITextSharpLayout);

                var replacements = new List<string>();
                if (config.ReplaceImagePaths)
                {
                    htmlString = ReplacePaths(htmlString, "img", "src", replacements);
                }

                if (config.ReplaceCssPaths)
                {
                    htmlString = ReplacePaths(htmlString, "link", "href", replacements);
                }

                // si la hoja de estilo ya está en el html, da un error => excluir las repetidas
                var stylesheetsFilePath = new List<string>();
                foreach (var stylesheetPath in config.StyleSheets)
                {
                    var path = HostingEnvironment.MapPath(stylesheetPath);
                    if (!replacements.Contains(path.ToLower()))
                    {
                        stylesheetsFilePath.Add(path);
                    }
                }



                var writer = PdfWriter.GetInstance(document, memStream);
                writer.CloseStream = false;
                writer.AddViewerPreference(PdfName.PICKTRAYBYPDFSIZE, PdfBoolean.PDFTRUE);

                document.Open();
                AddHTMLText(writer, document, htmlString, stylesheetsFilePath);
                document.Close();

                documentBytes = memStream.ToArray();
            }

            return documentBytes;
        }


        private static void AddHTMLText(PdfWriter writer, Document document, string htmlString, List<string> stylesheetFilePath)
        {

            var cssResolver = new StyleAttrCSSResolver();

            Stream cssStream = null;
            if (stylesheetFilePath != null)
            {
                foreach (var stylesheetPath in stylesheetFilePath)
                {
                    cssStream = new FileStream(stylesheetPath, FileMode.Open, FileAccess.Read);
                    var cssFile = XMLWorkerHelper.GetCSS(cssStream);
                    cssResolver.AddCss(cssFile);
                }
            }

            var cssAppliersImpl = new CssAppliersImpl();
            var htmlPipelineContext = new HtmlPipelineContext(cssAppliersImpl);
            htmlPipelineContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());

            var pdfWriterPipeline = new PdfWriterPipeline(document, writer);
            var htmlPipe = new HtmlPipeline(htmlPipelineContext, pdfWriterPipeline);
            var cssResolverPipeline = new CssResolverPipeline(cssResolver, htmlPipe);

            var worker = new XMLWorker(cssResolverPipeline, true);
            var parser = new XMLParser(worker);
            var stringReader = new StringReader(htmlString);
            parser.Parse(stringReader);

            if (cssStream != null)
            {
                cssStream.Dispose();
            }
        }

        private static string ReplacePaths(string htmlString, string tag, string attrib, List<string> replacements)
        {
            var tagPattern = string.Format("<{0}", tag);
            var attribPattern = string.Format("{0}=\"/", attrib);
            var attribOffset = attribPattern.Length;

            var tagIndex = htmlString.IndexOf(tagPattern);
            while (tagIndex > -1)
            {
                var endTagIndex = htmlString.IndexOf(">", tagIndex);
                var attribIndex = htmlString.IndexOf(attribPattern, tagIndex) + attribOffset - 1;
                if (attribIndex > -1 && attribIndex < endTagIndex)
                {
                    var lastPosition = htmlString.IndexOf('"', attribIndex + 1);
                    var relPath = htmlString.Substring(attribIndex, lastPosition - attribIndex);
                    var path = HostingEnvironment.MapPath(relPath);
                    htmlString = htmlString.Replace(relPath, path);

                    replacements.Add(path.ToLower());
                }

                tagIndex = htmlString.IndexOf(tagPattern, tagIndex + 1);
            }

            return htmlString;
        }

    }
}
