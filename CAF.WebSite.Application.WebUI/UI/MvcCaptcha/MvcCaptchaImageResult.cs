using System;
using System.Drawing.Imaging;
using System.Web.Mvc;

namespace CAF.WebSite.Application.WebUI.MvcCaptcha
{
    internal class MvcCaptchaImageResult:ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            string guid = context.HttpContext.Request.ServerVariables["Query_String"];
            if (guid.Contains("&"))
                guid = guid.Split('&')[0];
            var ci = MvcCaptchaImage.GetCachedCaptcha(guid);
            if (String.IsNullOrEmpty(guid)||ci == null)
            {
                context.HttpContext.Response.StatusCode = 404;
                context.HttpContext.Response.StatusDescription = "Not Found";
                context.HttpContext.Response.End();
                return;
            }
            ci.ResetText();
            using (var b = ci.RenderImage())
            {
                b.Save(context.HttpContext.Response.OutputStream, ImageFormat.Gif);
            }
            context.HttpContext.Response.Cache.SetNoStore();
            context.HttpContext.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

            context.HttpContext.Response.ContentType = "image/gif";
            context.HttpContext.Response.StatusCode = 200;
            context.HttpContext.Response.StatusDescription = "OK";
            context.HttpContext.ApplicationInstance.CompleteRequest();
        }
    }
}