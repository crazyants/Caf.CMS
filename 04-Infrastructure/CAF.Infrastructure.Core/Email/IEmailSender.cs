using System;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Core.Email
{    
    /// <summary>
    /// contract for email sender 
    /// </summary>
    public interface IEmailSender
    {
        void SendEmail(SmtpContext smtpContext, EmailMessage message);
		Task SendEmailAsync(SmtpContext smtpContext, EmailMessage message); 
    }
}
