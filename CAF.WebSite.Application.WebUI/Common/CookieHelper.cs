using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CAF.WebSite.Application.WebUI.Common
{
    /// <summary> 
    /// Cookie操作类 
    /// </summary> 
    public class CookieHelper
    {
        /// <summary> 
        /// 保存一个Cookie 
        /// </summary> 
        /// <param name="CookieName">Cookie名称</param> 
        /// <param name="CookieValue">Cookie值</param> 
        /// <param name="CookieTime">Cookie过期时间(小时),0为关闭页面失效</param> 
        public static void SaveCookie(string CookieName, string CookieValue, double CookieTime)
        {
            HttpCookie myCookie = new HttpCookie(CookieName);
            DateTime now = DateTime.Now;
            myCookie.Value = DEncrypt.Encrypt(HttpUtility.HtmlEncode(CookieValue), "chinacloudtech");
            if (CookieTime != 0)
            {
                //有两种方法，第一方法设置Cookie时间的话，关闭浏览器不会自动清除Cookie 
                //第二方法不设置Cookie时间的话，关闭浏览器会自动清除Cookie ,但是有效期 
                //多久还未得到证实。 
                myCookie.Expires = now.AddMinutes(CookieTime);
                if (HttpContext.Current.Response.Cookies[CookieName] != null)
                    HttpContext.Current.Response.Cookies.Remove(CookieName);
                HttpContext.Current.Response.Cookies.Add(myCookie);
            }
            else
            {
                if (HttpContext.Current.Response.Cookies[CookieName] != null)
                    HttpContext.Current.Response.Cookies.Remove(CookieName);

                HttpContext.Current.Response.Cookies.Add(myCookie);
            }
        }
        /// <summary> 
        /// 取得CookieValue 
        /// </summary> 
        /// <param name="CookieName">Cookie名称</param> 
        /// <returns>Cookie的值</returns> 
        public static string GetCookie(string CookieName)
        {
            HttpCookie myCookie = new HttpCookie(CookieName);
            myCookie = HttpContext.Current.Request.Cookies[CookieName];
            if (myCookie != null)
            {
                string cookievalue = DEncrypt.Decrypt(myCookie.Value, "chinacloudtech");
                return HttpUtility.HtmlDecode(cookievalue);
            }
            else
                return null;
        }
        /// <summary> 
        /// 清除CookieValue 
        /// </summary> 
        /// <param name="CookieName">Cookie名称</param> 
        public static void ClearCookie(string CookieName)
        {
            HttpCookie myCookie = new HttpCookie(CookieName);
            DateTime now = DateTime.Now;

            myCookie.Expires = now.AddYears(-2);

            HttpContext.Current.Response.Cookies.Add(myCookie);
        }
    }
}
