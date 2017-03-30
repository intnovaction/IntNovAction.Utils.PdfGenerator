# IntNovAction.Utils.PdfGenerator
Utilidad para exportar una vista de MVC a pdf (como array de bytes)

```c#
var pdfGenerator = new PdfGenerator();
var model = new IndexModel() { ShowButtons = false };
var content = pdfGenerator.GetDocument("~/Views/Home/Index.cshtml", model);
```

## Opciones de configuración
La clase tiene una propiedad de tipo DocumentConfiguration para establecer distintas opciones del renderizado

### LayoutType
Tamaño de página. Los valores que admite son A3, A4 y A5 (por defecto A4)

```c#
var pdfGenerator = new PdfGenerator();
pdfGenerator.Configuration.Layout = Configuration.LayoutType.A4;
var content = pdfGenerator.GetDocument("~/Views/Home/Index.cshtml", model);
```

### Landscape
bool que indica si el documento se generará apaisado (por defecto false)

```c#
var pdfGenerator = new PdfGenerator();
pdfGenerator.Configuration.Landscape = false;
var content = pdfGenerator.GetDocument("~/Views/Home/Index.cshtml", model);
```
### MasterName
En esta propiedad se indica el path del layout que utiliza la vista. Si no se especifica un valor, se renderiza como vista parcial (no renderiza el layout)

```c#
var pdfGenerator = new PdfGenerator();
pdfGenerator.Configuration.MasterName = "~/Views/Shared/_Layout.cshtml";
var content = pdfGenerator.GetDocument("~/Views/Home/Index.cshtml", model);
```

### ReplaceImagePaths

bool para indicar si se van a reemplazar los paths de las imágenes que se encuentren en la vista. Reemplaza el path de todas aquellas imágenes cuyo src empiece por '/' o '~/' por paths locales de la máquina (si las imágenes no tienen path locales, el motor no las renderiza en el pdf).
El valor por defecto es true

```c#
var pdfGenerator = new PdfGenerator();
pdfGenerator.Configuration.ReplaceImagePaths = true;
var content = pdfGenerator.GetDocument("~/Views/Home/Index.cshtml", model);
```

### ReplaceCssPaths

bool para indicar si se van a reemplazar los paths de las hojas de estilo que se encuentren en la vista. Reemplaza el path de todas aquellos link cuyo href empiece por '/' o '~/' por paths locales de la máquina (si las hojas de estilo no tienen path locales, el motor no las interpreta).
El valor por defecto es true

```c#
var pdfGenerator = new PdfGenerator();
pdfGenerator.Configuration.ReplaceCssPaths = true;
var content = pdfGenerator.GetDocument("~/Views/Home/Index.cshtml", model);
```

### StyleSheets

Lista de cadenas con paths a las hojas de estilo que se quieren aplicar en el documento. Útil por ejemplo si no queremos usar el Layout (MasterName vacío) pero se quiere aplicar un estilo

```c#
var pdfGenerator = new PdfGenerator();
pdfGenerator.Configuration.StyleSheets.Add("~/Content/site.css");
var content = pdfGenerator.GetDocument("~/Views/Home/Index.cshtml", model);
```
