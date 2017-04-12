using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntNovAction.Utils.PdfGenerator.Configuration
{
    public enum LayoutType
    {
        A3,
        A4,
        A5,
    }       

    public class DocumentConfiguration
    {
        

        private static Dictionary<LayoutType, Rectangle> ITextSharpLayouts = new Dictionary<LayoutType, Rectangle>
        {
            { LayoutType.A3, PageSize.A3 },
            { LayoutType.A4, PageSize.A4 },
            { LayoutType.A5, PageSize.A5 },
        };

        public LayoutType Layout { get; set; } = LayoutType.A4;
        public bool Landscape { get; set; } = false;

        internal Rectangle ITextSharpLayout => this.Landscape ? ITextSharpLayouts[Layout].Rotate() : ITextSharpLayouts[Layout];

        /// <summary>
        /// ruta del layout. Si no se especifica se renderiza una vista parcial
        /// </summary>
        public string MasterName { get; set; }

        /// <summary>
        /// Hojas de estilo a aplicar. Sólo son necesarias si ReplaceCssPaths es False (se quiere usar unas hojas de estilo distintas)
        /// </summary>
        public List<string> StyleSheets { get; internal set; } = new List<string>();

        /// <summary>
        /// Indica si se sustituiran en la vista los paths de las hojas de estilo por paths locales de la máquina: El generador sólo interpreta hojas de estilo
        /// locales. 
        /// </summary>
        public bool ReplaceCssPaths { get; set; } = true;

    }
}
