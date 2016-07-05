using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.Security;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Diagnostics;
using System.Threading;

namespace CAF.Infrastructure.Core.IO
{
    public partial class CFiles
    {
        private const string PATH_SPLIT_CHAR = "\\";

        #region 删除指定目录的所有文件和子目录
        /// <summary>
        /// 删除指定目录的所有文件和子目录
        /// </summary>
        /// <param name="TargetDir">操作目录</param>
        /// <param name="delSubDir">如果为true,包含对子目录的操作</param>
        public static void DeleteDirectoryFiles(string TargetDir, bool delSubDir)
        {
            foreach (string fileName in Directory.GetFiles(TargetDir))
            {
                File.SetAttributes(fileName, FileAttributes.Normal);
                File.Delete(fileName);
            }
            if (delSubDir)
            {
                DirectoryInfo dir = new DirectoryInfo(TargetDir);
                foreach (DirectoryInfo subDi in dir.GetDirectories())
                {
                    DeleteDirectoryFiles(subDi.FullName, true);
                    subDi.Delete();
                }
            }
        }
        #endregion

        #region 删除指定目录下的指定文件
        /// <summary>
        /// 删除指定目录下的指定文件
        /// </summary>
        /// <param name="TargetFileDir">指定文件的目录</param>
        public static void DeleteFiles(string TargetFileDir)
        {
            File.Delete(TargetFileDir);
        }
        #endregion

        #region 创建指定目录
        /// <summary>
        /// 创建指定目录
        /// </summary>
        /// <param name="targetDir"></param>
        public static void CreateDirectory(string targetDir)
        {
            DirectoryInfo dir = new DirectoryInfo(targetDir);
            if (!dir.Exists)
                dir.Create();
        }
        #endregion

        #region 建立子目录
        /// <summary>
        /// 建立子目录
        /// </summary>
        /// <param name="parentDir">目录路径</param>
        /// <param name="subDirName">子目录名称</param>
        public static void CreateDirectory(string parentDir, string subDirName)
        {
            CreateDirectory(parentDir + PATH_SPLIT_CHAR + subDirName);
        }
        #endregion

        #region 重命名文件夹目录
        /// <summary>
        /// 重命名文件夹
        /// </summary>
        /// <param name="OldFloderName">原路径文件夹名称</param>
        /// <param name="NewFloderName">新路径文件夹名称</param>
        /// <returns></returns>
        public static bool ReNameFloder(string OldFloderName, string NewFloderName)
        {
            try
            {
                if (Directory.Exists(HttpContext.Current.Server.MapPath("//") + OldFloderName))
                {
                    Directory.Move(HttpContext.Current.Server.MapPath("//") + OldFloderName, HttpContext.Current.Server.MapPath("//") + NewFloderName);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 删除指定目录
        /// <summary>
        /// 删除指定目录
        /// </summary>
        /// <param name="targetDir">目录路径</param>
        public static void DeleteDirectory(string targetDir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(targetDir);
            if (dirInfo.Exists)
            {
                DeleteDirectoryFiles(targetDir, true);
                dirInfo.Delete(true);
            }
        }
        #endregion

        #region 检测目录是否存在
        /// <summary>
        /// 检测目录是否存在
        /// </summary>
        /// <param name="StrPath">路径</param>
        /// <returns></returns>
        public static bool DirectoryIsExists(string StrPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(StrPath);
            return dirInfo.Exists;
        }
        /// <summary>
        /// 检测目录是否存在
        /// </summary>
        /// <param name="StrPath">路径</param>
        /// <param name="Create">如果不存在，是否创建</param>
        public static void DirectoryIsExists(string StrPath, bool Create)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(StrPath);
            //return dirInfo.Exists;
            if (!dirInfo.Exists)
            {
                if (Create) dirInfo.Create();
            }
        }
        #endregion

        #region 删除指定目录的所有子目录,不包括对当前目录文件的删除
        /// <summary>
        /// 删除指定目录的所有子目录,不包括对当前目录文件的删除
        /// </summary>
        /// <param name="targetDir">目录路径</param>
        public static void DeleteSubDirectory(string targetDir)
        {
            foreach (string subDir in Directory.GetDirectories(targetDir))
            {
                DeleteDirectory(subDir);
            }
        }
        #endregion

        #region 复制指定目录的所有文件
        /// <summary>
        /// 复制指定目录的所有文件
        /// </summary>
        /// <param name="sourceDir">原始目录</param>
        /// <param name="targetDir">目标目录</param>
        /// <param name="overWrite">如果为true,覆盖同名文件,否则不覆盖</param>
        /// <param name="copySubDir">如果为true,包含目录,否则不包含</param>
        public static void CopyFiles(string sourceDir, string targetDir, bool overWrite, bool copySubDir)
        {
            //复制当前目录文件
            foreach (string sourceFileName in Directory.GetFiles(sourceDir))
            {
                string targetFileName = Path.Combine(targetDir, sourceFileName.Substring(sourceFileName.LastIndexOf(PATH_SPLIT_CHAR) + 1));

                if (File.Exists(targetFileName))
                {
                    if (overWrite == true)
                    {
                        File.SetAttributes(targetFileName, FileAttributes.Normal);
                        File.Copy(sourceFileName, targetFileName, overWrite);
                    }
                }
                else
                {
                    File.Copy(sourceFileName, targetFileName, overWrite);
                }
            }
        }
        #endregion

        #region 移动指定目录的所有文件
        /// <summary>
        /// 移动指定目录的所有文件
        /// </summary>
        /// <param name="sourceDir">原始目录</param>
        /// <param name="targetDir">目标目录</param>
        /// <param name="overWrite">如果为true,覆盖同名文件,否则不覆盖</param>
        /// <param name="moveSubDir">如果为true,包含目录,否则不包含</param>
        public static void MoveFiles(string sourceDir, string targetDir, bool overWrite, bool moveSubDir)
        {
            //移动当前目录文件
            foreach (string sourceFileName in Directory.GetFiles(sourceDir))
            {
                string targetFileName = Path.Combine(targetDir, sourceFileName.Substring(sourceFileName.LastIndexOf(PATH_SPLIT_CHAR) + 1));
                if (File.Exists(targetFileName))
                {
                    if (overWrite == true)
                    {
                        File.SetAttributes(targetFileName, FileAttributes.Normal);
                        File.Delete(targetFileName);
                        File.Move(sourceFileName, targetFileName);
                    }
                }
                else
                {
                    File.Move(sourceFileName, targetFileName);
                }
            }
            if (moveSubDir)
            {
                foreach (string sourceSubDir in Directory.GetDirectories(sourceDir))
                {
                    string targetSubDir = Path.Combine(targetDir, sourceSubDir.Substring(sourceSubDir.LastIndexOf(PATH_SPLIT_CHAR) + 1));
                    if (!Directory.Exists(targetSubDir))
                        Directory.CreateDirectory(targetSubDir);
                    MoveFiles(sourceSubDir, targetSubDir, overWrite, true);
                    Directory.Delete(sourceSubDir);
                }
            }
        }
        #endregion
    }
}
