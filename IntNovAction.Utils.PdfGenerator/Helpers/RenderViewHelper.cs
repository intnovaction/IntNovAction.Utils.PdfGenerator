using IntNovAction.Utils.PdfGenerator.Configuration;
using IntNovAction.Utils.PdfGenerator.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;

namespace IntNovAction.Utils.PdfGenerator.Helpers
{
    public static class RenderViewHelper
    {

        internal static string Render(DocumentConfiguration config, string view, object model)
        {

            var controller = new EmptyController();

            controller.ViewData.Model = model;

            var httpBase = new HttpContextWrapper(HttpContext.Current);
            var route = new RouteData();
            route.Values.Add("controller", "value");
            var controllerContext = new ControllerContext(httpBase, route, controller);

            using (var sw = new StringWriter())
            {
                var viewResult = !string.IsNullOrWhiteSpace(config.MasterName) ?
                    ViewEngines.Engines.FindView(controllerContext, view, config.MasterName) :
                    ViewEngines.Engines.FindPartialView(controllerContext, view);

                var viewContext = new ViewContext(controllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                var result = sw.GetStringBuilder().ToString();

                return result;
            }
        }


    }
}
