using CAF.Infrastructure.Core;
using CAF.Mvc.JQuery.Datatables;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Authentication;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.WebUI.Captcha;
using CAF.WebSite.Application.WebUI.MvcCaptcha;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Mvc.Admin.Models.Channels;
using CAF.WebSite.Mvc.Admin.Models.Users;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.WebSite.Application.Services.Media;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Data;
using CAF.WebSite.Application.Services.Seo;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Sites;

using System.Text;
using CAF.WebSite.Application.Services.Common;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using System.Globalization;
using CAF.WebSite.Application.Services.Channels;
using CAF.WebSite.Mvc.Admin.Models.Common;
using CAF.WebSite.Application.Services.Searchs;
using CAF.Infrastructure.Core.Domain;

namespace CAF.WebSite.Mvc.Admin.Controllers
{
    public class ArticleController : AdminControllerBase
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly IModelTemplateService _modelTemplateService;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly UserSettings _userSettings;
        private readonly IUserService _userService;
        private readonly IUserContentService _userContentService;
        private readonly IArticleService _articleService;
        private readonly IArticleTagService _articleTagService;
        private readonly IArticleCategoryService _categoryService;
        private readonly IUserActivityService _userActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPermissionService _permissionService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ISiteService _siteService;
        private readonly ISiteMappingService _siteMappingService;
        private readonly IChannelService _channelService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IArticleAttributeService _articleAttributeService;
        private readonly IExtendedAttributeService _extendedAttributeService;
        private readonly IExtendedAttributeParser _extendedAttributeParser;
        private readonly ArticleCatalogSettings _catalogSettings;
        private readonly IDownloadService _downloadService;
        private readonly IAclService _aclService;
        private readonly IDbContext _dbContext;
        private readonly SiteInformationSettings _siteSettings;

        #endregion

        #region Ctor

        public ArticleController(
            IWorkContext workContext,
            ISiteContext siteContext,
            IModelTemplateService modelTemplateService,
            IArticleCategoryService categoryService,
            ILanguageService languageService,
            IPictureService pictureService,
            AdminAreaSettings adminAreaSettings,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            UserSettings userSettings,
            IUserService userService,
            IUserContentService userContentService,
            IArticleService articleService,
            IArticleTagService articleTagService,
            IUserActivityService userActivityService,
            IEventPublisher eventPublisher,
            IPermissionService permissionService,
            IUrlRecordService urlRecordService,
            ISiteService siteService,
            IChannelService channelService,
            ISiteMappingService siteMappingService,
            IArticleAttributeService articleAttributeService,
            IExtendedAttributeService extendedAttributeService,
            IExtendedAttributeParser extendedAttributeParser,
            IDateTimeHelper dateTimeHelper,
            ArticleCatalogSettings catalogSettings,
            IDownloadService downloadService,
            IAclService aclService,
            IDbContext dbContext,
            SiteInformationSettings siteSettings
             )
        {
            this._pictureService = pictureService;
            this._modelTemplateService = modelTemplateService;
            this._workContext = workContext;
            this._siteContext = siteContext;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._userSettings = userSettings;
            this._userService = userService;
            this._userContentService = userContentService;
            this._articleService = articleService;
            this._articleTagService = articleTagService;
            this._categoryService = categoryService;
            this._userActivityService = userActivityService;
            this._adminAreaSettings = adminAreaSettings;
            this._eventPublisher = eventPublisher;
            this._permissionService = permissionService;
            this._dateTimeHelper = dateTimeHelper;
            this._urlRecordService = urlRecordService;
            this._siteService = siteService;
            this._siteMappingService = siteMappingService;
            this._articleAttributeService = articleAttributeService;
            this._extendedAttributeService = extendedAttributeService;
            this._extendedAttributeParser = extendedAttributeParser;
            this._aclService = aclService;
            this._dbContext = dbContext;
            this._downloadService = downloadService;
            this._catalogSettings = catalogSettings;
            this._channelService = channelService;
            this._siteSettings = siteSettings;
        }
        #endregion

        #region Update[...]

        [NonAction]
        protected void UpdateArticleGeneralInfo(Article article, ArticleModel model)
        {
            var a = article;
            var m = model;
            a.IsHot = m.IsHot;
            a.IsPasswordProtected = m.IsPasswordProtected;
            a.IsRed = m.IsRed;
            a.IsSlide = m.IsSlide;
            a.IsSys = m.IsSys;
            a.IsTop = m.IsTop;
            a.AllowComments = m.AllowComments;
            a.CategoryId = m.CategoryId;
            a.Click = m.Click; ;
            a.ImgUrl = m.ImgUrl;
            if (m.PictureId != 0)
                a.PictureId = m.PictureId;
            a.LinkUrl = m.LinkUrl;
            a.Password = m.Password;
            a.StatusId = m.StatusId;
            a.Author = m.Author;
            a.ModelTemplateId = m.ModelTemplateId;
            a.Title = m.Title;
            a.ShortContent = m.ShortContent;
            a.FullContent = m.FullContent;
            a.StartDateUtc = m.StartDate;
            a.EndDateUtc = m.EndDate;
            a.DisplayOrder = m.DisplayOrder;
            a.AllowUserReviews = m.AllowUserReviews;
            a.IsDownload = m.IsDownload;
            a.DownloadId = m.DownloadId;
            a.UnlimitedDownloads = m.UnlimitedDownloads;
            a.MaxNumberOfDownloads = m.MaxNumberOfDownloads;

            a.ModifiedOnUtc = DateTime.UtcNow;
            //网站限制
            if (this._siteSettings.SiteContentShare)
            {
                a.LimitedToSites = m.LimitedToSites;
            }
            else
            {
                a.LimitedToSites = true;
                var siteIds = new List<int>();
                siteIds.Add(this._siteContext.CurrentSite.Id);
                m.SelectedSiteIds = siteIds.ToArray();
            }
            //权限
            a.SubjectToAcl = m.SubjectToAcl;
            //SEo
            a.MetaKeywords = m.MetaKeywords;
            a.MetaDescription = m.MetaDescription;
            a.MetaTitle = m.MetaTitle;

        }

        [NonAction]
        protected void UpdateArticleTags(Article article, string rawArticleTags)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            var articleTags = new List<string>();

            foreach (string str in rawArticleTags.SplitSafe(","))
            {
                string tag = str.TrimSafe();
                if (tag.HasValue())
                    articleTags.Add(tag);
            }

            var existingArticleTags = article.ArticleTags.ToList();
            var articleTagsToRemove = new List<ArticleTag>();

            foreach (var existingArticleTag in existingArticleTags)
            {
                bool found = false;
                foreach (string newArticleTag in articleTags)
                {
                    if (existingArticleTag.Name.Equals(newArticleTag, StringComparison.InvariantCultureIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    articleTagsToRemove.Add(existingArticleTag);
                }
            }

            foreach (var articleTag in articleTagsToRemove)
            {
                article.ArticleTags.Remove(articleTag);
                _articleService.UpdateArticle(article);
            }

            foreach (string articleTagName in articleTags)
            {
                ArticleTag articleTag = null;
                var articleTag2 = _articleTagService.GetArticleTagByName(articleTagName);

                if (articleTag2 == null)
                {
                    //add new article tag
                    articleTag = new ArticleTag()
                    {
                        Name = articleTagName
                    };
                    _articleTagService.InsertArticleTag(articleTag);
                }
                else
                {
                    articleTag = articleTag2;
                }

                if (!article.ArticleTagExists(articleTag.Id))
                {
                    article.ArticleTags.Add(articleTag);
                    _articleService.UpdateArticle(article);
                }
            }
        }

        [NonAction]
        protected void UpdateArticleAcl(Article article, ArticleModel model)
        {


            var existingAclRecords = _aclService.GetAclRecords(article);
            var allUserRoles = _userService.GetAllUserRoles(true);
            foreach (var userRole in allUserRoles)
            {
                if (model.SelectedUserRoleIds != null && model.SelectedUserRoleIds.Contains(userRole.Id))
                {
                    //new role
                    if (existingAclRecords.Where(acl => acl.UserRoleId == userRole.Id).Count() == 0)
                        _aclService.InsertAclRecord(article, userRole.Id);
                }
                else
                {
                    //removed role
                    var aclRecordToDelete = existingAclRecords.Where(acl => acl.UserRoleId == userRole.Id).FirstOrDefault();
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }
        [NonAction]
        protected void UpdateSiteMappings(Article article, ArticleModel model)
        {
            _siteMappingService.SaveSiteMappings<Article>(article, model.SelectedSiteIds);
        }

        [NonAction]
        protected void UpdateArticleSeo(Article article, ArticleModel model)
        {
            var a = article;
            var m = model;
            // SEO
            var service = _localizedEntityService;
            foreach (var localized in model.Locales)
            {
                service.SaveLocalizedValue(a, x => x.MetaKeywords, localized.MetaKeywords, localized.LanguageId);
                service.SaveLocalizedValue(a, x => x.MetaDescription, localized.MetaDescription, localized.LanguageId);
                service.SaveLocalizedValue(a, x => x.MetaTitle, localized.MetaTitle, localized.LanguageId);
            }
        }

        [NonAction]
        private void UpdatePictureSeoNames(Article article)
        {
            foreach (var pp in article.ArticleAlbum)
            {
                _pictureService.SetSeoFilename(pp.PictureId, _pictureService.GetPictureSeName(article.Title));
            }
        }
        [NonAction]
        private void UpdateDataOfExistingArticle(Article article, ArticleModel model, bool editMode)
        {
            var p = article;
            var m = model;

            var modifiedProperties = editMode ? _dbContext.GetModifiedProperties(p) : new Dictionary<string, object>();

            var nameChanged = modifiedProperties.ContainsKey("Name");
            var seoTabLoaded = m.LoadedTabs.Contains("SEO", StringComparer.OrdinalIgnoreCase);

            // SEO
            m.SeName = p.ValidateSeName(m.SeName, p.Title, true);
            _urlRecordService.SaveSlug(p, m.SeName, 0);

            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(article, x => x.Title, localized.Title, localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(article, x => x.ShortContent, localized.ShortContent, localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(article, x => x.FullContent, localized.FullContent, localized.LanguageId);

                // search engine name
                var localizedSeName = p.ValidateSeName(localized.SeName, localized.Title, false, localized.LanguageId);
                _urlRecordService.SaveSlug(p, localizedSeName, localized.LanguageId);
            }

            // picture seo names
            if (nameChanged)
            {
                UpdatePictureSeoNames(p);
            }

            // article tags
            UpdateArticleTags(p, m.ArticleTags);
            UpdateArticleSeo(article, model);
            UpdateSiteMappings(article, model);
            UpdateArticleAcl(article, model);
        }

        [NonAction]
        private void UpdateLocales(ArticleTag articleTag, ArticleTagModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(articleTag, x => x.Name, localized.Name, localized.LanguageId);
            }
        }

        [NonAction]
        protected void UpdateExtendedAttribute(Article article, ArticleModel model, FormCollection form)
        {
            string selectedAttributes = "";

            var category = _categoryService.GetArticleCategoryById(model.CategoryId);
            if (category == null)
            {
                return;
            }
            var extendedAttributes = category.Channel.ExtendedAttributes;
            foreach (var attribute in extendedAttributes)
            {
                string controlId = string.Format("extended_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var rblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(rblAttributes))
                            {
                                int selectedAttributeId = int.Parse(rblAttributes);
                                if (selectedAttributeId > 0)
                                    selectedAttributes = _extendedAttributeParser.AddExtendedAttribute(selectedAttributes,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                        {
                            var colorAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(colorAttributes))
                            {
                                string enteredText = colorAttributes.Trim();
                                selectedAttributes = _extendedAttributeParser.AddExtendedAttribute(selectedAttributes,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        selectedAttributes = _extendedAttributeParser.AddExtendedAttribute(selectedAttributes,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var txtAttribute = form[controlId];
                            if (!String.IsNullOrEmpty(txtAttribute))
                            {
                                string enteredText = txtAttribute.Trim();
                                selectedAttributes = _extendedAttributeParser.AddExtendedAttribute(selectedAttributes,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var date = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(date));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                selectedAttributes = _extendedAttributeParser.AddExtendedAttribute(selectedAttributes,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;

                    case AttributeControlType.VideoUpload:
                    case AttributeControlType.FileUpload:
                        {
                            //var postedFile = this.Request.Files[controlId].ToPostedFileResult();
                            //if (postedFile != null && postedFile.FileName.HasValue())
                            //{
                            //    int fileMaxSize = _catalogSettings.FileUploadMaximumSizeBytes;
                            //    if (postedFile.Size > fileMaxSize)
                            //    {
                            //        //TODO display warning
                            //        //warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), (int)(fileMaxSize / 1024)));
                            //    }
                            //    else
                            //    {
                            //        //save an uploaded file
                            //        var download = new Download
                            //        {
                            //            DownloadGuid = Guid.NewGuid(),
                            //            UseDownloadUrl = false,
                            //            DownloadUrl = "",
                            //            DownloadBinary = postedFile.Buffer,
                            //            ContentType = postedFile.ContentType,
                            //            Filename = postedFile.FileTitle,
                            //            Extension = postedFile.FileExtension,
                            //            IsNew = true
                            //        };
                            //        _downloadService.InsertDownload(download);
                            //        //save attribute
                            //        selectedAttributes = _extendedAttributeParser.AddExtendedAttribute(selectedAttributes, attribute, download.DownloadGuid.ToString());
                            //    }
                            //}
                            var txtAttribute = form[controlId];
                            if (!String.IsNullOrEmpty(txtAttribute))
                            {
                                string enteredText = txtAttribute.Trim();
                                selectedAttributes = _extendedAttributeParser.AddExtendedAttribute(selectedAttributes,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            //save extended attributes
            _articleAttributeService.SaveAttribute(article, ArticleAttributeNames.ExtendedAttributes, selectedAttributes);

        }

        #endregion

        #region Utitilies
        [NonAction]
        protected void PrepareArticleModel(ArticleModel model, Article article, bool setPredefinedValues, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (article != null)
            {

                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(article.CreatedOnUtc, DateTimeKind.Utc);
                model.UpdatedOn = article.ModifiedOnUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(article.ModifiedOnUtc.Value, DateTimeKind.Utc) : DateTime.Now;

                model.Url = Url.RouteUrl("Article", new { SeName = article.GetSeName() }, Request.Url.Scheme);
            }
            else if (model.CategoryId != 0)
            {
                var categoryItem = _categoryService.GetArticleCategoryById(model.CategoryId);
                model.ModelTemplateId = categoryItem.DetailModelTemplateId;
                if (categoryItem != null && !categoryItem.Deleted)
                    model.CategoryBreadcrumb = categoryItem.GetCategoryBreadCrumb(_categoryService);
                else
                    model.CategoryId = 0;
                model.ChannelId = categoryItem.ChannelId;
            }

            #region templates

            var templates = _modelTemplateService.GetAllModelTemplates((int)TemplateTypeFormat.Detail);
            foreach (var template in templates)
            {
                model.AvailableModelTemplates.Add(new SelectListItem()
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }

            #endregion

            #region Tag
            var allTags = _articleTagService.GetAllArticleTagNames();
            foreach (var tag in allTags)
            {
                model.AvailableArticleTags.Add(new SelectListItem() { Text = tag, Value = tag });
            }

            if (article != null)
            {
                var result = new StringBuilder();
                var tags = article.ArticleTags.ToList();
                for (int i = 0; i < article.ArticleTags.Count; i++)
                {
                    var pt = tags[i];
                    result.Append(pt.Name);
                    if (i != article.ArticleTags.Count - 1)
                        result.Append(", ");
                }
                model.ArticleTags = result.ToString();
            }
            #endregion

            model.AvailableUserRoles = _userService
                .GetAllUserRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (article != null)
                {
                    model.SelectedUserRoleIds = _aclService.GetUserRoleIdsWithAccess(article);
                }
                else
                {
                    model.SelectedUserRoleIds = new int[0];
                }
            }



        }

        [NonAction]
        private void PrerpareArticleExtendedAttributes(ArticleModel model, Article article)
        {
            if (model != null)
            {
                #region Extended attributes
                var category = _categoryService.GetArticleCategoryById(model.CategoryId);
                if (category != null)
                {

                    var extendedAttributes = category.Channel.ExtendedAttributes;

                    foreach (var attribute in extendedAttributes)
                    {
                        var caModel = new ArticleModel.ArticleExtendedAttributeModel()
                        {
                            Id = attribute.Id,
                            Name = attribute.GetLocalized(x => x.Title),
                            TextPrompt = attribute.GetLocalized(x => x.TextPrompt),
                            IsRequired = attribute.IsRequired,
                            AttributeControlType = attribute.AttributeControlType,
                            AllowedFileExtensions = _catalogSettings.FileUploadAllowedExtensions
                        };

                        if (attribute.ShouldHaveValues())
                        {
                            //values
                            var caValues = _extendedAttributeService.GetExtendedAttributeValues(attribute.Id);
                            foreach (var caValue in caValues)
                            {
                                var pvaValueModel = new ArticleModel.ArticleExtendedAttributeValueModel()
                                {
                                    Id = caValue.Id,
                                    Name = caValue.GetLocalized(x => x.Name),
                                    IsPreSelected = caValue.IsPreSelected
                                };
                                caModel.Values.Add(pvaValueModel);

                            }
                        }

                        //set already selected attributes
                        string selectedExtendedAttributes = article == null ? "" : article.GetArticleAttribute<string>(ArticleAttributeNames.ExtendedAttributes, _articleAttributeService);
                        switch (attribute.AttributeControlType)
                        {
                            case AttributeControlType.DropdownList:
                            case AttributeControlType.RadioList:
                            case AttributeControlType.Checkboxes:
                                {
                                    if (!String.IsNullOrEmpty(selectedExtendedAttributes))
                                    {
                                        //clear default selection
                                        foreach (var item in caModel.Values)
                                            item.IsPreSelected = false;

                                        //select new values
                                        var selectedCaValues = _extendedAttributeParser.ParseExtendedAttributeValues(selectedExtendedAttributes);
                                        foreach (var caValue in selectedCaValues)
                                            foreach (var item in caModel.Values)
                                                if (caValue.Id == item.Id)
                                                    item.IsPreSelected = true;
                                    }
                                }
                                break;
                            case AttributeControlType.ColorSquares:
                            case AttributeControlType.TextBox:
                            case AttributeControlType.MultilineTextbox:
                            case AttributeControlType.VideoUpload:
                            case AttributeControlType.FileUpload:
                                {
                                    if (!String.IsNullOrEmpty(selectedExtendedAttributes))
                                    {
                                        var enteredText = _extendedAttributeParser.ParseValues(selectedExtendedAttributes, attribute.Id);
                                        if (enteredText.Count > 0)
                                            caModel.DefaultValue = enteredText[0];
                                    }
                                }
                                break;
                            case AttributeControlType.Datepicker:
                                {
                                    //keep in mind my that the code below works only in the current culture
                                    var selectedDateStr = _extendedAttributeParser.ParseValues(selectedExtendedAttributes, attribute.Id);
                                    if (selectedDateStr.Count > 0)
                                    {
                                        DateTime selectedDate;
                                        if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                                               DateTimeStyles.None, out selectedDate))
                                        {
                                            //successfully parsed
                                            caModel.SelectedDay = selectedDate.Day;
                                            caModel.SelectedMonth = selectedDate.Month;
                                            caModel.SelectedYear = selectedDate.Year;
                                        }
                                    }

                                }
                                break;
                            default:
                                break;
                        }

                        model.ArticleExtendedAttributes.Add(caModel);
                    }
                }

                #endregion
            }
        }

        [NonAction]
        private void PrepareArticlePictureThumbnailModel(ArticleModel model, Article article)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (article != null && _adminAreaSettings.DisplayArticlePictures)
            {
                var defaultArticlePicture = _pictureService.GetPicturesByArticleId(article.Id, 1).FirstOrDefault();
                model.PictureThumbnailUrl = _pictureService.GetPictureUrl(defaultArticlePicture, 75, true);
                model.NoThumb = defaultArticlePicture == null;
            }
        }

        [NonAction]
        private void PrepareSitesMappingModel(ArticleModel model, Article article, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableSites = _siteService
                .GetAllSites()
                .Select(s => s.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (article != null)
                {
                    model.SelectedSiteIds = _siteMappingService.GetSitesIdsWithAccess(article);
                }
                else
                {
                    model.SelectedSiteIds = new int[0];
                }
            }
            model.SiteContentShare = _siteSettings.SiteContentShare;
        }
        #endregion

        #region Methods
        #region Article list / create / edit / delete
        // GET: Edit
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }
        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var allChannels = _channelService.GetAllChannels();
            #region categories
            //categories
            var allCategories = _categoryService.GetAllCategories(showHidden: true);

            var mappedCategories = allCategories.ToDictionary(x => x.Id);
            foreach (var c in allCategories)
            {
                // c.Name = c.GetCategoryNameWithPrefix(_categoryService, mappedCategories);
                c.Name = c.GetCategoryBreadCrumb(_categoryService, mappedCategories);
            }

            var menuModels = new List<MenuModel>();
            foreach (var item in allChannels)
            {

                var menuModel = new MenuModel();
                menuModel.text = item.Title;
                var channelCategory = allCategories.Where(p => p.ChannelId == item.Id).ToList();
                foreach (var category in channelCategory)
                {
                    var menuItemModel = new MenuItemModel();
                    menuItemModel.text = category.Name;
                    menuItemModel.id = category.Id.ToString();
                    menuItemModel.href = @Url.Action("Center", new { SearchCategoryId = category.Id });
                    menuModel.items.Add(menuItemModel);
                }
                menuModels.Add(menuModel);

            }

            #endregion



            return View(menuModels);
        }
        public ActionResult Center(ArticleListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            #region templates
            var templates = _modelTemplateService.GetAllModelTemplates();
            foreach (var template in templates)
            {
                model.AvailableModelTemplates.Add(new SelectListItem()
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }

            #endregion
            #region categories
            //categories
            var allCategories = _categoryService.GetAllCategories(showHidden: true);

            var mappedCategories = allCategories.ToDictionary(x => x.Id);
            foreach (var c in allCategories)
            {
                model.AvailableCategories.Add(new SelectListItem { Text = c.GetCategoryNameWithPrefix(_categoryService, mappedCategories), Value = c.Id.ToString() });
            }
            allCategories.Insert(0, new ArticleCategory() { Id = 0, Name = "所有" });
            #endregion

            return View(model);
        }
        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam, ArticleListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();


            var searchContext = new ArticleSearchContext
            {
                SiteId = model.SearchSiteId,
                Keywords = model.SearchArticleName,
                LanguageId = _workContext.WorkingLanguage.Id,
                OrderBy = ArticleSortingEnum.Position,
                PageIndex = dataTableParam.PageIndex,
                PageSize = dataTableParam.PageSize,
                ShowHidden = true,
                WithoutCategories = model.SearchWithoutCategories
            };

            if (model.SearchCategoryId > 0)
                searchContext.CategoryIds.Add(model.SearchCategoryId);
            var categories = _categoryService.GetAllArticleCategoriesByParentCategoryId(model.SearchCategoryId);
            if (categories != null && categories.Any())
            {
                var articlesID = categories.CategoriesForTree();
                searchContext.CategoryIds.AddRange(articlesID);
            }
            var articles = _articleService.SearchArticles(searchContext);

            var total = articles.TotalCount;
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = articles.Select(x =>
                {
                    var m = x.ToModel();
                    //获取图片
                    //  PrepareArticlePictureThumbnailModel(m, x);
                    m.StatusName = x.GetArticleStatusLabel(_localizationService);
                    if (x.ModifiedOnUtc.HasValue)
                        m.UpdatedOn = _dateTimeHelper.ConvertToUserTime(x.ModifiedOnUtc.Value, DateTimeKind.Utc);
                    if (x.StartDateUtc.HasValue)
                        m.StartDate = _dateTimeHelper.ConvertToUserTime(x.StartDateUtc.Value, DateTimeKind.Utc);
                    if (x.EndDateUtc.HasValue)
                        m.EndDate = _dateTimeHelper.ConvertToUserTime(x.EndDateUtc.Value, DateTimeKind.Utc);
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    //m.LanguageName = x.Language.Name;
                    m.Comments = x.ApprovedCommentCount + x.NotApprovedCommentCount;
                    m.CategoryBreadcrumb = _categoryService.GetArticleCategoryById(x.CategoryId).GetCategoryBreadCrumb(_categoryService);
                    return m;
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };

        }

        public ActionResult Create(int? categoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var model = new ArticleModel();
            if (categoryId.HasValue)
            {
                model.CategoryId = categoryId.Value;

            }
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            PrerpareArticleExtendedAttributes(model, null);
            PrepareArticleModel(model, null, false, false);
            PrepareSitesMappingModel(model, null, false);
            //article
            AddLocales(_languageService, model.Locales);
            model.SiteContentShare = _siteSettings.SiteContentShare;
            model.DisplayOrder = 1;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing", "save-create", "continueCreate")]
        [ValidateInput(false)]
        public ActionResult Create(string btnId, string formId, ArticleModel model, bool continueEditing, bool continueCreate, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            if (ModelState.IsValid)
            {
                var article = model.ToEntity();
                article.AddEntitySysParam();
                MapModelToArticle(model, article, form);
                UpdateExtendedAttribute(article, model, form);
                _articleService.InsertArticle(article);
                UpdateDataOfExistingArticle(article, model, false);

                //搜索引擎添加索引
                this._eventPublisher.Publish(new ArticleEvent(article, Url.RouteUrl("Article", new { SeName = article.GetSeName() }, Request.Url.Scheme)));

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Articles.Added"));
                if (continueCreate)
                {
                    return RedirectToAction("Create", new { categoryId = model.CategoryId });
                }

                // ensure that the same tab gets selected in edit view
                var selectedTab = TempData["SelectedTab.article-edit"] as CAF.WebSite.Application.WebUI.SelectedTabInfo;
                if (selectedTab != null)
                {
                    selectedTab.Path = Url.Action("Edit", new System.Web.Routing.RouteValueDictionary { { "id", article.Id } });
                }
                return continueEditing ? RedirectToAction("Edit", new { id = article.Id }) : RedirectToAction("Center", new { SearchCategoryId = model.CategoryId });


            }


            PrerpareArticleExtendedAttributes(model, null);
            PrepareArticleModel(model, null, false, false);
            PrepareSitesMappingModel(model, null, false);
            return View(model);
        }

        //edit article
        public ActionResult Edit(int id, string btnId, string formId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var article = _articleService.GetArticleById(id);

            if (article == null || article.Deleted)
                return RedirectToAction("Center");

            var model = article.ToModel();


            // codehint: sm-edit
            var parentCategory = _categoryService.GetArticleCategoryById(model.CategoryId);
            if (parentCategory != null && !parentCategory.Deleted)
                model.CategoryBreadcrumb = parentCategory.GetCategoryBreadCrumb(_categoryService);
            else
                model.CategoryId = 0;

            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Title = article.GetLocalized(x => x.Title, languageId, false, false);
                locale.ShortContent = article.GetLocalized(x => x.ShortContent, languageId, false, false);
                locale.FullContent = article.GetLocalized(x => x.FullContent, languageId, false, false);
                locale.MetaKeywords = article.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = article.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = article.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = article.GetSeName(languageId, false, false);
                //locale.BundleTitleText = article.GetLocalized(x => x.BundleTitleText, languageId, false, false);
            });
      
            PrerpareArticleExtendedAttributes(model, article);
            PrepareArticlePictureThumbnailModel(model, article);
            PrepareArticleModel(model, article, false, false);
            PrepareSitesMappingModel(model, article, false);
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing", "save-create", "continueCreate")]
        [ValidateInput(false)]
        public ActionResult Edit(string btnId, string formId, ArticleModel model, bool continueEditing, bool continueCreate, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var article = _articleService.GetArticleById(model.Id);
            if (article == null || article.Deleted)
            {
                return RedirectToAction("Center", new { categoryId = model.CategoryId });
            }

            if (ModelState.IsValid)
            {
                MapModelToArticle(model, article, form);
                article.AddEntitySysParam(false, true);
                UpdateExtendedAttribute(article, model, form);
                _articleService.UpdateArticle(article);
                UpdateDataOfExistingArticle(article, model, false);

                // activity log
                _userActivityService.InsertActivity("EditArticle", _localizationService.GetResource("ActivityLog.EditArticle"), article.Title);
                //搜索引擎添加索引
                this._eventPublisher.Publish(new ArticleEvent(article, Url.RouteUrl("Article", new { SeName = article.GetSeName() }, Request.Url.Scheme)));

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Articles.Updated"));
                if (continueCreate)
                {
                    return RedirectToAction("Create", new { btnId = btnId, formId = formId });
                }
                return continueEditing ? RedirectToAction("Edit", new { id = article.Id }) : RedirectToAction("Center", new { SearchCategoryId = model.CategoryId });

            }


            PrerpareArticleExtendedAttributes(model, article);
            PrepareArticleModel(model, article, false, true);
            PrepareArticlePictureThumbnailModel(model, article);
            PrepareSitesMappingModel(model, article, false);

            return View(model);
        }

        [NonAction]
        protected void MapModelToArticle(ArticleModel model, Article article, FormCollection form)
        {
            if (model.LoadedTabs == null || model.LoadedTabs.Length == 0)
            {
                model.LoadedTabs = new string[] { "Info" };
            }

            foreach (var tab in model.LoadedTabs)
            {
                switch (tab.ToLower())
                {
                    case "info":
                        UpdateArticleGeneralInfo(article, model);
                        break;
                    case "seo":

                        break;
                    case "acl":

                        break;
                    case "sites":

                        break;
                    case "exts":
                        UpdateExtendedAttribute(article, model, form);
                        break;
                }
            }

            _eventPublisher.Publish(new ModelBoundEvent(model, article, form));
        }

        public ActionResult LoadEditTab(int id, string tabName, string viewPath = null)
        {

            try
            {
                if (id == 0)
                {
                    // is Create mode
                    return PartialView("_Create.SaveFirst");
                }

                if (tabName.IsEmpty())
                {
                    return Content("A unique tab name has to specified (route parameter: tabName)");
                }

                var article = _articleService.GetArticleById(id);

                var model = article.ToModel();


                AddLocales(_languageService, model.Locales, (locale, languageId) =>
                {
                    locale.Title = article.GetLocalized(x => x.Title, languageId, false, false);
                    locale.ShortContent = article.GetLocalized(x => x.ShortContent, languageId, false, false);
                    locale.FullContent = article.GetLocalized(x => x.FullContent, languageId, false, false);
                    locale.MetaKeywords = article.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = article.GetLocalized(x => x.MetaDescription, languageId, false, false);
                    locale.MetaTitle = article.GetLocalized(x => x.MetaTitle, languageId, false, false);
                    locale.SeName = article.GetSeName(languageId, false, false);
                    // locale.BundleTitleText = article.GetLocalized(x => x.BundleTitleText, languageId, false, false);
                });
                PrepareArticlePictureThumbnailModel(model, article);
                PrepareArticleModel(model, article, false, false);
                PrepareSitesMappingModel(model, article, false);

                return PartialView(viewPath.NullEmpty() ?? "_CreateOrUpdate." + tabName, model);
            }
            catch (Exception ex)
            {
                return Content("Error while loading template: " + ex.Message);
            }
        }

        public ActionResult ArticleExtendedPartialView(int id, int categoryid, string tabName, string viewPath = null)
        {

            try
            {
                if (tabName.IsEmpty())
                {
                    return Content("A unique tab name has to specified (route parameter: tabName)");
                }

                var article = _articleService.GetArticleById(id);
                var model = article == null ? new ArticleModel() : article.ToModel();
                model.CategoryId = categoryid;
                PrerpareArticleExtendedAttributes(model, article);

                return PartialView(viewPath.NullEmpty() ?? "_CreateOrUpdate." + tabName, model);
            }
            catch (Exception ex)
            {
                return Content("Error while loading template: " + ex.Message);
            }
        }

        //delete article
        [HttpPost]
        public ActionResult Delete(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var article = _articleService.GetArticleById(id);
            _articleService.DeleteArticle(article);

            //activity log
            _userActivityService.InsertActivity("DeleteArticle", _localizationService.GetResource("ActivityLog.DeleteArticle"), article.Title);

            NotifySuccess(_localizationService.GetResource("Admin.Catalog.Articles.Deleted"));
            return RedirectToAction("Center", new { SearchCategoryId = article.CategoryId });

        }

        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var articles = new List<Article>();
            if (selectedIds != null)
            {
                articles.AddRange(_articleService.GetArticlesByIds(selectedIds.ToArray()));

                for (int i = 0; i < articles.Count; i++)
                {
                    var article = articles[i];
                    _articleService.DeleteArticle(article);
                }
            }
            return Json(new { Result = true });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BulkEditSave(ArticleListModel.BatchCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();


            if (model.OpenCategorieCheckBox && model.CategoryId.HasValue)
            {
                foreach (var id in model.SelectedIds)
                {
                    //update
                    var article = _articleService.GetArticleById(id);
                    if (article != null)
                    {
                        article.CategoryId = model.CategoryId.Value;
                        _articleService.UpdateArticle(article);
                    }
                }
            }
            if (model.OpenTemplateCheckBox && model.TemplateId.HasValue)
            {
                foreach (var id in model.SelectedIds)
                {
                    //update
                    var article = _articleService.GetArticleById(id);
                    if (article != null)
                    {
                        article.ModelTemplateId = model.TemplateId.Value;
                        _articleService.UpdateArticle(article);
                    }
                }
            }
            if (model.OpenCheckBox)
            {
                foreach (var id in model.SelectedIds)
                {
                    //update
                    var article = _articleService.GetArticleById(id);
                    if (article != null)
                    {
                        article.AllowComments = model.AllowComments;
                        article.IsTop = model.IsTop;
                        article.IsRed = model.IsRed;
                        article.IsHot = model.IsHot;
                        article.IsSlide = model.IsSlide;
                        _articleService.UpdateArticle(article);
                    }
                }
            }
            return Json(new { state = 1 }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]

        public ActionResult BulkEditOrderSave(List<String> updatedArticles)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();


            if (updatedArticles != null)
            {
                foreach (var aModel in updatedArticles)
                {
                    //update
                    var values = aModel.Split('|');
                    var article = _articleService.GetArticleById(values[0].ToInt());
                    if (article != null)
                    {
                        article.DisplayOrder = values[1].ToInt();
                        _articleService.UpdateArticle(article);
                    }
                }
            }

            return Json(new { state = 1 }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Article pictures

        public ActionResult ArticlePictureAdd(int pictureId, int displayOrder, int articleId)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            if (pictureId == 0)
                throw new ArgumentException();

            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            _articleService.InsertArticleAlbum(new ArticleAlbum()
            {
                PictureId = pictureId,
                ArticleId = articleId,
                DisplayOrder = displayOrder,
            });

            _pictureService.SetSeoFilename(pictureId, _pictureService.GetPictureSeName(article.Title));

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ArticlePictureList(DataTablesParam dataTableParam, int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var articlePictures = _articleService.GetArticleAlbumsByArticleId(Id);
            var articlePicturesModel = articlePictures
                .Select(x =>
                {
                    return new ArticleModel.ArticlePictureModel()
                    {
                        Id = x.Id,
                        ArticleId = x.ArticleId,
                        PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                        PictureId = x.PictureId,
                        DisplayOrder = x.DisplayOrder
                    };
                })
                .ToList().AsQueryable();
            var total = articlePicturesModel.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = articlePicturesModel.Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };

        }


        public ActionResult ArticlePictureUpdate(int id, string name, string value)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var articlePicture = _articleService.GetArticleAlbumById(id);
            if (articlePicture == null)
                throw new ArgumentException("No article picture found with the specified id");

            articlePicture.DisplayOrder = value.ToInt();
            _articleService.UpdateArticleAlbum(articlePicture);
            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            // return ArticlePictureList(articlePicture.ArticleId);
        }


        public ActionResult ArticlePictureDelete(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var articlePicture = _articleService.GetArticleAlbumById(id);
            if (articlePicture == null)
                throw new ArgumentException("No article picture found with the specified id");

            var articleId = articlePicture.ArticleId;
            _articleService.DeleteArticleAlbum(articlePicture);

            var picture = _pictureService.GetPictureById(articlePicture.PictureId);
            _pictureService.DeletePicture(picture);
            return Json(new { Result = true, Message = _localizationService.GetResource("Admin.Article.ArticlePictureDelete") }, JsonRequestBehavior.AllowGet);
            // return ArticlePictureList(articleId);
        }

        #endregion

        #region Article tags

        public ActionResult ArticleTags()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult ArticleTags(DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var tags = _articleTagService.GetAllArticleTags()
                //order by Article count
                .OrderByDescending(x => _articleTagService.GetArticleCount(x.Id, 0))
                .Select(x =>
                {
                    return new ArticleTagModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ArticleCount = _articleTagService.GetArticleCount(x.Id, 0)
                    };
                });

            var total = tags.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = tags.PagedForCommand(dataTableParam.PageIndex, dataTableParam.PageSize).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }


        public ActionResult ArticleTagDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var tag = _articleTagService.GetArticleTagById(id);
            if (tag == null)
                throw new ArgumentException("No Article tag found with the specified id");
            _articleTagService.DeleteArticleTag(tag);

            return Json(new { Result = true, Message = _localizationService.GetResource("Admin.Article.ArticleTagDelete") }, JsonRequestBehavior.AllowGet);
        }
        //create
        public ActionResult CreateArticleTag()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            var model = new ArticleTagModel();

            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateArticleTag(string btnId, string formId, ArticleTagModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var articleTag = new ArticleTag();
                articleTag.Name = model.Name;
                _articleTagService.InsertArticleTag(articleTag);
                //locales
                UpdateLocales(articleTag, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }
            //If we got this far, something failed, redisplay form
            return View(model);
        }
        //edit
        public ActionResult EditArticleTag(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var ArticleTag = _articleTagService.GetArticleTagById(id);
            if (ArticleTag == null)
                //No Article tag found with the specified id
                return RedirectToAction("List");

            var model = new ArticleTagModel()
            {
                Id = ArticleTag.Id,
                Name = ArticleTag.Name,
                ArticleCount = _articleTagService.GetArticleCount(ArticleTag.Id, 0)
            };
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = ArticleTag.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public ActionResult EditArticleTag(string btnId, string formId, ArticleTagModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var ArticleTag = _articleTagService.GetArticleTagById(model.Id);
            if (ArticleTag == null)
                //No Article tag found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                ArticleTag.Name = model.Name;
                _articleTagService.UpdateArticleTag(ArticleTag);
                //locales
                UpdateLocales(ArticleTag, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Related articles

        [HttpPost]
        public ActionResult RelatedArticleList(DataTablesParam dataTableParam, int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var relatedArticles = _articleService.GetRelatedArticlesByArticleId1(articleId, true);
            var relatedArticlesModel = relatedArticles
                .Select(x =>
                {
                    var article2 = _articleService.GetArticleById(x.ArticleId2);

                    return new ArticleModel.RelatedArticleModel()
                    {
                        Id = x.Id,
                        ArticleId2 = x.ArticleId2,
                        Article2Name = article2.Title,
                        DisplayOrder = x.DisplayOrder,
                        Article2Published = article2.StatusFormat == StatusFormat.Norma
                    };
                })
                .ToList();
            var total = relatedArticlesModel.Count;
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = relatedArticlesModel.Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }


        public ActionResult RelatedArticleUpdate(ArticleModel.RelatedArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var relatedArticle = _articleService.GetRelatedArticleById(model.Id);
            if (relatedArticle == null)
                throw new ArgumentException("No related article found with the specified id");

            relatedArticle.DisplayOrder = model.DisplayOrder;
            _articleService.UpdateRelatedArticle(relatedArticle);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RelatedArticleDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var relatedArticle = _articleService.GetRelatedArticleById(id);
            if (relatedArticle == null)
                throw new ArgumentException("No related article found with the specified id");

            var articleId = relatedArticle.ArticleId1;
            _articleService.DeleteRelatedArticle(relatedArticle);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RelatedArticleAddPopup(int articleId, string btnId, string formId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var ctx = new ArticleSearchContext();
            ctx.LanguageId = _workContext.WorkingLanguage.Id;
            ctx.OrderBy = ArticleSortingEnum.Position;
            ctx.PageSize = _adminAreaSettings.GridPageSize;
            ctx.ShowHidden = true;

            var articles = _articleService.SearchArticles(ctx);

            var model = new ArticleModel.AddRelatedArticleModel();
            model.ArticleId = articleId;
            //categories
            var allCategories = _categoryService.GetAllCategories(showHidden: true);
            var mappedCategories = allCategories.ToDictionary(x => x.Id);
            foreach (var c in allCategories)
            {
                model.AvailableCategories.Add(new SelectListItem() { Text = c.GetCategoryNameWithPrefix(_categoryService, mappedCategories), Value = c.Id.ToString() });
            }

            //sites
            model.AvailableSites.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _siteService.GetAllSites())
                model.AvailableSites.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        [HttpPost]
        public ActionResult RelatedArticleAddPopupList(DataTablesParam dataTableParam, ArticleModel.AddRelatedArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            var ctx = new ArticleSearchContext();

            if (model.SearchCategoryId > 0)
                ctx.CategoryIds.Add(model.SearchCategoryId);

            ctx.SiteId = model.SearchSiteId;
            ctx.Keywords = model.SearchArticleName;
            ctx.LanguageId = _workContext.WorkingLanguage.Id;
            ctx.OrderBy = ArticleSortingEnum.Position;
            ctx.PageIndex = dataTableParam.PageIndex;
            ctx.PageSize = dataTableParam.PageSize;
            ctx.ShowHidden = true;

            var articles = _articleService.SearchArticles(ctx);

            var total = articles.Count;
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = articles.Select(x =>
                {
                    var articleModel = x.ToModel();
                    return articleModel;
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        [HttpPost]
        public ActionResult RelatedArticleAddPopup(string btnId, string formId, ArticleModel.AddRelatedArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (model.SelectedArticleIds != null)
            {
                foreach (int id in model.SelectedArticleIds)
                {
                    var article = _articleService.GetArticleById(id);
                    if (article != null)
                    {
                        var existingRelatedArticles = _articleService.GetRelatedArticlesByArticleId1(model.ArticleId);
                        if (existingRelatedArticles.FindRelatedArticle(model.ArticleId, id) == null)
                        {
                            _articleService.InsertRelatedArticle(
                                new RelatedArticle()
                                {
                                    ArticleId1 = model.ArticleId,
                                    ArticleId2 = id,
                                    DisplayOrder = 1
                                });
                        }
                    }
                }
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateAllMutuallyRelatedArticles(int articleId)
        {
            string message = null;

            if (_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
            {
                var article = _articleService.GetArticleById(articleId);
                if (article != null)
                {
                    int count = _articleService.EnsureMutuallyRelatedArticles(articleId);
                    message = T("Admin.Common.CreateMutuallyAssociationsResult", count);
                }
                else
                {
                    message = "No article found with the specified id";
                }
            }
            else
            {
                message = T("Admin.AccessDenied.Title");
            }

            return new JsonResult
            {
                Data = new { Message = message }
            };
        }

        #endregion

        #endregion

        #region Comments

        public ActionResult Comments(int? filterByArticleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            ViewBag.FilterByArticleId = filterByArticleId;

            return View();
        }

        [HttpPost]
        public ActionResult Comments(int? filterByArticleId, DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            IList<ArticleComment> comments;
            if (filterByArticleId.HasValue)
            {
                //filter comments by article
                var articlePost = _articleService.GetArticleById(filterByArticleId.Value);
                comments = articlePost.ArticleComments.OrderBy(bc => bc.CreatedOnUtc).ToList();
            }
            else
            {
                //load all article comments
                comments = _userContentService.GetAllUserContent<ArticleComment>(0, null);
            }

            var total = comments.Count;
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = comments.PagedForCommand(dataTableParam.PageIndex, dataTableParam.PageSize).Select(articleComment =>
                {
                    var commentModel = new ArticleCommentModel();
                    var user = _userService.GetUserById(articleComment.UserId);

                    commentModel.Id = articleComment.Id;
                    commentModel.ArticleId = articleComment.ArticleId;
                    commentModel.ArticleTitle = articleComment.Article.Title;
                    commentModel.UserId = articleComment.UserId;
                    commentModel.IpAddress = articleComment.IpAddress;
                    commentModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(articleComment.CreatedOnUtc, DateTimeKind.Utc);
                    commentModel.Comment = CAF.Infrastructure.Core.Html.HtmlUtils.FormatText(articleComment.CommentText, false, true, false, false, false, false);

                    if (user == null)
                        commentModel.UserName = "".NaIfEmpty();
                    else
                        commentModel.UserName = user.GetFullName();

                    return commentModel;
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        public ActionResult CommentDelete(int? filterByArticleId, int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var comment = _userContentService.GetUserContentById(id) as ArticleComment;
            if (comment == null)
                throw new ArgumentException("No comment found with the specified id");

            var articlePost = comment.Article;
            _userContentService.DeleteUserContent(comment);
            //update totals
            _articleService.UpdateCommentTotals(articlePost);

            return Json(new { Result = true });
        }


        #endregion


    }
}