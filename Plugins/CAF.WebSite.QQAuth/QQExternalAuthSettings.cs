

using CAF.Infrastructure.Core.Configuration;
namespace CAF.WebSite.QQAuth
{
    public class QQExternalAuthSettings : ISettings
    {
        public string ClientKeyIdentifier { get; set; }
        public string ClientSecret { get; set; }

        public string AuthorizeURL { get; set; }
    }
}
