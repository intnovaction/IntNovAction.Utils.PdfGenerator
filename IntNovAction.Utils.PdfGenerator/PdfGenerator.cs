using IntNovAction.Utils.PdfGenerator.Configuration;
using IntNovAction.Utils.PdfGenerator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntNovAction.Utils.PdfGenerator
{
    public class PdfGenerator
    {

        /// <summary>
        /// Configuración del generador
        /// </summary>
        public DocumentConfiguration Configuration { get; internal set; } = new DocumentConfiguration();


        public async Task<byte[]> GetDocumentAsync(string view, object model)
        {
            var result = await Task.Run(() => GetDocumentAsync(view, model));
            return result;
        }

        /// <summary>
        /// Genera un pdf a partir de una vista y un modelo
        /// </summary>
        /// <param name="view">ruta de la vista (por ejemplo ~/Views/Home/Index.cshtml)</param>
        /// <param name="model">Modelo</param>
        /// <returns></returns>
        public byte[] GetDocument(string view, object model)
        {
            var htmlString = RenderViewHelper.Render(this.Configuration, view, model);
            var document = DocumentHelper.GetDocumentFromHtml(this.Configuration, htmlString);
            return document;
        }


    }
}
