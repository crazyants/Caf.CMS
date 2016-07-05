using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class FileController : AdminControllerBase
    {
        private readonly IPictureService _pictureService;
        private readonly IPermissionService _permissionService;

        public FileController(IPictureService pictureService,
             IPermissionService permissionService)
        {
            this._pictureService = pictureService;
            this._permissionService = permissionService;
        }

        [HttpPost]
        public ActionResult SaveUploadedFile()
        {
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            HttpPostedFileBase httpPostedFile = null;
            List<int> pictureIds = new List<int>();
            foreach (string fName in Request.Files)
            {
                if (String.IsNullOrEmpty(Request["qqfile"]))
                {

                    // IE
                    httpPostedFile = Request.Files[fName];
                    if (httpPostedFile == null)
                        throw new ArgumentException("No file uploaded");
                    stream = httpPostedFile.InputStream;
                    fileName = Path.GetFileName(httpPostedFile.FileName);
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

                // var picture = _pictureService.InsertPicture(fileBinary, contentType, null, true);
                //pictureIds.Add(picture.Id);
                if (httpPostedFile != null && httpPostedFile.ContentLength > 0)
                {
                    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\WallImages", Server.MapPath(@"\")));

                    string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "imagepath");

                    var fileName1 = Path.GetFileName(httpPostedFile.FileName);


                    bool isExists = System.IO.Directory.Exists(pathString);

                    if (!isExists)
                        System.IO.Directory.CreateDirectory(pathString);

                    var path = string.Format("{0}\\{1}", pathString, httpPostedFile.FileName);
                    httpPostedFile.SaveAs(path);
                }
            }

            return Json(
                 new
                 {
                     success = true,
                 },
                 "text/plain");

        }
    }
}
