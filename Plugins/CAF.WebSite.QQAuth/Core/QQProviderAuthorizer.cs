//Contributor:  Nicholas Mayne

using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Authentication.External;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;


namespace CAF.WebSite.QQAuth.Core
{
    public class QQProviderAuthorizer : IOAuthProviderQQAuthorizer
    {
        #region Fields

        private readonly IExternalAuthorizer _authorizer;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly HttpContextBase _httpContext;
        private readonly ICommonServices _services;
        private const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";
        private const string AuthorizationEndpoint = "https://graph.qq.com/oauth2.0/authorize";
        private const string TokenEndpoint = "https://graph.qq.com/oauth2.0/token";
        private const string UserInfoEndpoint = "https://openmobile.qq.com/user/get_simple_userinfo";
        private const string OpenIDEndpoint = "https://graph.qq.com/oauth2.0/me";

        #endregion

        #region Ctor

        public QQProviderAuthorizer(IExternalAuthorizer authorizer,
            IOpenAuthenticationService openAuthenticationService,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            HttpContextBase httpContext,
            ICommonServices services)
        {
            this._authorizer = authorizer;
            this._openAuthenticationService = openAuthenticationService;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._httpContext = httpContext;
            this._services = services;
        }

        #endregion

        #region Utilities


        private AuthorizeState VerifyAuthentication(string returnUrl)
        {
            var settings = _services.Settings.LoadSetting<QQExternalAuthSettings>(_services.SiteContext.CurrentSite.Id);

            string accessTokenUri = TokenEndpoint +
                "?client_id=" + Uri.EscapeDataString(settings.ClientKeyIdentifier) +
                "&client_secret=" + Uri.EscapeDataString(settings.ClientSecret) +
                "&redirect_uri=" + Uri.EscapeDataString(returnUrl) +
                "&code=test" +
                "&grant_type=authorization_code";
            string oauthTokenResponse = Get_Http(accessTokenUri, 120000);
            var tokenDict = QueryStringToDict(oauthTokenResponse);
            string accessToken = null;

            if (tokenDict.ContainsKey("access_token"))
            {
                accessToken = tokenDict["access_token"];
            }
            else
            {
                throw new Exception("Authentication result does not contain accesstoken data");
            }
            if (!accessToken.IsEmail())
            {
                string openIDUri = OpenIDEndpoint + "?access_token=" + Uri.EscapeDataString(accessToken);
                string openIDString = Get_Http(openIDUri, 120000);
                openIDString = ExtractOpenIDCallbackBody(openIDString);
                JObject openIDInfo = JObject.Parse(openIDString);

                var clientId = openIDInfo["client_id"].Value<string>();
                var openId = openIDInfo["openid"].Value<string>();

                string userInfoUri = UserInfoEndpoint +
                    "?access_token=" + Uri.EscapeDataString(accessToken) +
                    "&oauth_consumer_key=" + Uri.EscapeDataString(clientId) +
                    "&openid=" + Uri.EscapeDataString(openId);

                string userInfoString = Get_Http(userInfoUri, 120000);
                JObject userInfo = JObject.Parse(userInfoString);

                var parameters = new OAuthAuthenticationParameters(Provider.SystemName)
                {
                    ExternalIdentifier = settings.ClientKeyIdentifier,
                    OAuthToken = accessToken,
                    OAuthAccessToken = openId,
                };
                // userInfo["nickname"].Value<string>()
                if (_externalAuthenticationSettings.AutoRegisterEnabled)
                    ParseClaims(userInfo, parameters);

                var result = _authorizer.Authorize(parameters);

                return new AuthorizeState(returnUrl, result);
            }

            var state = new AuthorizeState(returnUrl, OpenAuthenticationStatus.Error);
            var error = "Unknown error";
            state.AddError(error);
            return state;
        }

        private void ParseClaims(JObject user, OAuthAuthenticationParameters parameters)
        {
            var claims = new UserClaims();
            claims.Contact = new ContactClaims();
            IDictionary<string, JToken> userAsDictionary = user;
            string name = PropertyValueIfExists("nickname", userAsDictionary);

            claims.Name = new NameClaims();

            if (!name.IsEmpty())
            {
                var nameSplit = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (nameSplit.Length >= 2)
                {
                    claims.Name.First = nameSplit[0];
                    claims.Name.Last = nameSplit[1];
                }
                else
                {
                    claims.Name.Last = nameSplit[0];
                }

            }
            parameters.AddClaim(claims);
        }
        private static string PropertyValueIfExists(string property, IDictionary<string, JToken> dictionary)
        {
            return dictionary.ContainsKey(property) ? dictionary[property].ToString() : null;
        }
        private AuthorizeState RequestAuthentication(string returnUrl)
        {
            var authUrl = GenerateServiceLoginUrl().AbsoluteUri;
            return new AuthorizeState("", OpenAuthenticationStatus.RequiresRedirect) { Result = new RedirectResult(authUrl) };
        }

        private Uri GenerateLocalCallbackUri()
        {
            string url = string.Format("{0}Plugins/CAF.WebSite.QQAuth/logincallback/", _services.WebHelper.GetSiteLocation());
            return new Uri(url);
        }

        private Uri GenerateServiceLoginUrl()
        {
            var settings = _services.Settings.LoadSetting<QQExternalAuthSettings>(_services.SiteContext.CurrentSite.Id);

            //code copied from DotNetOpenAuth.AspNet.Clients.FacebookClient file
            var builder = new UriBuilder(AuthorizationEndpoint);
            var args = new Dictionary<string, string>();
            args.Add("client_id", Uri.EscapeDataString(settings.ClientKeyIdentifier ?? string.Empty));
            args.Add("redirect_uri", Uri.EscapeDataString(GenerateLocalCallbackUri().AbsoluteUri));
            args.Add("scope", "get_user_info");
            args.Add("state", "test");
            args.Add("response_type", "code");

            AppendQueryArgs(builder, args);

            return builder.Uri;


        }

        private void AppendQueryArgs(UriBuilder builder, IEnumerable<KeyValuePair<string, string>> args)
        {
            if ((args != null) && (args.Count<KeyValuePair<string, string>>() > 0))
            {
                StringBuilder builder2 = new StringBuilder(50 + (args.Count<KeyValuePair<string, string>>() * 10));
                if (!string.IsNullOrEmpty(builder.Query))
                {
                    builder2.Append(builder.Query.Substring(1));
                    builder2.Append('&');
                }
                builder2.Append(CreateQueryString(args));
                builder.Query = builder2.ToString();
            }
        }

        private string CreateQueryString(IEnumerable<KeyValuePair<string, string>> args)
        {
            if (!args.Any<KeyValuePair<string, string>>())
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder(args.Count<KeyValuePair<string, string>>() * 10);
            foreach (KeyValuePair<string, string> pair in args)
            {
                builder.Append(pair.Key);
                builder.Append('=');
                builder.Append(pair.Value);
                builder.Append('&');
            }
            builder.Length--;
            return builder.ToString();
        }

        private string ExtractOpenIDCallbackBody(string callbackString)
        {
            int leftBracketIndex = callbackString.IndexOf('{');
            int rightBracketIndex = callbackString.IndexOf('}');
            if (leftBracketIndex >= 0 && rightBracketIndex >= 0)
            {
                return callbackString.Substring(leftBracketIndex, rightBracketIndex - leftBracketIndex + 1).Trim();
            }
            return callbackString;
        }


        /// <summary>
        /// Gets HTTP
        /// </summary>
        /// <param name="StrUrl">Url</param>
        /// <param name="Timeout">Timeout</param>
        /// <returns>Result</returns>
        public string Get_Http(string StrUrl, int Timeout)
        {
            string strResult = string.Empty;
            try
            {
                HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(StrUrl);
                myReq.Timeout = Timeout;
                HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader sr = new StreamReader(myStream, Encoding.Default);
                StringBuilder strBuilder = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    strBuilder.Append(sr.ReadLine());
                }

                strResult = strBuilder.ToString();
            }
            catch (Exception exc)
            {
                strResult = "Error: " + exc.Message;
            }
            return strResult;
        }
        private IDictionary<string, string> QueryStringToDict(string str)
        {
            var strArr = str.Split('&');
            var dict = new Dictionary<string, string>(strArr.Length);
            foreach (var s in strArr)
            {
                var equalSymbolIndex = s.IndexOf('=');
                if (equalSymbolIndex > 0 && equalSymbolIndex < s.Length - 1)
                {
                    dict.Add(
                        s.Substring(0, equalSymbolIndex),
                        s.Substring(equalSymbolIndex + 1, s.Length - equalSymbolIndex - 1));
                }
            }
            return dict;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Authorize response
        /// </summary>
        /// <param name="returnUrl">Return URL</param>
        /// <param name="verifyResponse">true - Verify response;false - request authentication;null - determine automatically</param>
        /// <returns>Authorize state</returns>
        public AuthorizeState Authorize(string returnUrl, bool? verifyResponse = null)
        {
            if (!verifyResponse.HasValue)
                throw new ArgumentException("Facebook plugin cannot automatically determine verifyResponse property");

            if (verifyResponse.Value)
            {
                return VerifyAuthentication(returnUrl);
            }
            else
            {
                return RequestAuthentication(returnUrl);
            }
        }

        #endregion
    }
}