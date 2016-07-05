using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Polls;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Domain.Cms.Polls;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.WebSite.Mvc.Admin.Models.Polls;
using System;
using System.Linq;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class PollController : AdminControllerBase
    {
        #region Fields

        private readonly IPollService _pollService;
        private readonly ILanguageService _languageService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ISiteService _siteService;
        private readonly ISiteMappingService _siteMappingService;

        #endregion

        #region Constructors

        public PollController(IPollService pollService, ILanguageService languageService,
            IDateTimeHelper dateTimeHelper, ILocalizationService localizationService,
            IPermissionService permissionService, AdminAreaSettings adminAreaSettings,
            ISiteService siteService,
            ISiteMappingService siteMappingService)
        {
            this._pollService = pollService;
            this._languageService = languageService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._adminAreaSettings = adminAreaSettings;
            this._siteService = siteService;
            this._siteMappingService = siteMappingService;
        }

        #endregion

        #region Utilities

        private void PreparePollModel(PollModel model, Poll poll, bool excludeProperties)
        {
            model.AvailableSites = _siteService.GetAllSites().Select(s => s.ToModel()).ToList();

            if (!excludeProperties)
            {
                if (poll != null)
                    model.SelectedSiteIds = _siteMappingService.GetSitesIdsWithAccess(poll);
                else
                    model.SelectedSiteIds = new int[0];
            }

        }

        #endregion Utilities

        #region Polls

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            var polls = _pollService.GetPolls(0, false, dataTableParam.PageIndex, dataTableParam.PageSize, true).Select(x =>
                {
                    var m = x.ToModel();
                    if (x.StartDateUtc.HasValue)
                        m.StartDate = _dateTimeHelper.ConvertToUserTime(x.StartDateUtc.Value, DateTimeKind.Utc);
                    if (x.EndDateUtc.HasValue)
                        m.EndDate = _dateTimeHelper.ConvertToUserTime(x.EndDateUtc.Value, DateTimeKind.Utc);
                    m.LanguageName = x.Language.Name;
                    return m;
                }).AsQueryable();

            return DataTablesResult.Create(polls, dataTableParam);

        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);

            var model = new PollModel();
            model.Published = true;
            model.ShowOnHomePage = true;

            PreparePollModel(model, null, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(PollModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var poll = model.ToEntity();
                poll.StartDateUtc = model.StartDate;
                poll.EndDateUtc = model.EndDate;

                _pollService.InsertPoll(poll);

                _siteMappingService.SaveSiteMappings<Poll>(poll, model.SelectedSiteIds);

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Polls.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = poll.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);

            PreparePollModel(model, null, true);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            var poll = _pollService.GetPollById(id);
            if (poll == null)
                return RedirectToAction("List");

            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            var model = poll.ToModel();
            model.StartDate = poll.StartDateUtc;
            model.EndDate = poll.EndDateUtc;

            PreparePollModel(model, poll, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(PollModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            var poll = _pollService.GetPollById(model.Id);
            if (poll == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                poll = model.ToEntity(poll);
                poll.StartDateUtc = model.StartDate;
                poll.EndDateUtc = model.EndDate;

                _pollService.UpdatePoll(poll);

                _siteMappingService.SaveSiteMappings<Poll>(poll, model.SelectedSiteIds);

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Polls.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = poll.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);

            PreparePollModel(model, poll, true);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            var poll = _pollService.GetPollById(id);
            if (poll == null)
                return RedirectToAction("List");

            _pollService.DeletePoll(poll);

            NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Polls.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Poll answer

        [HttpPost]
        public ActionResult PollAnswers(int pollId, DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            var poll = _pollService.GetPollById(pollId);
            if (poll == null)
                throw new ArgumentException("No poll found with the specified id", "pollId");

            var answers = poll.PollAnswers.OrderBy(x => x.DisplayOrder).Select(x =>
                {
                    return new PollAnswerModel()
                    {
                        Id = x.Id,
                        PollId = x.PollId,
                        Name = x.Name,
                        NumberOfVotes = x.NumberOfVotes,
                        DisplayOrder = x.DisplayOrder
                    };
                }).AsQueryable();

            return DataTablesResult.Create(answers, dataTableParam);
        }


        public ActionResult OptionEditPopup(int id)
        {
            var pollAnswer = _pollService.GetPollAnswerById(id);
            if (pollAnswer == null)
                throw new ArgumentException("No poll answer found with the specified id", "id");
            var model= new PollAnswerModel();
            model.DisplayOrder = pollAnswer.DisplayOrder;
            model.Name = pollAnswer.Name;
            return View(model);
        }
        [HttpPost]
        public ActionResult OptionEditPopup(string btnId, string formId, PollAnswerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var pollAnswer = _pollService.GetPollAnswerById(model.Id);
            if (pollAnswer == null)
                throw new ArgumentException("No poll answer found with the specified id", "id");

            pollAnswer.Name = model.Name;
            pollAnswer.DisplayOrder = model.DisplayOrder;
            _pollService.UpdatePoll(pollAnswer.Poll);

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return View(model);
        }

        public ActionResult OptionCreatePopup(int pollId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();
            var model = new PollAnswerModel();
            model.DisplayOrder = 1;
            model.PollId = pollId;
            return View(model);
        }
        [HttpPost]
        public ActionResult OptionCreatePopup(string btnId, string formId, PollAnswerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var poll = _pollService.GetPollById(model.PollId);
            if (poll == null)
                throw new ArgumentException("No poll found with the specified id", "pollId");

            poll.PollAnswers.Add(new PollAnswer
            {
                Name = model.Name,
                DisplayOrder = model.DisplayOrder
            });
            _pollService.UpdatePoll(poll);
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        [HttpPost]
        public ActionResult PollAnswerDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            var pollAnswer = _pollService.GetPollAnswerById(id);
            if (pollAnswer == null)
                throw new ArgumentException("No poll answer found with the specified id", "id");

            int pollId = pollAnswer.PollId;
            _pollService.DeletePollAnswer(pollAnswer);
            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
