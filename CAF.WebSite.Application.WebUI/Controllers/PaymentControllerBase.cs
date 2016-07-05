using CAF.WebSite.Application.Services.Payments;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Mvc;
using CAF.Infrastructure.Core;

namespace CAF.WebSite.Application.WebUI.Controllers
{

    public abstract class PaymentControllerBase : BaseController
    {
        public abstract IList<string> ValidatePaymentForm(FormCollection form);
        public abstract ProcessPaymentRequest GetPaymentInfo(FormCollection form);

        public virtual string GetPaymentSummary(FormCollection form)
        {
            return null;
        }

        protected virtual NameValueCollection GetPaymentData()
        {
            if (Request.RequestType.IsCaseInsensitiveEqual("POST"))
            {
                return Request.Form;
            }

            var persisted = Session["PaymentData"] as NameValueCollection;

            return persisted ?? Request.Form;
        }
    }
}
