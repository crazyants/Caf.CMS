using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using CAF.Infrastructure.Core;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.IO;

namespace CAF.WebSite.Mvc.Admin.Controllers
{
   // [AdminAuthorize]
    public class DownloadController : AdminControllerBase
    {
        const string TEMPLATE = "EditorTemplates/Download";

        private readonly IDownloadService _downloadService;
        private readonly ISiteContext _webSiteContext;
        public DownloadController(IDownloadService downloadService,
            ISiteContext webSiteContext)
        {
            this._downloadService = downloadService;
            this._webSiteContext = webSiteContext;
        }

        public ActionResult DownloadFile(int downloadId)
        {
            var download = _downloadService.GetDownloadById(downloadId);
            if (download == null)
                return Content("No download record found with the specified id");

            if (download.UseDownloadUrl)
            {
                return new RedirectResult(download.DownloadUrl);
            }
            else
            {
                //use stored data
                if (download.DownloadBinary == null)
                    return Content(string.Format("Download data is not available any more. Download ID={0}", downloadId));

                string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : downloadId.ToString();
                string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
                return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveDownloadUrl(string downloadUrl, bool minimalMode = false, string fieldName = null)
        {
            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = true,
                DownloadUrl = downloadUrl,
                IsNew = true,
                IsTransient = true
            };

            _downloadService.InsertDownload(download);

            return Json(new
            {
                success = true,
                downloadId = download.Id,
                html = this.RenderPartialViewToString(TEMPLATE, download.Id, new { minimalMode = minimalMode, fieldName = fieldName })
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AsyncUpload(bool minimalMode = false, string fieldName = null)
        {
            var postedFile = Request.ToPostedFileResult();
            if (postedFile == null)
            {
                throw new ArgumentException("No file uploaded");
            }
            string fileNameKey = Guid.NewGuid().ToString();
           
            postedFile.FileName = fileNameKey + postedFile.FileExtension;
            string filePath = FileUpLoad.FileSaveAs(postedFile, this._webSiteContext.CurrentSite.Id.ToString());

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = true,
                DownloadUrl = filePath,
                //DownloadBinary = postedFile.Buffer,
                ContentType = postedFile.ContentType,
                // we store filename without extension for downloads
                Filename = postedFile.FileTitle,
                Extension = postedFile.FileExtension,
                IsNew = true,
                IsTransient = true
            };

            _downloadService.InsertDownload(download);

            return Json(new
            {
                success = true,
                downloadId = download.Id,
                fileName = download.Filename + download.Extension,
                downloadUrl = Url.Action("DownloadFile", new { downloadId = download.Id }), 
                html = this.RenderPartialViewToString(TEMPLATE, download.Id, new { minimalMode = minimalMode, fieldName = fieldName })
            });
        }

        [HttpPost]
        public ActionResult DeleteDownload(bool minimalMode = false, string fieldName = null)
        {
            // We don't actually delete here. We just return the editor in it's init state
            // so the download entity can be set to transient state and deleted later by a scheduled task.
            return Json(new
            {
                success = true,
                html = this.RenderPartialViewToString(TEMPLATE, null, new { minimalMode = minimalMode, fieldName = fieldName }),
            });
        }
    }
}
