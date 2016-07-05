using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Media;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Controllers
{
    public partial class DownloadController : PublicControllerBase
    {
        private readonly IDownloadService _downloadService;
        private readonly IArticleService _articleService;
        private readonly IWorkContext _workContext;

        private readonly UserSettings _cserSettings;

        public DownloadController(IDownloadService downloadService, IArticleService articleService,
           IWorkContext workContext, UserSettings cserSettings)
        {
            this._downloadService = downloadService;
            this._articleService = articleService;
            this._workContext = workContext;
            this._cserSettings = cserSettings;
        }
        /// <summary>
        /// 从内容中获取文件信息并下载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Sample(int id /* articleId */)
        {

            var article = _articleService.GetArticleById(id);
            if (article == null)
                return HttpNotFound();

            if (!article.IsDownload)
                return Content("Article variant doesn't have a sample download.");

            var download = _downloadService.GetDownloadById(article.DownloadId);
            if (download == null)
                return Content("Sample download is not available any more.");

            if (!article.UnlimitedDownloads && article.DownloadCount >= article.MaxNumberOfDownloads)
                return Content(string.Format("You have reached maximum number of downloads {0}", article.MaxNumberOfDownloads));

            if (download.UseDownloadUrl)
            {
                article.DownloadCount++;
                _articleService.UpdateArticle(article);
                return new RedirectResult(download.DownloadUrl);
            }
            else
            {
                if (download.DownloadBinary == null)
                    return Content("Download data is not available any more.");
                //increase download
                article.DownloadCount++;
                _articleService.UpdateArticle(article);
                string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : article.Id.ToString();
                string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
                return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
            }
        }

        /// <summary>
        /// 从文件存储中下载内容
        /// </summary>
        /// <param name="downloadId"></param>
        /// <returns></returns>
        public ActionResult GetFileUpload(Guid downloadId)
        {
            var download = _downloadService.GetDownloadByGuid(downloadId);
            if (download == null)
                return Content("Download is not available any more.");

            if (download.UseDownloadUrl)
            {
                //return result
                return new RedirectResult(download.DownloadUrl);
            }
            else
            {
                if (download.DownloadBinary == null)
                    return Content("Download data is not available any more.");

                //return result
                string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : downloadId.ToString();
                string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
                return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
            }
        }

        public ActionResult GetFileUploadById(int id)
        {
            var download = _downloadService.GetDownloadById(id);
            if (download == null)
                return Content("Download is not available any more.");

            if (download.UseDownloadUrl)
            {
                //return result
                return new RedirectResult(download.DownloadUrl);
            }
            else
            {
                if (download.DownloadBinary == null)
                    return Content("Download data is not available any more.");

                //return result
                string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : id.ToString();
                string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
                return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
            }
        }
    }
}
