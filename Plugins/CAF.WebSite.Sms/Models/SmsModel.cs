
using CAF.WebSite.Application.WebUI;
using System.ComponentModel.DataAnnotations;

namespace CAF.WebSite.Sms.Models
{
    public class SmsModel
    {
        [LangResourceDisplayName("Plugins.Sms.Clickatell.Fields.Enabled")]
        public bool Enabled { get; set; } 

        [LangResourceDisplayName("Plugins.Sms.Clickatell.Fields.PhoneNumber")]
        public string PhoneNumber { get; set; }

        [LangResourceDisplayName("Plugins.Sms.Clickatell.Fields.ApiId")]
        public string ApiId { get; set; }

        [LangResourceDisplayName("Plugins.Sms.Clickatell.Fields.Username")]
        public string Username { get; set; }

        [LangResourceDisplayName("Plugins.Sms.Clickatell.Fields.Password")]
		[DataType(DataType.Password)]
        public string Password { get; set; }


        [LangResourceDisplayName("Plugins.Sms.Clickatell.Fields.TestMessage")]
        public string TestMessage { get; set; }
        public string TestSmsResult { get; set; }
    }
}