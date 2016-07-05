using System.Web;
using System.Web.Mvc;

namespace CAF.WebSite.Application.WebUI.MvcCaptcha
{
    public class _MvcCaptchaController:Controller
    {

        public ActionResult MvcCaptchaImage()
        {
            return new MvcCaptchaImageResult();
        }

        public ActionResult MvcCaptchaLoader()
        {
            string prevGuid = Request.ServerVariables["Query_String"];
            if (!string.IsNullOrEmpty(prevGuid))
                HttpContext.Session.Remove(prevGuid);
            var options = new MvcCaptchaOptions() {  };
            var config=MvcCaptchaConfigSection.GetConfig();
            if(config!=null)
            {
                options.TextChars = config.TextChars;
                options.TextLength = config.TextLength;
                options.FontWarp = config.FontWarp;
                options.BackgroundNoise = config.BackgroundNoise;
                options.LineNoise = config.LineNoise;
            }
            
            var image = new MvcCaptchaImage(options);
            HttpContext.Session.Add(
                 image.UniqueId,
                 image);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return Content(image.UniqueId);
        }
    }
}
