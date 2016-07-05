using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web;
using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Common;


namespace CAF.WebSite.Application.Services.Common
{
    /// <summary>
    /// PDF service
    /// </summary>
    public partial class PdfService : IPdfService
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;


        private readonly PdfSettings _pdfSettings;


        #endregion

        #region Ctor

        public PdfService(ILocalizationService localizationService,
            IWorkContext workContext,
            IWebHelper webHelper,
            PdfSettings pdfSettings)
        {
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._webHelper = webHelper;
            this._pdfSettings = pdfSettings;

        }

        #endregion

        #region Utilities

        protected virtual Font GetFont()
        {
            //SmartSite.NET supports unicode characters
            //SmartSite.NET uses Free Serif font by default (~/App_Data/Pdf/OpenSans-Regular.ttf file)
            string fontPath = Path.Combine(_webHelper.MapPath("~/App_Data/Pdf/"), _pdfSettings.FontFileName);
            var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var font = new Font(baseFont, 10, Font.NORMAL);
            return font;
        }

        #endregion

        #region Methods

        #endregion


        #region PdfEvents

    

        #endregion

    }
}