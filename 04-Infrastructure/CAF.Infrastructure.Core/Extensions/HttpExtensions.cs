using CAF.Infrastructure.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace CAF.Infrastructure.Core
{
    
    public static class HttpExtensions
    {
        private const string HTTP_CLUSTER_VAR = "HTTP_CLUSTER_HTTPS";
        
        /// <summary>
        /// Gets a value which indicates whether the HTTP connection uses secure sockets (HTTPS protocol). 
        /// Works with Cloud's load balancers.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsSecureConnection(this HttpRequestBase request)
        {
            return (request.IsSecureConnection || (request.ServerVariables[HTTP_CLUSTER_VAR] != null || request.ServerVariables[HTTP_CLUSTER_VAR] == "on"));
        }

        /// <summary>
        /// Gets a value which indicates whether the HTTP connection uses secure sockets (HTTPS protocol). 
        /// Works with Cloud's load balancers.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsSecureConnection(this HttpRequest request)
        {
            return (request.IsSecureConnection || (request.ServerVariables[HTTP_CLUSTER_VAR] != null || request.ServerVariables[HTTP_CLUSTER_VAR] == "on"));
        }
        public static void SetFormsAuthenticationCookie(this HttpWebRequest webRequest, HttpRequestBase httpRequest)
        {
            Guard.ArgumentNotNull(() => webRequest);
            Guard.ArgumentNotNull(() => httpRequest);

            var authCookie = httpRequest.Cookies[FormsAuthentication.FormsCookieName];
            var sendCookie = new Cookie(authCookie.Name, authCookie.Value, authCookie.Path, httpRequest.Url.Host);

            if (webRequest.CookieContainer == null)
            {
                webRequest.CookieContainer = new CookieContainer();
            }
            webRequest.CookieContainer.Add(sendCookie);
        }


        public static bool IsAdminArea(this HttpRequest request)
        {
            if (request != null)
            {
                return IsAdminArea(new HttpRequestWrapper(request));
            }

            return false;
        }

        public static bool IsAdminArea(this HttpRequestBase request)
        {
            try
            {
                if (request != null)
                {
                    var area = request.RequestContext.RouteData.GetAreaName();
                    if (area != null)
                    {
                        return area.IsCaseInsensitiveEqual("admin");
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsPublicArea(this HttpRequest request)
        {
            if (request != null)
            {
                return IsPublicArea(new HttpRequestWrapper(request));
            }

            return false;
        }

        public static bool IsPublicArea(this HttpRequestBase request)
        {
            try
            {
                if (request != null)
                {
                    var area = request.RequestContext.RouteData.GetAreaName();
                    return area.IsEmpty();
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
        public static PostedFileResult ToPostedFileResult(this HttpRequestBase httpRequest)
        {
            if (httpRequest != null && httpRequest.Files.Count > 0)
            {
                return httpRequest.Files[0].ToPostedFileResult();
            }

            return null;
        }

        public static PostedFileResult ToPostedFileResult(this HttpPostedFile httpFile)
        {
            if (httpFile != null && httpFile.ContentLength > 0)
            {
                return new PostedFileResult(new HttpPostedFileWrapper(httpFile));
            }

            return null;
        }

        public static PostedFileResult ToPostedFileResult(this HttpPostedFileBase httpFile)
        {
            if (httpFile != null && httpFile.ContentLength > 0)
            {
                return new PostedFileResult(httpFile);
            }

            return null;
        }

        public static IEnumerable<PostedFileResult> ToPostedFileResults(this HttpFileCollection httpFileCollection)
        {
            if (httpFileCollection != null && httpFileCollection.Count > 0)
            {
                return new HttpFileCollectionWrapper(httpFileCollection).ToPostedFileResults();
            }

            return Enumerable.Empty<PostedFileResult>();
        }

        public static IEnumerable<PostedFileResult> ToPostedFileResults(this HttpFileCollectionBase httpFileCollection)
        {
            if (httpFileCollection == null)
                yield break;

            var batchId = Guid.NewGuid();

            for (var i = 0; i < httpFileCollection.Count; i++)
            {
                var httpFile = httpFileCollection[i];
                var result = httpFile.ToPostedFileResult();
                if (result != null)
                {
                    result.BatchId = batchId;
                    yield return result;
                }
            }
        }

        public static RouteData GetRouteData(this HttpContextBase httpContext)
        {
            Guard.ArgumentNotNull(() => httpContext);

            var handler = httpContext.Handler as MvcHandler;
            if (handler != null && handler.RequestContext != null)
            {
                return handler.RequestContext.RouteData;
            }

            return null;
        }

        public static bool TryGetRouteData(this HttpContextBase httpContext, out RouteData routeData)
        {
            routeData = httpContext.GetRouteData();
            return routeData != null;
        }

        public static string GetUserThemeChoiceFromCookie(this HttpContextBase context)
        {
            if (context == null)
                return null;

            var cookie = context.Request.Cookies.Get("caf.UserThemeChoice");
            if (cookie != null)
            {
                return cookie.Value.NullEmpty();
            }

            return null;
        }

        public static void SetUserThemeChoiceInCookie(this HttpContextBase context, string value)
        {
            if (context == null)
                return;

            var cookie = context.Request.Cookies.Get("caf.UserThemeChoice");

            if (value.HasValue() && cookie == null)
            {
                cookie = new HttpCookie("caf.UserThemeChoice");
                cookie.HttpOnly = true;
                cookie.Expires = DateTime.UtcNow.AddYears(1);
            }

            if (value.HasValue())
            {
                cookie.Value = value;
                context.Request.Cookies.Set(cookie);
            }

            if (value.IsEmpty() && cookie != null)
            {
                cookie.Expires = DateTime.UtcNow.AddYears(-10);
            }

            if (cookie != null)
            {
                context.Response.SetCookie(cookie);
            }
        }

        public static HttpCookie GetPreviewModeCookie(this HttpContextBase context, bool createIfMissing)
        {
            if (context == null)
                return null;

            var cookie = context.Request.Cookies.Get("caf.PreviewModeOverrides");

            if (cookie == null && createIfMissing)
            {
                cookie = new HttpCookie("caf.PreviewModeOverrides");
                cookie.HttpOnly = true;
                context.Request.Cookies.Set(cookie);
            }

            if (cookie != null)
            {
                // when cookie gets created or touched, extend its lifetime
                cookie.Expires = DateTime.UtcNow.AddMinutes(20);
            }

            return cookie;
        }

        public static void SetPreviewModeValue(this HttpContextBase context, string key, string value)
        {
            if (context == null)
                return;

            var cookie = context.GetPreviewModeCookie(value.HasValue());
            if (cookie != null)
            {
                if (value.HasValue())
                {
                    cookie.Values[key] = value;
                }
                else
                {
                    cookie.Values.Remove(key);
                }
            }
        }

        public static IDisposable PreviewModeCookie(this HttpContextBase context)
        {
            var disposable = new ActionDisposable(() =>
            {
                var cookie = GetPreviewModeCookie(context, false);
                if (cookie != null)
                {
                    if (!cookie.HasKeys)
                    {
                        cookie.Expires = DateTime.UtcNow.AddYears(-10);
                    }
                    else
                    {
                        cookie.Expires = DateTime.UtcNow.AddMinutes(20);
                    }

                    context.Response.SetCookie(cookie);
                }
            });

            return disposable;
        }

    }

}
