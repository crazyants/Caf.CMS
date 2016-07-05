using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Html;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Media;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Application.Services.Localization;
using System;
using System.Text;
using System.Web;

namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Checkout attribute helper
    /// </summary>
    public partial class ExtendedAttributeFormatter : IExtendedAttributeFormatter
    {
        private readonly IWorkContext _workContext;
        private readonly IExtendedAttributeService _ExtendedAttributeService;
        private readonly IExtendedAttributeParser _ExtendedAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly IWebHelper _webHelper;

        public ExtendedAttributeFormatter(IWorkContext workContext,
            IExtendedAttributeService ExtendedAttributeService,
            IExtendedAttributeParser ExtendedAttributeParser,
            IDownloadService downloadService,
            IWebHelper webHelper)
        {
            this._workContext = workContext;
            this._ExtendedAttributeService = ExtendedAttributeService;
            this._ExtendedAttributeParser = ExtendedAttributeParser;
            this._downloadService = downloadService;
            this._webHelper = webHelper;
        }


        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="article">User</param>
        /// <param name="serapator">Serapator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <param name="allowHyperlinks">A value indicating whether to HTML hyperink tags could be rendered (if required)</param>
        /// <returns>Attributes</returns>
        public string FormatAttributes(string attributes,
            Article article,
            string serapator = "<br />",
            bool htmlEncode = true,
            bool renderPrices = true,
            bool allowHyperlinks = true)
        {
            var result = new StringBuilder();

            var caCollection = _ExtendedAttributeParser.ParseExtendedAttributes(attributes);

            if (caCollection.Count <= 0)
            {
                return null;
            }
            for (int i = 0; i < caCollection.Count; i++)
            {
                var ca = caCollection[i];
                var valuesStr = _ExtendedAttributeParser.ParseValues(attributes, ca.Id);
                for (int j = 0; j < valuesStr.Count; j++)
                {
                    string valueStr = valuesStr[j];
                    string caAttribute = "";
                    if (!ca.ShouldHaveValues())
                    {
                        //no values
                        if (ca.AttributeControlType == AttributeControlType.MultilineTextbox)
                        {
                            //multiline textbox
                            var attributeName = ca.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id);
                            //encode (if required)
                            if (htmlEncode)
                                attributeName = HttpUtility.HtmlEncode(attributeName);

                            caAttribute = string.Format("{0}: {1}", attributeName,
                                HtmlUtils.FormatText(valueStr.EmptyNull().Replace(":", ""), false, true, false, false, false, false));

                            //we never encode multiline textbox input
                        }
                        else if (ca.AttributeControlType == AttributeControlType.FileUpload || ca.AttributeControlType == AttributeControlType.VideoUpload)
                        {
                            //file upload
                            Guid downloadGuid;
                            Guid.TryParse(valueStr, out downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                                string attributeText = "";
                                var fileName = string.Format("{0}{1}",
                                    download.Filename ?? download.DownloadGuid.ToString(),
                                    download.Extension);
                                //encode (if required)
                                if (htmlEncode)
                                    fileName = HttpUtility.HtmlEncode(fileName);
                                if (allowHyperlinks)
                                {
                                    //hyperlinks are allowed
                                    var downloadLink = string.Format("{0}download/getfileupload/?downloadId={1}", _webHelper.GetSiteLocation(false), download.DownloadGuid);
                                    attributeText = string.Format("<a href=\"{0}\" class=\"fileuploadattribute\">{1}</a>", downloadLink, fileName);
                                }
                                else
                                {
                                    //hyperlinks aren't allowed
                                    attributeText = fileName;
                                }
                                var attributeName = ca.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id);
                                //encode (if required)
                                if (htmlEncode)
                                    attributeName = HttpUtility.HtmlEncode(attributeName);
                                caAttribute = string.Format("{0}: {1}", attributeName, attributeText);
                            }
                        }
                        else
                        {
                            //other attributes (textbox, datepicker)
                            caAttribute = string.Format("{0}: {1}", ca.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), valueStr);
                            //encode (if required)
                            if (htmlEncode)
                                caAttribute = HttpUtility.HtmlEncode(caAttribute);
                        }
                    }
                    else
                    {
                        int caId = 0;
                        if (int.TryParse(valueStr, out caId))
                        {
                            var caValue = _ExtendedAttributeService.GetExtendedAttributeValueById(caId);
                            //if (caValue != null)
                            //{
                            //    caAttribute = string.Format("{0}: {1}", ca.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), caValue.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id));
                            //    if (renderPrices)
                            //    {
                            //        decimal priceAdjustmentBase = _taxService.GetExtendedAttributePrice(caValue, article);
                            //        decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                            //        if (priceAdjustmentBase > 0)
                            //        {
                            //            string priceAdjustmentStr = _priceFormatter.FormatPrice(priceAdjustment);
                            //            caAttribute += string.Format(" [+{0}]", priceAdjustmentStr);
                            //        }
                            //    }
                            //}
                            //encode (if required)
                            if (htmlEncode)
                                caAttribute = HttpUtility.HtmlEncode(caAttribute);
                        }
                    }

                    if (!String.IsNullOrEmpty(caAttribute))
                    {
                        if (i != 0 || j != 0)
                            result.Append(serapator);
                        result.Append(caAttribute);
                    }
                }
            }

            return result.ToString();
        }

    }
}
