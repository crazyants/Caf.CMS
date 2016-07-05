using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;
using System.Web;
using System.Reflection;

namespace CAF.WebSite.Application.WebUI.Common
{

   // [StrongNameIdentityPermissionAttribute(SecurityAction.LinkDemand, PublicKey =""]

    /// <summary>
    /// CacheHelper 数据缓存操作类
    /// </summary>
    public class CacheHelper
    {
        
        //public static Cache cache = new Cache();
        //如果不能运行请修改成
        public static Cache cache = HttpContext.Current.Cache;
        public CacheHelper()
        {

        }

        /// <summary>
        /// 缓存相对过期，最后一次访问后minute分钟后过期
        /// </summary>
        ///<param name="key">Cache键值</param>
        ///<param name="value">给Cache[key]赋的值</param>
        ///<param name="minute">滑动过期分钟</param>
        public static void CacheInsertFromMinutes(string key, object value, int minute)
        {
            if (value == null) return;
            cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(minute));
        }


        /// <summary>
        /// 缓存绝对过期时间
        /// </summary>
        ///<param name="key">Cache键值</param>
        ///<param name="value">给Cache[key]赋的值</param>
        ///<param name="minute">minute分钟后绝对过期</param>
        public static void CacheInsertAddMinutes(string key, object value, int minute)
        {
            cache.Insert(key, value, null, DateTime.Now.AddMinutes(minute), System.Web.Caching.Cache.NoSlidingExpiration);
        }


        /// <summary>
        ///以key键值，把value赋给Cache[key].如果不主动清空，会一直保存在内存中.
        /// </summary>
        ///<param name="key">Cache键值</param>
        ///<param name="value">给Cache[key]赋的值</param>
        public static void CacheInsert(string key, object value)
        {
            cache.Insert(key, value);
        }

        /// <summary>
        ///清除Cache[key]的值
        /// </summary>
        ///<param name="key"></param>
        public static void CacheNull(string key)
        {
            cache.Remove(key);
        }

        /// <summary>
        ///根据key值，返回Cache[key]的值
        /// </summary>
        ///<param name="key"></param>
        public static object CacheValue(string key)
        {
            return cache.Get(key);
        }
        /// <summary>
        /// 获取cache的过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static DateTime GetCacheUtcExpiryDateTime(string key)
        {
            try
            {
                object cacheEntry = cache.GetType().GetMethod("Get", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(cache, new object[] { key, 1 });
                PropertyInfo utcExpiresProperty = cacheEntry.GetType().GetProperty("UtcExpires", BindingFlags.NonPublic | BindingFlags.Instance);
                DateTime utcExpiresValue = (DateTime)utcExpiresProperty.GetValue(cacheEntry, null);
                return utcExpiresValue.ToLocalTime();
            }
            catch { return DateTime.Now; }
        }
    }

}
