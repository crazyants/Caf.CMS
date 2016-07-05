
using CAF.Infrastructure.Core.Configuration;


namespace CAF.WebSite.Application.WebUI.Captcha
{
    public class CaptchaSettings : ISettings
    {
        public bool Enabled { get; set; }
        public bool ShowOnLoginPage { get; set; }
        public bool ShowOnRegistrationPage { get; set; }
        public bool ShowOnContactUsPage { get; set; }
        public bool ShowOnEmailWishlistToFriendPage { get; set; }
        public bool ShowOnEmailProductToFriendPage { get; set; }
        public bool ShowOnAskQuestionPage { get; set; }
        public bool ShowOnArticleCommentPage { get; set; }
        public string ReCaptchaPublicKey { get; set; }
        public string ReCaptchaPrivateKey { get; set; }
    }
}