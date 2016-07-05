using CAF.WebSite.Application.Services.Pdf;
using System;
using System.Web.Mvc;
 

namespace CAF.WebSite.Application.WebUI.Pdf
{

	public class PdfPartialViewContent : PdfHtmlContent
	{
		public PdfPartialViewContent(string partialViewName, object model, ControllerContext controllerContext)
			: base(PdfViewContent.ViewToString(partialViewName, null, model, true, controllerContext, false))
		{
		}
	}

}
