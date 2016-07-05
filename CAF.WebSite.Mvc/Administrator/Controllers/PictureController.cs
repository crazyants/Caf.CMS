using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using CAF.WebSite.Application.WebUI;
using CAF.Infrastructure.Core;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.WebUI.Controllers;


namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class PictureController : AdminControllerBase
    {
        private readonly IPictureService _pictureService;
        private readonly IArticleService _articleService;
        private readonly IPermissionService _permissionService;

        public PictureController(IPictureService pictureService,
             IPermissionService permissionService,
             IArticleService articleService)
        {
            this._pictureService = pictureService;
            this._articleService = articleService;
            this._permissionService = permissionService;
        }

        /// <summary>
        /// FileUpload图片异步更新
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AsyncUpload(bool isTransient = false)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UploadPictures))
                return Json(new { success = false, error = "You do not have the required permissions" });

            var postedFile = Request.ToPostedFileResult();

            var picture = _pictureService.InsertPicture(postedFile.Buffer, postedFile.ContentType, null, true, isTransient);

            return Json(
                new
                {
                    success = true,
                    pictureId = picture.Id,
                    imageUrl = _pictureService.GetPictureUrl(picture, 100)
                });
        }
        /// <summary>
        /// DropZone图片保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveUploaded()
        {
            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            var articleId = 0;
            if (String.IsNullOrEmpty(Request["qqfile"]))
            {
                // IE
                HttpPostedFileBase httpPostedFile = Request.Files[0];
                if (httpPostedFile == null)
                    throw new ArgumentException("No file uploaded");
                stream = httpPostedFile.InputStream;
                fileName = Path.GetFileName(httpPostedFile.FileName);
                articleId = Request.Params["id"].ToInt();
                contentType = httpPostedFile.ContentType;

            }
            else
            {
                //Webkit, Mozilla
                stream = Request.InputStream;
                fileName = Request["qqfile"];

            }

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();
            //contentType is not always available 
            //that's why we manually update it here
            //http://www.sfsu.edu/training/mimetype.htm
            if (String.IsNullOrEmpty(contentType))
            {
                switch (fileExtension)
                {
                    case ".bmp":
                        contentType = "image/bmp";
                        break;
                    case ".gif":
                        contentType = "image/gif";
                        break;
                    case ".jpeg":
                    case ".jpg":
                    case ".jpe":
                    case ".jfif":
                    case ".pjpeg":
                    case ".pjp":
                        contentType = "image/jpeg";
                        break;
                    case ".png":
                        contentType = "image/png";
                        break;
                    case ".tiff":
                    case ".tif":
                        contentType = "image/tiff";
                        break;
                    default:
                        break;
                }
            }

            var picture = _pictureService.InsertPicture(fileBinary, contentType, null, true);
            if (articleId == 0)
            {
                var article = _articleService.GetArticleById(articleId);
                if (article == null)
                    throw new ArgumentException("No article found with the specified id");

                _articleService.InsertArticleAlbum(new ArticleAlbum()
                {
                    PictureId = picture.Id,
                    ArticleId = articleId,
                    DisplayOrder = 99,
                });

                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(article.Title));
            }
            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new { success = true, pictureId = picture.Id });
        }
        /// <summary>
        /// 图片删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ArticlePictureDelete(int id)
        {


            var articlePicture = _articleService.GetArticleAlbumById(id);
            if (articlePicture == null)
                throw new ArgumentException("No article picture found with the specified id");

            var articleId = articlePicture.ArticleId;
            _articleService.DeleteArticleAlbum(articlePicture);

            var picture = _pictureService.GetPictureById(articlePicture.PictureId);
            _pictureService.DeletePicture(picture);

            return Json(new { success = true });
        }
        /// <summary>
        /// 获取图片列表
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public ActionResult GetAttachments(int articleId)
        {
            var articlePictures = _articleService.GetArticleAlbumsByArticleId(articleId);
            var articlePicturesModel = articlePictures
                .Select(x =>
                {
                    return new ArticleModel.ArticlePictureModel()
                    {
                        Id = x.Id,
                        ArticleId = x.ArticleId,
                        PictureId = x.PictureId,
                        PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                        DisplayOrder = x.DisplayOrder,
                        SeoFilename = x.Picture.SeoFilename
                    };
                })
                .ToList();

            return Json(new { Data = articlePicturesModel }, JsonRequestBehavior.AllowGet);


        }
    }
}
