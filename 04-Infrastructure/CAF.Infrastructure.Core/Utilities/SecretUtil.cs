using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace CAF.Infrastructure.Core.Utilities
{
   /// <summary>
    /// 网站安全通用防护
    /// 
    /// 主要功能：
    ///          拦截攻击者注入恶意代码，可以防御诸如跨站脚本攻击（XSS）、SQL注入攻击等恶意攻击行为。
    /// </summary>
    public partial class SecretUtil
    {
        //正则过滤字符
        private const string StrRegex = @"<[^>]+?style=[\w]+?:expression\(|\b(alert|confirm|prompt)\b|^\+/v(8|9)|<[^>]*?=[^>]*?&#[^>]*?>|\b(and|or)\b.{1,6}?(=|>|<|\bin\b|\blike\b)|/\*.+?\*/|<\s*script\b|<\s*img\b|\bEXEC\b|UNION.+?SELECT|UPDATE.+?SET|INSERT\s+INTO.+?VALUES|(SELECT|DELETE).+?FROM|(CREATE|ALTER|DROP|TRUNCATE)\s+(TABLE|DATABASE)";
        /// <summary>
        ///  POST请求
        /// </summary>
        /// <param name="putData">输出非法字符串</param>
        /// <returns></returns>
        public static bool PostData(out string putData)
        {
            bool result = false;
            putData = string.Empty;
            for (int i = 0; i < HttpContext.Current.Request.Form.Count; i++)
            {
                result = CheckData(HttpContext.Current.Request.Form[i].ToString(), out putData);
                if (result)
                {
                    putData = HttpContext.Current.Request.Form[i].ToString();
                    break;
                }
            }
            return result;
        }
        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="putData">输出非法字符串</param>
        /// <returns></returns>
        public static bool GetData(out string putData)
        {
            bool result = false;
            putData = string.Empty;
            for (int i = 0; i < HttpContext.Current.Request.QueryString.Count; i++)
            {
                result = CheckData(HttpContext.Current.Request.QueryString[i].ToString(), out putData);
                if (result)
                {
                    putData = HttpContext.Current.Request.QueryString[i].ToString();
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// COOKIE
        /// </summary>
        /// <param name="putData">输出非法字符串</param>
        /// <returns></returns>
        public static bool CookieData(out string putData)
        {
            bool result = false;
            putData = string.Empty;
            for (int i = 0; i < HttpContext.Current.Request.Cookies.Count; i++)
            {
                result = CheckData(HttpContext.Current.Request.Cookies[i].Value.ToLower(), out putData);
                if (result)
                {
                    putData = HttpContext.Current.Request.Cookies[i].Value.ToLower();
                    break;
                }
            }
            return result;

        }
        /// <summary>
        /// UrlReferrer请求来源
        /// </summary>
        /// <param name="putData">输出非法字符串</param>
        /// <returns></returns>
        public static bool Referer(out string putData)
        {
            bool result = false;
            result = CheckData(HttpContext.Current.Request.UrlReferrer.ToString(), out putData);
            if (result)
            {
                putData = HttpContext.Current.Request.UrlReferrer.ToString();
            }
            return result;
        }
        /// <summary>
        /// 正则检查
        /// </summary>
        /// <param name="inputData">字符串</param>
        /// <param name="putData">输出非法字符串</param>
        /// <returns></returns>
        public static bool CheckData(string inputData, out string putData)
        {
            putData = string.Empty;
            //if (Regex.IsMatch(inputData.ToUpper(), StrRegex.ToUpper(),RegexOptions.IgnoreCase))
            if (Regex.IsMatch(inputData, StrRegex, RegexOptions.IgnoreCase))
            {
                putData = inputData;
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
