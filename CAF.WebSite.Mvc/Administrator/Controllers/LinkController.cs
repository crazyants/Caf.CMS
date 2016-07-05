using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Links;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Mvc.Admin.Models.Links;
using CAF.Infrastructure.Core;
using System;
using System.Linq;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.Services.Tables;



namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class LinkController : AdminControllerBase
    {
        #region Fields

        private readonly ILinkService _linkService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISiteService _siteService;
        private readonly ISiteMappingService _siteMappingService;
        private readonly IPictureService _pictureService;
        #endregion Fields

        #region Constructors

        public LinkController(ILinkService linkService, ILanguageService languageService,
            ILocalizedEntityService localizedEntityService, ILocalizationService localizationService,
            IPermissionService permissionService, ISiteService siteService, IPictureService pictureService,
            ISiteMappingService siteMappingService)
        {
            this._pictureService = pictureService;
            this._linkService = linkService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._siteService = siteService;
            this._siteMappingService = siteMappingService;
        }

        #endregion

        #region Utilities

        [NonAction]
        private void PrepareSitesMappingModel(LinkModel model, Link newsItem, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableSites = _siteService
                .GetAllSites()
                .Select(s => s.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (newsItem != null)
                {
                    model.SelectedSiteIds = _siteMappingService.GetSitesIdsWithAccess(newsItem);
                }
                else
                {
                    model.SelectedSiteIds = new int[0];
                }
            }
        }

        #endregion

        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLinks))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLinks))
                return AccessDeniedView();
            var links = _linkService.GetAllLinks(0, 0, dataTableParam.PageIndex, dataTableParam.PageSize,false);
            var linksQueryable = links
              .Select(x =>
              {
                  var link = x.ToModel();
                  if (x.PictureId.HasValue)
                      link.PictureUrl = _pictureService.GetPictureUrl(x.PictureId.Value);
                  return link;
              })
              .ToList().AsQueryable();
            return DataTablesResult.Create(linksQueryable, dataTableParam);
        }

        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLinks))
                return AccessDeniedView();
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            var model = new LinkModel();
            //Sites
            PrepareSitesMappingModel(model, null, false);
         
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(LinkModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLinks))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {

                var link = model.ToEntity();
                link.AddEntitySysParam();
                _linkService.InsertLink(link);
                //Sites
                _siteMappingService.SaveSiteMappings<Link>(link, model.SelectedSiteIds);
                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Links.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = link.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);

            //Sites
            PrepareSitesMappingModel(model, null, true);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLinks))
                return AccessDeniedView();

            var link = _linkService.GetLinkById(id);
            if (link == null)
                //No link found with the specified id
                return RedirectToAction("List");

         
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);

            var model = link.ToModel();

            //sites
            PrepareSitesMappingModel(model, link, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(LinkModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLinks))
                return AccessDeniedView();

            var link = _linkService.GetLinkById(model.Id);
            if (link == null)
                //No link found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                link = model.ToEntity(link);
                link.AddEntitySysParam(false, true);
                _linkService.UpdateLink(link);

                //Sites
                _siteMappingService.SaveSiteMappings<Link>(link, model.SelectedSiteIds);
                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Links.Updated"));
                return continueEditing ? RedirectToAction("Edit", link.Id) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);

            //sites
            PrepareSitesMappingModel(model, link, true);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLinks))
                return AccessDeniedView();

            var link = _linkService.GetLinkById(id);
            if (link == null)
                //No link found with the specified id
                return RedirectToAction("List");

            _linkService.DeleteLink(link);

            NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Links.Deleted"));
            return RedirectToAction("List");
        }

        #endregion
    }

    //public class LinkGridRegistration : IGridRegistration
    //{

    //    public void RegisterGrids()
    //    {
    //        ColumnDefaults colDefauls = new ColumnDefaults()
    //        {
    //            EnableSorting = true
    //        };
    //        #region 日志
    //        MVCGridDefinitionTable.Add("LogGrid", new MVCGridBuilder<LogModel>(colDefauls)
    //    .WithAuthorizationType(AuthorizationType.AllowAnonymous)
    //    .WithSorting(sorting: true, defaultSortColumn: "Frequency", defaultSortDirection: SortDirection.Dsc)
    //    .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
    //    .WithAdditionalQueryOptionNames("search")
    //            //.WithRenderingMode(RenderingMode.Controller)
    //            //.WithViewPath("~/Views/MVCGrid/_Grid.cshtml")
    //    .WithFiltering(true)
    //    .WithKeyId("Id")
    //    .WithShowCheckBox(true)
    //    .AddColumns(cols =>
    //    {
    //        cols.Add("Id").WithHeaderText("")
    //           .WithValueExpression(p => p.Id.ToString())
    //           .WithVisibility(false)
    //           .WithSorting(false);
    //        cols.Add("View").WithHeaderText("信息")
    //            .WithValueExpression((p, c) => c.UrlHelper.Action("View", "Log", new { id = p.Id, area = "admin" }))
    //            .WithValueTemplate("<span class='label label-{Model.LogLevelHint}' style='margin-right: 4px'>{Model.LogLevel}</span><a href='{Value}'>{Model.ShortMessage}</a>", false)
    //            .WithHtmlEncoding(false)
    //            .WithSorting(false);
    //        cols.Add("LogLevel").WithHeaderText("日志级别")
    //         .WithVisibility(true, true)
    //         .WithSorting(false)
    //         .WithValueExpression(p => p.LogLevel);
    //        cols.Add("Frequency").WithHeaderText("频率")
    //          .WithVisibility(true, true)
    //          .WithValueExpression(p => p.Frequency.ToString());
    //        cols.Add("IpAddress").WithHeaderText("IP地址")
    //            .WithVisibility(true, true)
    //            .WithValueExpression(p => p.IpAddress);
    //        cols.Add("UserEmail").WithHeaderText("用户")
    //            .WithVisibility(true, true)
    //            .WithSorting(false)
    //            .WithValueExpression(p => p.UserEmail);
    //        cols.Add("PageUrl").WithHeaderText("页面地址")
    //            .WithValueTemplate("{Model.PageUrl}")
    //            .WithVisibility(true, true)
    //            .WithSorting(false);
    //        cols.Add("CreatedOn").WithHeaderText("创建时间")
    //           .WithVisibility(true, true)
    //            .WithValueExpression(p => p.CreatedOn.ToShortDateString());

    //    })
    //    .WithRetrieveDataMethod((context) =>
    //    {

    //        var _dateTimeHelper = EngineContext.Current.Resolve<IDateTimeHelper>();
    //        var _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
    //        var _workContext = EngineContext.Current.Resolve<IWorkContext>();

    //        var options = context.QueryOptions;
    //        int totalRecords;
    //        string globalSearch = options.GetAdditionalQueryOptionString("search");
    //        string sortColumn = options.GetSortColumnData<string>();
    //        //var items = repo.GetData(out totalRecords, globalSearch, options.GetLimitOffset(), options.GetLimitRowcount(),
    //        //    sortColumn, options.SortDirection == SortDirection.Dsc);

    //        DateTime? createdOnFromFilter = options.GetFilterString("CreatedOnFrom").ToDateTime(null);
    //        DateTime? createdOnToFilter = options.GetFilterString("CreatedOnTo").ToDateTime(null);
    //        int logLevelFilter = options.GetFilterString("LogLevelId").ToInt();
    //        string messageFilter = options.GetFilterString("Message");
    //        int minFrequencyFilter = options.GetFilterString("MinFrequency").ToInt();

    //        DateTime? createdOnFromValue = (createdOnFromFilter == null) ? null
    //                          : (DateTime?)_dateTimeHelper.ConvertToUtcTime(createdOnFromFilter.Value, _dateTimeHelper.CurrentTimeZone);
    //        DateTime? createdToFromValue = (createdOnToFilter == null) ? null
    //                          : (DateTime?)_dateTimeHelper.ConvertToUtcTime(createdOnToFilter.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
    //        LogLevel? logLevel = logLevelFilter > 0 ? (LogLevel?)(logLevelFilter) : null;

    //        var Logger = EngineContext.Current.Resolve<ILogger>();
    //        var logItems = Logger.GetAllLogs(createdOnFromValue, createdToFromValue, messageFilter,
    //                logLevel, options.GetLimitOffset().Value, options.GetLimitRowcount().Value, minFrequencyFilter, new SortCondition[] { new SortCondition(options.SortColumnName, options.SortDirection == SortDirection.Dsc) });
    //        totalRecords = logItems.TotalCount;

    //        return new QueryResult<LogModel>()
    //        {
    //            Items = logItems.Select(x =>
    //            {
    //                var logModel = new LogModel()
    //                {
    //                    Id = x.Id,
    //                    LogLevelHint = s_logLevelHintMap[x.LogLevel],
    //                    LogLevel = x.LogLevel.GetLocalizedEnum(_localizationService, _workContext),
    //                    ShortMessage = x.ShortMessage,
    //                    FullMessage = x.FullMessage,
    //                    IpAddress = x.IpAddress,
    //                    UserId = x.UserId,
    //                    UserEmail = x.User != null ? x.User.Email : null,
    //                    PageUrl = x.PageUrl,
    //                    ReferrerUrl = x.ReferrerUrl,
    //                    CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc),
    //                    Frequency = x.Frequency,
    //                    ContentHash = x.ContentHash
    //                };

    //                if (x.UpdatedOnUtc.HasValue)
    //                    logModel.UpdatedOn = _dateTimeHelper.ConvertToUserTime(x.UpdatedOnUtc.Value, DateTimeKind.Utc);

    //                return logModel;
    //            }),
    //            TotalRecords = totalRecords
    //        };
    //    })
    //);


    //        #endregion
    //    }
    //}
}
