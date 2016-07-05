
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Users;
using System.Web.Mvc;
using CAF.WebSite.Application.WebUI.WebApi.Client;

namespace CAF.WebSite.Application.WebUI.Security
{
    public class SSOAuthHelper
    {
        #region 认证参数
        // private const string PublicKey = "84c47524e3eb89beb4afb9888e19bd70";
        // private const string SecretKey = "ea202663fb61bcf0fb2c4d1fd8125354";
        private static string PublicKey
        {
            get
            {
                return ConfigurationManager.AppSettings["SSOPublicKey"];
            }
        }
        private static string SecretKey
        {
            get
            {
                return ConfigurationManager.AppSettings["SSOSecretKey"];
            }
        }
        private static string SiteCode
        {
            get
            {
                return ConfigurationManager.AppSettings["SiteCode"];
            }
        }
        private static string AppKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AppKey"];
            }
        }
        #endregion

        #region Server Url 地址
        /// <summary>
        /// 登录地址
        /// </summary>
        private static string GetLoginUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SSOPassport"] + "/Passport";
            }
        }
        private static string GetLogoutUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SSOPassport"] + "/odata/v1/Passport/LogOut";
            }
        }
        /// <summary>
        /// 认证地址
        /// </summary>
        private static string GetCertificateUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SSOPassport"] + "/odata/v1/Passport({0})/Certificate";
            }
        }
        /// <summary>
        /// 获取系统菜单 
        /// </summary>
        private static string GetPermissionUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SSOPassport"] + "/odata/v1/Passport({0})/Permissions";
            }
        }
        /// <summary>
        /// 获取组织架构
        /// </summary>
        private static string GetOrganizesUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SSOPassport"] + "/odata/v1/Passport({0})/Organizes";
            }
        }

        /// <summary>
        /// 获取用户行为登记
        /// </summary>
        private static string GetAccessRegisterUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SSOPassport"] + "/odata/v1/Passport/AccessRegister";
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        private static string getUpdateCertificateUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SSOPassport"] + "/odata/v1/Passport/UpdateCertificate";
            }
        }

        /// <summary>
        /// 同步功能模块
        /// </summary>
        private static string getSynRefModuleUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SSOPassport"] + "/odata/v1/Passport/UpdateRefModule";
            }
        }
        #endregion

        #region 登录登出
        public static ActionResult SsoLoginResult(string username)
        {
            return new RedirectResult(string.Format("{0}?siteCode={1}&appKey={2}&username={3}",
                   GetLoginUrl,
                   SiteCode,
                   AppKey,
                    username));
        }
        /// <summary>
        /// 安全退出
        /// </summary>
        /// <returns></returns>
        public static bool LogOut()
        {
            //IWorkContext workContext = EngineContext.Current.Resolve<IWorkContext>();
            //string url = GetLogoutUrl + "?sessionKey=" + workContext.CurrentUser.SessionKey;
            //string cerJson = CallTheApi(url, "", PublicKey, SecretKey, "GET");
            //OperationResult result = JsonConvert.DeserializeObject<OperationResult>(cerJson);
            //if (result.ResultType == OperationResultType.Success)
            //{

                return true;
            //}
            //else
            //    return false;

        }

        #endregion


        /// <summary>
        /// 用户认证
        /// </summary>
        /// <param name="token">sessionId</param>
        /// <returns></returns>
        public static UserCertificate GetCertificate(string sessionKey)
        {
            try
            {
                //如果没有token直接显示未登陆
                if (string.IsNullOrEmpty(sessionKey))
                    return null;
                UserCertificate cer = null;

                string cerJson = CallTheApi(GetCertificateUrl.FormatWith(sessionKey), "", PublicKey, SecretKey, "POST", (a, r) =>
                {
                    cer = (UserCertificate)a.TryParseEntity<UserCertificate>(r, "Passport",
                         new ExcludePropertiesContractResolver(new List<string> { "Id" }));
                });
                //UserCertificate cer = JsonConvert.DeserializeObject<UserCertificate>(cerJson);
                return cer != null ? cer : null;

            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取当前用户的功能菜单树
        /// </summary>
        /// <returns></returns>
        //public static string GetSysMenu()
        //{
        //    //return HttpCrossDomain.Post(GetSysMenuUrl, "{sessionKey:'" + AuthToken.CurrentUser.sessionKey + "'}");
        //    return "";
        //}
        /// <summary>
        /// 获取用户拥有的权限
        /// </summary>
        /// <returns></returns>
        public static List<Permission> GetPermissions(string sessionKey)
        {
            List<Permission> permission = null;
            string moduleJson = CallTheApi(GetPermissionUrl.FormatWith(sessionKey) + "?$top=120", "{\"AppKey\":\"" + AppKey + "\"}", PublicKey, SecretKey, "POST", (a, r) =>
                {
                    permission = a.TryParseEntitys<Permission>(r, "Permission");
                });

            return permission;
        }
        public static List<Organize> GetOrganizes(string sessionKey)
        {
            List<Organize> permission = null;
            string moduleJson = CallTheApi(GetPermissionUrl.FormatWith(sessionKey) + "?$top=120", "", PublicKey, SecretKey, "POST", (a, r) =>
            {
                permission = a.TryParseEntitys<Organize>(r, "Organize");
            });

            return permission;
        }
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="cer"></param>
        //public static ResultMsg UpdateCertificate(CertificateDto cer)
        //{
        //    CertificateDto dto = AuthToken.CurrentUser;
        //    string jsonData = "{sessionKey:'" + dto.sessionKey + "',"
        //                      + "Id:'" + dto.Id + "',"
        //                      + "UserName:'" + cer.UserName + "',"
        //                      + "NickName:'" + cer.NickName + "',"
        //                      + "Phone:'" + cer.Phone + "',"
        //                      + "Email:'" + cer.Email + "',"
        //                      + "QQ:'" + cer.QQ + "',"
        //                      + "Address:'" + cer.Address + "',"
        //                      + "UserPwd:'" + cer.UserPwd + "',"
        //                      + "NewPwd:'" + cer.NewPwd + "',"
        //                      + "Sex:'" + cer.Sex + "'}";
        //    string result = HttpCrossDomain.Post(getUpdateCertificateUrl, jsonData);
        //    ResultMsg msg = JsonMapper.ToObject<ResultMsg>(result);
        //    if (msg.success)
        //        GetCertificate(dto.sessionKey);
        //    return msg;
        //}

        private static string CallTheApi(string url, string content, string publicKey, string secretKey, string httpMethod, Action<ApiConsumer, WebApiConsumerResponse> action = null, bool AcceptJson = true)
        {


            var context = new WebApiRequestContext()
            {
                PublicKey = publicKey,
                SecretKey = secretKey,
                Url = url,
                HttpMethod = httpMethod,
                HttpAcceptType = (AcceptJson ? ApiConsumer.JsonAcceptType : ApiConsumer.XmlAcceptType)
            };

            if (!context.IsValid)
            {

                return "";
            }

            var apiConsumer = new ApiConsumer();
            var requestContent = new StringBuilder();
            var response = new WebApiConsumerResponse();
            var sb = new StringBuilder();



            var webRequest = apiConsumer.StartRequest(context, content, requestContent);

            var requestStr = requestContent.ToString();

            bool result = apiConsumer.ProcessResponse(webRequest, response);

            var responseStr = "Response: " + response.Status;

            sb.Append(response.Headers);

            //if (result)
            //{
            //    var customers = apiConsumer.TryParseCustomers(response);

            //    if (customers != null)
            //    {
            //        sb.AppendLine(string.Format("Parsed {0} customer(s):", customers.Count));

            //        foreach (var customer in customers)
            //            sb.AppendLine(customer.ToString());

            //        sb.Append("\r\n");
            //    }

            //}
            if (result)
            {
                if (action != null)
                    action(apiConsumer, response);
            }

            sb.Append(response.Content);
            return response.Content;
        }
        private static string CallTheApi(string url, string query, string content, string publicKey, string secretKey, string httpMethod, Action<ApiConsumer, WebApiConsumerResponse> action = null, bool AcceptJson = true)
        {

            //if (!url.EndsWith("/"))
            //{
            //    url = url + "/";
            //}
            var context = new WebApiRequestContext()
            {
                PublicKey = publicKey,
                SecretKey = secretKey,
                Url = url,
                HttpMethod = httpMethod,
                HttpAcceptType = (AcceptJson ? ApiConsumer.JsonAcceptType : ApiConsumer.XmlAcceptType)
            };
            if (!string.IsNullOrWhiteSpace(query))
                context.Url = string.Format("{0}?{1}", context.Url, query);

            if (!context.IsValid)
            {

                return "";
            }

            var apiConsumer = new ApiConsumer();
            var requestContent = new StringBuilder();
            var response = new WebApiConsumerResponse();
            var sb = new StringBuilder();



            var webRequest = apiConsumer.StartRequest(context, content, requestContent);

            var requestStr = requestContent.ToString();

            bool result = apiConsumer.ProcessResponse(webRequest, response);

            var responseStr = "Response: " + response.Status;

            sb.Append(response.Headers);

            if (result)
            {
                if (action != null)
                    action(apiConsumer, response);
            }


            sb.Append(response.Content);
            return response.Content;
        }
        private static string CallTheApi(string url, string actionPath, string query, string content, string publicKey, string secretKey, string version, string httpMethod, Action<ApiConsumer, WebApiConsumerResponse> action = null, bool AcceptJson = true)
        {


            var context = new WebApiRequestContext()
            {
                PublicKey = publicKey,
                SecretKey = secretKey,
                Url = url + "api/" + version + actionPath,
                HttpMethod = httpMethod,
                HttpAcceptType = (AcceptJson ? ApiConsumer.JsonAcceptType : ApiConsumer.XmlAcceptType)
            };

            if (!string.IsNullOrWhiteSpace(query))
                context.Url = string.Format("{0}?{1}", context.Url, query);

            if (!context.IsValid)
            {

                return "";
            }

            var apiConsumer = new ApiConsumer();
            var requestContent = new StringBuilder();
            var response = new WebApiConsumerResponse();
            var sb = new StringBuilder();



            var webRequest = apiConsumer.StartRequest(context, content, requestContent);

            var requestStr = requestContent.ToString();

            bool result = apiConsumer.ProcessResponse(webRequest, response);

            var responseStr = "Response: " + response.Status;

            sb.Append(response.Headers);

            if (result)
            {
                if (action != null)
                    action(apiConsumer, response);
            }

            sb.Append(response.Content);
            return response.Content;
        }
    }
}