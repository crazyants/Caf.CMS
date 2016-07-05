using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CAF.Infrastructure.Core.IO
{
    public class FileUpLoad
    {
        /// <summary>
        /// 文件上传方法
        /// </summary>
        /// <param name="postedFile">文件流</param>
        /// <param name="saveDir">上传目录名称</param>
        /// <returns>上传后文件信息</returns>
        public static string FileSaveAs(PostedFileResult postedFile,string saveDir)
        {
            try
            {
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
               
                int fileSize = postedFile.Size; //获得文件大小，以字节为单位
                string fileName = postedFile.FileName; //取得文件名有后缀
                string upLoadPath = GetUpLoadPath(saveDir); //上传目录相对路径
                string fullUpLoadPath = webHelper.MapPath(upLoadPath); //上传目录的物理路径
                string newFilePath = upLoadPath + postedFile.FileName ; //上传后的路径

                //检查文件扩展名是否合法
                //if (!CheckFileExt(fileExt))
                //{
                //    return "{\"status\": 0, \"msg\": \"不允许上传" + fileExt + "类型的文件！\"}";
                //}
                ////检查文件大小是否合法
                //if (!CheckFileSize(fileExt, fileSize))
                //{
                //    return "{\"status\": 0, \"msg\": \"文件超过限制的大小！\"}";
                //}
                //检查上传的物理路径是否存在，不存在则创建
                if (!Directory.Exists(fullUpLoadPath))
                {
                    Directory.CreateDirectory(fullUpLoadPath);
                }

                //保存文件
                postedFile.File.SaveAs(fullUpLoadPath + fileName);
                return newFilePath;
            }
            catch
            {
                return "";
            }
        }
        #region 私有方法
        /// <summary>
        /// 返回上传目录相对路径
        /// </summary>
        /// <param name="fileName">上传文件名</param>
        private static string GetUpLoadPath(string folderName)
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            string path = "/Media/Files/{0}/".FormatCurrent(folderName); //站点目录+上传目录

            //按年月/日存入不同的文件夹
            path += DateTime.Now.ToString("yyyyMM") + "/" + DateTime.Now.ToString("dd");

            return path + "/";
        }


        #endregion

    }
}
