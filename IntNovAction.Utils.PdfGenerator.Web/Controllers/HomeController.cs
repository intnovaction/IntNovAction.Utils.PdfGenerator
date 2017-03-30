using IntNovAction.Utils.PdfGenerator.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntNovAction.Utils.PdfGenerator.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new IndexModel() { ShowButtons = true };
            return View(model);
        }


        public virtual FileResult Print(bool landscape)
        {
            var filename = string.Format("print{0}.pdf", landscape ? "-landscape" : "");
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = true
            };

            var pdfGenerator = new PdfGenerator();

            //pdfGenerator.StyleSheets.Add("~/Content/bootstrap.css");
            pdfGenerator.Configuration.StyleSheets.Add("~/Content/site.css");
            pdfGenerator.Configuration.Landscape = landscape;

            pdfGenerator.Configuration.MasterName = "~/Views/Shared/_Layout.cshtml";
            var model = new IndexModel() { ShowButtons = false };

            var content = pdfGenerator.GetDocument("~/Views/Home/Index.cshtml", model);

            this.Response.AppendHeader("Content-Disposition", cd.ToString());
            return new FileContentResult(content, "application/pdf");
        }
    }
}