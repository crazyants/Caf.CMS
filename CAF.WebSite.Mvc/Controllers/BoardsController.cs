using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Forums;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.WebUI.Security;
using CAF.Infrastructure.Core.Domain.Cms.Forums;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Mvc.Models.Boards;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.WebUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Html;
using CAF.Infrastructure.Core.Exceptions;

namespace CAF.WebSite.Mvc.Controllers
{
    [RequireHttpsByConfigAttribute(SslRequirement.No)]
    public partial class BoardsController : PublicControllerBase
    {
        #region Fields

        private readonly IForumService _forumService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly ICountryService _countryService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
		private readonly ISiteContext _siteContext;
        private readonly ForumSettings _forumSettings;
        private readonly UserSettings _userSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Constructors

        public BoardsController(IForumService forumService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            ICountryService countryService,
            IWebHelper webHelper,
            IWorkContext workContext,
			ISiteContext siteContext,
            ForumSettings forumSettings,
            UserSettings userSettings,
            MediaSettings mediaSettings,
            IDateTimeHelper dateTimeHelper)
        {
            this._forumService = forumService;
            this._localizationService = localizationService;
            this._pictureService = pictureService;
            this._countryService = countryService;
            this._webHelper = webHelper;
            this._workContext = workContext;
			this._siteContext = siteContext;
            this._forumSettings = forumSettings;
            this._userSettings = userSettings;
            this._mediaSettings = mediaSettings;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected ForumTopicRowModel PrepareForumTopicRowModel(ForumTopic topic)
        {
            var topicModel = new ForumTopicRowModel()
            {
                Id = topic.Id,
                Subject = topic.Subject,
                SeName = topic.GetSeName(),
                LastPostId = topic.LastPostId,
                NumPosts = topic.NumPosts,
                Views = topic.Views,
                NumReplies = topic.NumReplies,
                ForumTopicType = topic.ForumTopicType,
                UserId = topic.UserId,
                AllowViewingProfiles = _userSettings.AllowViewingProfiles,
                UserName = topic.User.FormatUserName(true),
                IsUserGuest = topic.User.IsGuest()
            };

            var forumPosts = _forumService.GetAllPosts(topic.Id, 0, string.Empty, 1, _forumSettings.PostsPageSize);
            topicModel.TotalPostPages = forumPosts.TotalPages;

            return topicModel;
        }

        [NonAction]
        protected ForumRowModel PrepareForumRowModel(Forum forum)
        {
            var forumModel = new ForumRowModel()
            {
                Id = forum.Id,
                Name = forum.Name,
                SeName = forum.GetSeName(),
                Description = forum.Description,
                NumTopics = forum.NumTopics,
                NumPosts = forum.NumPosts,
                LastPostId = forum.LastPostId,
            };
            return forumModel;
        }

        [NonAction]
        protected ForumGroupModel PrepareForumGroupModel(ForumGroup forumGroup)
        {
            var forumGroupModel = new ForumGroupModel()
            {
                Id = forumGroup.Id,
                Name = forumGroup.Name,
                SeName = forumGroup.GetSeName(),
                Description = forumGroup.Description,
            };
            var forums = _forumService.GetAllForumsByGroupId(forumGroup.Id);
            foreach (var forum in forums)
            {
                var forumModel = PrepareForumRowModel(forum);
                forumGroupModel.Forums.Add(forumModel);
            }
            return forumGroupModel;
        }

        [NonAction]
        protected IEnumerable<SelectListItem> ForumTopicTypesList()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Forum.Normal"),
                Value = ((int)ForumTopicType.Normal).ToString()
            });

            list.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Forum.Sticky"),
                Value = ((int)ForumTopicType.Sticky).ToString()
            });

            list.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Forum.Announcement"),
                Value = ((int)ForumTopicType.Announcement).ToString()
            });

            return list;
        }

        [NonAction]
        protected IEnumerable<SelectListItem> ForumGroupsForumsList()
        {
            var forumsList = new List<SelectListItem>();
            var separator = "--";
            var forumGroups = _forumService.GetAllForumGroups();

            foreach (var fg in forumGroups)
            {
                // Add the forum group with Value of 0 so it won't be used as a target forum
                forumsList.Add(new SelectListItem { Text = fg.Name, Value = "0" });

                var forums = _forumService.GetAllForumsByGroupId(fg.Id);
                foreach (var f in forums)
                {
                    forumsList.Add(new SelectListItem { Text = string.Format("{0}{1}", separator, f.Name), Value = f.Id.ToString() });
                }
            }

            return forumsList;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            var forumGroups = _forumService.GetAllForumGroups();

            var model = new BoardsIndexModel()
            {
                CurrentTime = _dateTimeHelper.ConvertToUserTime(DateTime.UtcNow)
            };
            foreach (var forumGroup in forumGroups)
            {

                var forumGroupModel = PrepareForumGroupModel(forumGroup);
                model.ForumGroups.Add(forumGroupModel);
            }
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult ActiveDiscussionsSmall()
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            int topicLimit = _forumSettings.HomePageActiveDiscussionsTopicCount;
            var topics = _forumService.GetActiveTopics(0, topicLimit);
            if (topics.Count == 0)
                return Content("");

            var model = new ActiveDiscussionsModel();
            foreach (var topic in topics)
            {
                var topicModel = PrepareForumTopicRowModel(topic);
                model.ForumTopics.Add(topicModel);
            }
            model.ViewAllLinkEnabled = true;
            model.ActiveDiscussionsFeedEnabled = _forumSettings.ActiveDiscussionsFeedEnabled;
            model.PostsPageSize = _forumSettings.PostsPageSize;

            return PartialView(model);
        }

        public ActionResult ActiveDiscussions(int forumId = 0)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            int topicLimit = _forumSettings.ActiveDiscussionsPageTopicCount;
            var topics = _forumService.GetActiveTopics(forumId, topicLimit);

            var model = new ActiveDiscussionsModel();
            foreach (var topic in topics)
            {
                var topicModel = PrepareForumTopicRowModel(topic);
                model.ForumTopics.Add(topicModel);
            }
            model.ViewAllLinkEnabled = false;
            model.ActiveDiscussionsFeedEnabled = _forumSettings.ActiveDiscussionsFeedEnabled;
            model.PostsPageSize = _forumSettings.PostsPageSize;
            return View(model);
        }

        public ActionResult ActiveDiscussionsRss(int forumId = 0)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            if (!_forumSettings.ActiveDiscussionsFeedEnabled)
            {
				return HttpNotFound();
            }

            int topicLimit = _forumSettings.ActiveDiscussionsFeedCount;
            var topics = _forumService.GetActiveTopics(forumId, topicLimit);
            string url = Url.Action("ActiveDiscussionsRSS", null, (object)null, "http");

            var feedTitle = _localizationService.GetResource("Forum.ActiveDiscussionsFeedTitle");
            var feedDescription = _localizationService.GetResource("Forum.ActiveDiscussionsFeedDescription");

            var feed = new SyndicationFeed(
									string.Format(feedTitle, _siteContext.CurrentSite.Name),
                                    feedDescription,
                                    new Uri(url),
                                    "ActiveDiscussionsRSS",
                                    DateTime.UtcNow);

            var items = new List<SyndicationItem>();

            var viewsText = _localizationService.GetResource("Forum.Views");
            var repliesText = _localizationService.GetResource("Forum.Replies");

            foreach (var topic in topics)
            {
                string topicUrl = Url.RouteUrl("TopicSlug", new { id = topic.Id, slug = topic.GetSeName() }, "http");
                string content = String.Format("{2}: {0}, {3}: {1}", topic.NumReplies.ToString(), topic.Views.ToString(), repliesText, viewsText);

                items.Add(new SyndicationItem(topic.Subject, content, new Uri(topicUrl),
                    String.Format("Topic:{0}", topic.Id), (topic.LastPostTime ?? topic.UpdatedOnUtc)));
            }
            feed.Items = items;

            return new RssActionResult() { Feed = feed };
        }

        public ActionResult ForumGroup(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            var forumGroup = _forumService.GetForumGroupById(id);
            if (forumGroup == null)
                return RedirectToRoute("Boards");

            var model = PrepareForumGroupModel(forumGroup);
            return View(model);
        }

        public ActionResult Forum(int id, int page = 1)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            var forum = _forumService.GetForumById(id);

            if (forum != null)
            {
                var model = new ForumPageModel();
                model.Id = forum.Id;
                model.Name = forum.Name;
                model.SeName = forum.GetSeName();
                model.Description = forum.Description;

                int pageSize = 10;
                if (_forumSettings.TopicsPageSize > 0)
                {
                    pageSize = _forumSettings.TopicsPageSize;
                }

                //subscription                
                if (_forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser))
                {
                    model.WatchForumText = _localizationService.GetResource("Forum.WatchForum");

                    var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentUser.Id, forum.Id, 0, 0, 1).FirstOrDefault();
                    if (forumSubscription != null)
                    {
                        model.WatchForumText = _localizationService.GetResource("Forum.UnwatchForum");
                        model.WatchForumSubscribed = true; // codehint: sm-add
                    }
                }

                var topics = _forumService.GetAllTopics(forum.Id, 0, string.Empty, 
                    ForumSearchType.All, 0, (page - 1), pageSize);
                model.TopicPageSize = topics.PageSize;
                model.TopicTotalRecords = topics.TotalCount;
                model.TopicPageIndex = topics.PageIndex;
                foreach (var topic in topics)
                {
                    var topicModel = PrepareForumTopicRowModel(topic);
                    model.ForumTopics.Add(topicModel);
                }
                model.IsUserAllowedToSubscribe = _forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser);
                model.ForumFeedsEnabled = _forumSettings.ForumFeedsEnabled;
                model.PostsPageSize = _forumSettings.PostsPageSize;
                return View(model);
            }

            return RedirectToRoute("Boards");
        }

        public ActionResult ForumRss(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            if (!_forumSettings.ForumFeedsEnabled)
            {
				return HttpNotFound();
            }

            int topicLimit = _forumSettings.ForumFeedCount;
            var forum = _forumService.GetForumById(id);

            if (forum != null)
            {
                //Order by newest topic posts & limit the number of topics to return
                var topics = _forumService.GetAllTopics(forum.Id, 0, string.Empty,
                     ForumSearchType.All, 0, 0, topicLimit);

                string url = Url.Action("ForumRSS", null, new { id = forum.Id }, "http");

                var feedTitle = _localizationService.GetResource("Forum.ForumFeedTitle");
                var feedDescription = _localizationService.GetResource("Forum.ForumFeedDescription");

                var feed = new SyndicationFeed(
										string.Format(feedTitle, _siteContext.CurrentSite.Name, forum.Name),
                                        feedDescription,
                                        new Uri(url),
                                        string.Format("ForumRSS:{0}", forum.Id),
                                        DateTime.UtcNow);

                var items = new List<SyndicationItem>();

                var viewsText = _localizationService.GetResource("Forum.Views");
                var repliesText = _localizationService.GetResource("Forum.Replies");

                foreach (var topic in topics)
                {
                    string topicUrl = Url.RouteUrl("TopicSlug", new { id = topic.Id, slug = topic.GetSeName() }, "http");
                    string content = string.Format("{2}: {0}, {3}: {1}", topic.NumReplies.ToString(), topic.Views.ToString(), repliesText, viewsText);

                    items.Add(new SyndicationItem(topic.Subject, content, new Uri(topicUrl), String.Format("Topic:{0}", topic.Id),
                        (topic.LastPostTime ?? topic.UpdatedOnUtc)));
                }

                feed.Items = items;

                return new RssActionResult() { Feed = feed };
            }

            return new RssActionResult() { Feed = new SyndicationFeed() };
        }

        [HttpPost]
        public ActionResult ForumWatch(int id)
        {
            bool subscribed = false;
            string watchTopic = _localizationService.GetResource("Forum.WatchForum");
            string unwatchTopic = _localizationService.GetResource("Forum.UnwatchForum");
            string returnText = watchTopic;

            var forum = _forumService.GetForumById(id);
            if (forum == null)
            {
                return Json(new { Subscribed = subscribed, Text = returnText, Error = true });
            }

            if (!_forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser))
            {
                return Json(new { Subscribed = subscribed, Text = returnText, Error = true });
            }

            var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentUser.Id,
                forum.Id, 0, 0, 1).FirstOrDefault();

            if (forumSubscription == null)
            {
                forumSubscription = new ForumSubscription()
                {
                    SubscriptionGuid = Guid.NewGuid(),
                    UserId = _workContext.CurrentUser.Id,
                    ForumId = forum.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                _forumService.InsertSubscription(forumSubscription);
                subscribed = true;
                returnText = unwatchTopic;
            }
            else
            {
                _forumService.DeleteSubscription(forumSubscription);
                subscribed = false;
            }

            return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
        }

        public ActionResult Topic(int id, int page = 1)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            var forumTopic = _forumService.GetTopicById(id);

            if (forumTopic != null)
            {
                //load posts
                var posts = _forumService.GetAllPosts(forumTopic.Id, 0, string.Empty,
                    page - 1, _forumSettings.PostsPageSize);
                // if no posts area loaded, redirect to the first page
                if (posts.Count == 0 && page > 1)
                {
                    return RedirectToRoute("TopicSlug", new {id = forumTopic.Id, slug = forumTopic.GetSeName()});
                }

                //update view count
                forumTopic.Views += 1;
                _forumService.UpdateTopic(forumTopic);

                //prepare model
                var model = new ForumTopicPageModel();
                model.Id = forumTopic.Id;
                model.Subject= forumTopic.Subject;
                model.SeName = forumTopic.GetSeName();

                model.IsUserAllowedToEditTopic = _forumService.IsUserAllowedToEditTopic(_workContext.CurrentUser, forumTopic);
                model.IsUserAllowedToDeleteTopic = _forumService.IsUserAllowedToDeleteTopic(_workContext.CurrentUser, forumTopic);
                model.IsUserAllowedToMoveTopic = _forumService.IsUserAllowedToMoveTopic(_workContext.CurrentUser, forumTopic);
                model.IsUserAllowedToSubscribe = _forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser);

                if (model.IsUserAllowedToSubscribe)
                {
                    model.WatchTopicText = _localizationService.GetResource("Forum.WatchTopic");

                    var forumTopicSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentUser.Id,
                        0, forumTopic.Id, 0, 1).FirstOrDefault();
                    if (forumTopicSubscription != null)
                    {
                        model.WatchTopicText = _localizationService.GetResource("Forum.UnwatchTopic");
                    }
                }
                model.PostsPageIndex = posts.PageIndex;
                model.PostsPageSize = posts.PageSize;
                model.PostsTotalRecords = posts.TotalCount;
                foreach (var post in posts)
                {
                    var forumPostModel = new ForumPostModel()
                    {
                        Id = post.Id,
                        ForumTopicId =  post.TopicId,
                        ForumTopicSeName = forumTopic.GetSeName(),
                        FormattedText = post.FormatPostText(),
                        IsCurrentUserAllowedToEditPost = _forumService.IsUserAllowedToEditPost(_workContext.CurrentUser, post),
                        IsCurrentUserAllowedToDeletePost = _forumService.IsUserAllowedToDeletePost(_workContext.CurrentUser, post),
                        UserId = post.UserId,
                        AllowViewingProfiles = _userSettings.AllowViewingProfiles,
                        UserName = post.User.FormatUserName(true),
                        IsUserForumModerator = post.User.IsForumModerator(),
                        IsUserGuest= post.User.IsGuest(),
                        ShowUsersPostCount = _forumSettings.ShowUsersPostCount,
                        ForumPostCount = post.User.GetAttribute<int>(SystemUserAttributeNames.ForumPostCount),
                        ShowUsersJoinDate = _userSettings.ShowUsersJoinDate,
                        UserJoinDate = post.User.CreatedOnUtc,
                        AllowPrivateMessages = _forumSettings.AllowPrivateMessages,
                        SignaturesEnabled = _forumSettings.SignaturesEnabled,
                        FormattedSignature = post.User.GetAttribute<string>(SystemUserAttributeNames.Signature).FormatForumSignatureText(),
                    };
                    //created on string
                    if (_forumSettings.RelativeDateTimeFormattingEnabled)
                       forumPostModel.PostCreatedOnStr = post.CreatedOnUtc.RelativeFormat(true, "f");
                    else
                        forumPostModel.PostCreatedOnStr =_dateTimeHelper.ConvertToUserTime(post.CreatedOnUtc, DateTimeKind.Utc).ToString("f");
                    //avatar
                    if (_userSettings.AllowUsersToUploadAvatars)
                    {
                        var user = post.User;
                        string avatarUrl = _pictureService.GetPictureUrl(user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId), _mediaSettings.AvatarPictureSize, false);
                        if (String.IsNullOrEmpty(avatarUrl) && _userSettings.DefaultAvatarEnabled)
                            avatarUrl = _pictureService.GetDefaultPictureUrl(_mediaSettings.AvatarPictureSize, PictureType.Avatar);
                        forumPostModel.UserAvatarUrl = avatarUrl;
                    }
                    //location
                    forumPostModel.ShowUsersLocation = _userSettings.ShowUsersLocation;
                    if (_userSettings.ShowUsersLocation)
                    {
                        var countryId = post.User.GetAttribute<int>(SystemUserAttributeNames.CountryId);
                        var country = _countryService.GetCountryById(countryId);
                        forumPostModel.UserLocation = country != null ? country.GetLocalized(x => x.Name) : string.Empty;
                    }

                    // page number is needed for creating post link in _ForumPost partial view
                    forumPostModel.CurrentTopicPage = page;
                    model.ForumPostModels.Add(forumPostModel);
                }

                return View(model);
            }

            return RedirectToRoute("Boards");
        }

        [HttpPost]
        public ActionResult TopicWatch(int id)
        {
            bool subscribed = false;
            string watchTopic = _localizationService.GetResource("Forum.WatchTopic");
            string unwatchTopic = _localizationService.GetResource("Forum.UnwatchTopic");
            string returnText = watchTopic;

            var forumTopic = _forumService.GetTopicById(id);
            if (forumTopic == null)
            {
                return Json(new { Subscribed = subscribed, Text = returnText, Error = true });
            }

            if (!_forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser))
            {
                return Json(new { Subscribed = subscribed, Text = returnText, Error = true });
            }

            var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentUser.Id,
                0, forumTopic.Id, 0, 1).FirstOrDefault();

            if (forumSubscription == null)
            {
                forumSubscription = new ForumSubscription()
                {
                    SubscriptionGuid = Guid.NewGuid(),
                    UserId = _workContext.CurrentUser.Id,
                    TopicId = forumTopic.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                _forumService.InsertSubscription(forumSubscription);
                subscribed = true;
                returnText = unwatchTopic;
            }
            else
            {
                _forumService.DeleteSubscription(forumSubscription);
                subscribed = false;
            }

            return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
        }

        public ActionResult TopicMove(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            var forumTopic = _forumService.GetTopicById(id);

            if (forumTopic == null)
            {
				return HttpNotFound();
            }

            var model = new TopicMoveModel();
            model.ForumList = ForumGroupsForumsList();
            model.Id = forumTopic.Id;
            model.TopicSeName = forumTopic.GetSeName();
            model.ForumSelected = forumTopic.ForumId;
            return View(model);
        }

        [HttpPost]
        public ActionResult TopicMove(TopicMoveModel model)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forumTopic = _forumService.GetTopicById(model.Id);

            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }

            var newForumId = model.ForumSelected;
            var forum = _forumService.GetForumById(newForumId);

            if (forum != null && forumTopic.ForumId != newForumId)
            {
                _forumService.MoveTopic(forumTopic.Id, newForumId);
            }

            return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = forumTopic.GetSeName() });
        }

        public ActionResult TopicDelete(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            var forumTopic = _forumService.GetTopicById(id);
            if (forumTopic != null)
            {
                if (!_forumService.IsUserAllowedToDeleteTopic(_workContext.CurrentUser, forumTopic))
                {
                    return new HttpUnauthorizedResult();
                }
                var forum = _forumService.GetForumById(forumTopic.ForumId);

                _forumService.DeleteTopic(forumTopic);

                if (forum != null)
                {
                    return RedirectToRoute("ForumSlug", new { id = forum.Id, slug = forum.GetSeName() });
                }
            }

            return RedirectToRoute("Boards");
        }

        public ActionResult TopicCreate(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            var forum = _forumService.GetForumById(id);

            if (forum == null)
            {
				return HttpNotFound();
            }

            if (_forumService.IsUserAllowedToCreateTopic(_workContext.CurrentUser, forum) == false)
            {
                return new HttpUnauthorizedResult();
            }

            var model = new EditForumTopicModel();
            model.Id = 0;
            model.IsEdit = false;
            model.ForumId = forum.Id;
            model.ForumName = forum.Name;
            model.ForumSeName = forum.GetSeName();
            model.ForumEditor = _forumSettings.ForumEditor;
            model.IsUserAllowedToSetTopicPriority = _forumService.IsUserAllowedToSetTopicPriority(_workContext.CurrentUser);
            model.TopicPriorities = ForumTopicTypesList();
            model.IsUserAllowedToSubscribe = _forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser);
            model.Subscribed = false;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult TopicCreate(EditForumTopicModel model)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            var forum = _forumService.GetForumById(model.ForumId);

            if (forum == null)
            {
                return RedirectToRoute("Boards");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!_forumService.IsUserAllowedToCreateTopic(_workContext.CurrentUser, forum))
                    {
                        return new HttpUnauthorizedResult();
                    }

                    string subject = model.Subject;
                    var maxSubjectLength = _forumSettings.TopicSubjectMaxLength;
                    if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                    {
                        subject = subject.Substring(0, maxSubjectLength);
                    }

                    var text = model.Text;
                    var maxPostLength = _forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                    {
                        text = text.Substring(0, maxPostLength);
                    }

                    var topicType = ForumTopicType.Normal;

                    string ipAddress = _webHelper.GetCurrentIpAddress();

                    var nowUtc = DateTime.UtcNow;

                    if (_forumService.IsUserAllowedToSetTopicPriority(_workContext.CurrentUser))
                    {
                        topicType = (ForumTopicType) Enum.ToObject(typeof (ForumTopicType), model.TopicTypeId);
                    }

                    //forum topic
                    var forumTopic = new ForumTopic()
                    {
                        ForumId = forum.Id,
                        UserId = _workContext.CurrentUser.Id,
                        TopicTypeId = (int) topicType,
                        Subject = subject,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    _forumService.InsertTopic(forumTopic, true);

                    //forum post
                    var forumPost = new ForumPost()
                    {
                        TopicId = forumTopic.Id,
                        UserId = _workContext.CurrentUser.Id,
                        Text = text,
                        IPAddress = ipAddress,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    _forumService.InsertPost(forumPost, false);

                    //update forum topic
                    forumTopic.NumPosts = 1;
                    forumTopic.LastPostId = forumPost.Id;
                    forumTopic.LastPostUserId = forumPost.UserId;
                    forumTopic.LastPostTime = forumPost.CreatedOnUtc;
                    forumTopic.UpdatedOnUtc = nowUtc;
                    _forumService.UpdateTopic(forumTopic);

                    //subscription                
                    if (_forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser))
                    {
                        if (model.Subscribed)
                        {
                            var forumSubscription = new ForumSubscription()
                            {
                                SubscriptionGuid = Guid.NewGuid(),
                                UserId = _workContext.CurrentUser.Id,
                                TopicId = forumTopic.Id,
                                CreatedOnUtc = nowUtc
                            };

                            _forumService.InsertSubscription(forumSubscription);
                        }
                    }

                    return RedirectToRoute("TopicSlug", new {id = forumTopic.Id, slug = forumTopic.GetSeName()});
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // redisplay form
            model.TopicPriorities = ForumTopicTypesList();
            model.IsEdit = false;
            model.ForumId = forum.Id;
            model.ForumName = forum.Name;
            model.ForumSeName = forum.GetSeName();
            model.Id = 0;
            model.IsUserAllowedToSetTopicPriority = _forumService.IsUserAllowedToSetTopicPriority(_workContext.CurrentUser);
            model.IsUserAllowedToSubscribe = _forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser);
            model.ForumEditor = _forumSettings.ForumEditor;

            return View(model);
        }

        public ActionResult TopicEdit(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            var forumTopic = _forumService.GetTopicById(id);

            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }

            if (!_forumService.IsUserAllowedToEditTopic(_workContext.CurrentUser, forumTopic))
            {
                return new HttpUnauthorizedResult();
            }

            var forum = forumTopic.Forum;
            if (forum == null)
            {
                return RedirectToRoute("Boards");
            }

            var model = new EditForumTopicModel();
            model.IsEdit = true;
            model.TopicPriorities = ForumTopicTypesList();
            model.ForumName = forum.Name;
            model.ForumSeName = forum.GetSeName();
            var firstPost = forumTopic.GetFirstPost(_forumService);
            model.Text = firstPost.Text;
            model.Subject = forumTopic.Subject;
            model.TopicTypeId = forumTopic.TopicTypeId;
            model.Id = forumTopic.Id;
            model.ForumId = forum.Id;
            model.ForumEditor = _forumSettings.ForumEditor;

            model.IsUserAllowedToSetTopicPriority = _forumService.IsUserAllowedToSetTopicPriority(_workContext.CurrentUser);
            model.IsUserAllowedToSubscribe = _forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser);

            //subscription            
            if (model.IsUserAllowedToSubscribe)
            {
                var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentUser.Id,
                    0, forumTopic.Id, 0, 1).FirstOrDefault();
                model.Subscribed = forumSubscription != null;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult TopicEdit(EditForumTopicModel model)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            var forumTopic = _forumService.GetTopicById(model.Id);

            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }
            var forum = forumTopic.Forum;
            if (forum == null)
            {
                return RedirectToRoute("Boards");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!_forumService.IsUserAllowedToEditTopic(_workContext.CurrentUser, forumTopic))
                    {
                        return new HttpUnauthorizedResult();
                    }

                    string subject = model.Subject;
                    var maxSubjectLength = _forumSettings.TopicSubjectMaxLength;
                    if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                    {
                        subject = subject.Substring(0, maxSubjectLength);
                    }

                    var text = model.Text;
                    var maxPostLength = _forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                    {
                        text = text.Substring(0, maxPostLength);
                    }

                    var topicType = ForumTopicType.Normal;

                    string ipAddress = _webHelper.GetCurrentIpAddress();

                    DateTime nowUtc = DateTime.UtcNow;

                    if (_forumService.IsUserAllowedToSetTopicPriority(_workContext.CurrentUser))
                    {
                        topicType = (ForumTopicType) Enum.ToObject(typeof (ForumTopicType), model.TopicTypeId);
                    }

                    //forum topic
                    forumTopic.TopicTypeId = (int) topicType;
                    forumTopic.Subject = subject;
                    forumTopic.UpdatedOnUtc = nowUtc;
                    _forumService.UpdateTopic(forumTopic);

                    //forum post                
                    var firstPost = forumTopic.GetFirstPost(_forumService);
                    if (firstPost != null)
                    {
                        firstPost.Text = text;
                        firstPost.UpdatedOnUtc = nowUtc;
                        _forumService.UpdatePost(firstPost);
                    }
                    else
                    {
                        //error (not possible)
                        firstPost = new ForumPost()
                        {
                            TopicId = forumTopic.Id,
                            UserId = forumTopic.UserId,
                            Text = text,
                            IPAddress = ipAddress,
                            UpdatedOnUtc = nowUtc
                        };

                        _forumService.InsertPost(firstPost, false);
                    }

                    //subscription
                    if (_forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser))
                    {
                        var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentUser.Id,
                                                                                  0, forumTopic.Id, 0, 1).FirstOrDefault
                            ();
                        if (model.Subscribed)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription()
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    UserId = _workContext.CurrentUser.Id,
                                    TopicId = forumTopic.Id,
                                    CreatedOnUtc = nowUtc
                                };

                                _forumService.InsertSubscription(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                _forumService.DeleteSubscription(forumSubscription);
                            }
                        }
                    }

                    // redirect to the topic page with the topic slug
                    return RedirectToRoute("TopicSlug", new {id = forumTopic.Id, slug = forumTopic.GetSeName()});
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // redisplay form
            model.TopicPriorities = ForumTopicTypesList();
            model.IsEdit = true;
            model.ForumName = forum.Name;
            model.ForumSeName = forum.GetSeName();
            model.ForumId = forum.Id;
            model.ForumEditor = _forumSettings.ForumEditor;

            model.IsUserAllowedToSetTopicPriority = _forumService.IsUserAllowedToSetTopicPriority(_workContext.CurrentUser);
            model.IsUserAllowedToSubscribe = _forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser);

            return View(model);
        }

        public ActionResult PostDelete(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            var forumPost = _forumService.GetPostById(id);

            if (forumPost != null)
            {
                if (!_forumService.IsUserAllowedToDeletePost(_workContext.CurrentUser, forumPost))
                {
                    return new HttpUnauthorizedResult();
                }

                var forumTopic = forumPost.ForumTopic;
                var forumId = forumTopic.Forum.Id;
                var forumSlug = forumTopic.Forum.GetSeName();

                _forumService.DeletePost(forumPost);

                string url = string.Empty;
                //get topic one more time because it can be deleted (first or only post deleted)
                forumTopic = _forumService.GetTopicById(forumPost.TopicId);
                if (forumTopic == null)
                {
                    return RedirectToRoute("ForumSlug", new { id = forumId, slug = forumSlug });
                }
                else
                {
                    return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = forumTopic.GetSeName() });
                }
            }

            return RedirectToRoute("Boards");
        }

        public ActionResult PostCreate(int id, int? quote)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            var forumTopic = _forumService.GetTopicById(id);

            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }

            if (!_forumService.IsUserAllowedToCreatePost(_workContext.CurrentUser, forumTopic))
            {
                return new HttpUnauthorizedResult();
            }

            var forum = forumTopic.Forum;
            if (forum == null)
            {
                return RedirectToRoute("Boards");
            }

            var model = new EditForumPostModel()
            {
                Id = 0,
                ForumTopicId = forumTopic.Id,
                IsEdit = false,
                ForumEditor = _forumSettings.ForumEditor,
                ForumName = forum.Name,
                ForumTopicSubject = forumTopic.Subject,
                ForumTopicSeName = forumTopic.GetSeName(),
                IsUserAllowedToSubscribe = _forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser),
                Subscribed = false,
            };
            
            //subscription            
            if (model.IsUserAllowedToSubscribe)
            {
                var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentUser.Id, 0, forumTopic.Id, 0, 1).FirstOrDefault();
                model.Subscribed = forumSubscription != null;
            }

            // Insert the quoted text
            string text = string.Empty;
            if (quote.HasValue)
            {
                var quotePost = _forumService.GetPostById(quote.Value);
                if (quotePost != null && quotePost.TopicId == forumTopic.Id)
                {
                    var quotePostText = quotePost.Text;

                    switch (_forumSettings.ForumEditor)
                    {
                        case EditorType.SimpleTextBox:
                            text = String.Format("{0}:\n{1}\n", quotePost.User.FormatUserName(), quotePostText);
                            break;
                        case EditorType.BBCodeEditor:
                            text = String.Format("[quote={0}]{1}[/quote]", quotePost.User.FormatUserName(), BBCodeHelper.RemoveQuotes(quotePostText));
                            break;
                    }
                    model.Text = text;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PostCreate(EditForumPostModel model)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            var forumTopic = _forumService.GetTopicById(model.ForumTopicId);
            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!_forumService.IsUserAllowedToCreatePost(_workContext.CurrentUser, forumTopic))
                        return new HttpUnauthorizedResult();

                    var text = model.Text;
                    var maxPostLength = _forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                        text = text.Substring(0, maxPostLength);

                    string ipAddress = _webHelper.GetCurrentIpAddress();

                    DateTime nowUtc = DateTime.UtcNow;

                    var forumPost = new ForumPost()
                    {
                        TopicId = forumTopic.Id,
                        UserId = _workContext.CurrentUser.Id,
                        Text = text,
                        IPAddress = ipAddress,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    _forumService.InsertPost(forumPost, true);

                    //subscription
                    if (_forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser))
                    {
                        var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentUser.Id,
                            0, forumPost.TopicId, 0, 1).FirstOrDefault();
                        if (model.Subscribed)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription()
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    UserId = _workContext.CurrentUser.Id,
                                    TopicId = forumPost.TopicId,
                                    CreatedOnUtc = nowUtc
                                };

                                _forumService.InsertSubscription(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                _forumService.DeleteSubscription(forumSubscription);
                            }
                        }
                    }

                    int pageSize = 10;
                    if (_forumSettings.PostsPageSize > 0)
                        pageSize = _forumSettings.PostsPageSize;

                    int pageIndex = (_forumService.CalculateTopicPageIndex(forumPost.TopicId, pageSize, forumPost.Id) + 1);
                    var url = string.Empty;
                    if (pageIndex > 1)
                    {
                        url = Url.RouteUrl("TopicSlug", new { id = forumPost.TopicId, slug = forumPost.ForumTopic.GetSeName(), page = pageIndex });
                    }
                    else
                    {
                        url = Url.RouteUrl("TopicSlug", new { id = forumPost.TopicId, slug = forumPost.ForumTopic.GetSeName() });
                    }
                    return Redirect(string.Format("{0}#{1}", url, forumPost.Id));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // redisplay form
            var forum = forumTopic.Forum;
            if (forum == null)
                return RedirectToRoute("Boards");

            model.IsEdit = false;
            model.ForumName = forum.Name;
            model.ForumTopicId = forumTopic.Id;
            model.ForumTopicSubject = forumTopic.Subject;
            model.ForumTopicSeName = forumTopic.GetSeName();
            model.Id = 0;
            model.IsUserAllowedToSubscribe = _forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser);
            model.ForumEditor = _forumSettings.ForumEditor;
            
            return View(model);
        }

        public ActionResult PostEdit(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            var forumPost = _forumService.GetPostById(id);

            if (forumPost == null)
            {
                return RedirectToRoute("Boards");
            }
            if (!_forumService.IsUserAllowedToEditPost(_workContext.CurrentUser, forumPost))
            {
                return new HttpUnauthorizedResult();
            }
            var forumTopic = forumPost.ForumTopic;
            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }
            var forum = forumTopic.Forum;
            if (forum == null)
            {
                return RedirectToRoute("Boards");
            }


            var model = new EditForumPostModel()
            {
                Id = forumPost.Id,
                ForumTopicId = forumTopic.Id,
                IsEdit = true,
                ForumEditor = _forumSettings.ForumEditor,
                ForumName = forum.Name,
                ForumTopicSubject = forumTopic.Subject,
                ForumTopicSeName = forumTopic.GetSeName(),
                IsUserAllowedToSubscribe = _forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser),
                Subscribed = false,
                Text = forumPost.Text,
            };

            //subscription
            if (model.IsUserAllowedToSubscribe)
            {
                var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentUser.Id,
                    0, forumTopic.Id, 0, 1).FirstOrDefault();
                model.Subscribed = forumSubscription != null;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PostEdit(EditForumPostModel model)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            var forumPost = _forumService.GetPostById(model.Id);
            if (forumPost == null)
            {
                return RedirectToRoute("Boards");
            }

            if (!_forumService.IsUserAllowedToEditPost(_workContext.CurrentUser, forumPost))
            {
                return new HttpUnauthorizedResult();
            }

            var forumTopic = forumPost.ForumTopic;
            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }

            var forum = forumTopic.Forum;
            if (forum == null)
            {
                return RedirectToRoute("Boards");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DateTime nowUtc = DateTime.UtcNow;

                    var text = model.Text;
                    var maxPostLength = _forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                    {
                        text = text.Substring(0, maxPostLength);
                    }

                    forumPost.UpdatedOnUtc = nowUtc;
                    forumPost.Text = text;
                    _forumService.UpdatePost(forumPost);

                    //subscription
                    if (_forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser))
                    {
                        var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentUser.Id,
                            0, forumPost.TopicId, 0, 1).FirstOrDefault();
                        if (model.Subscribed)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription()
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    UserId = _workContext.CurrentUser.Id,
                                    TopicId = forumPost.TopicId,
                                    CreatedOnUtc = nowUtc
                                };
                                _forumService.InsertSubscription(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                _forumService.DeleteSubscription(forumSubscription);
                            }
                        }
                    }

                    int pageSize = 10;
                    if (_forumSettings.PostsPageSize > 0)
                    {
                        pageSize = _forumSettings.PostsPageSize;
                    }
                    int pageIndex = (_forumService.CalculateTopicPageIndex(forumPost.TopicId, pageSize, forumPost.Id) + 1);
                    var url = string.Empty;
                    if (pageIndex > 1)
                    {
                        url = Url.RouteUrl("TopicSlug", new { id = forumPost.TopicId, slug = forumPost.ForumTopic.GetSeName(), page = pageIndex });
                    }
                    else
                    {
                        url = Url.RouteUrl("TopicSlug", new { id = forumPost.TopicId, slug = forumPost.ForumTopic.GetSeName() });
                    }
                    return Redirect(string.Format("{0}#{1}", url, forumPost.Id));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            model.IsEdit = true;
            model.ForumName = forum.Name;
            model.ForumTopicId = forumTopic.Id;
            model.ForumTopicSubject = forumTopic.Subject;
            model.ForumTopicSeName = forumTopic.GetSeName();
            model.Id = forumPost.Id;
            model.IsUserAllowedToSubscribe = _forumService.IsUserAllowedToSubscribe(_workContext.CurrentUser);
            model.ForumEditor = _forumSettings.ForumEditor;
            
            return View(model);
        }

        public ActionResult Search(string searchterms, bool? adv, string forumId,
            string within, string limitDays, int page = 1)
        {
            if (!_forumSettings.ForumsEnabled)
            {
				return HttpNotFound();
            }

            int pageSize = 10;

            var model = new SearchModel();

            // Create the values for the "Limit results to previous" select list
            var limitList = new List<SelectListItem>
                            {
                                new SelectListItem
                                {
                                    Text = _localizationService.GetResource("Forum.Search.LimitResultsToPrevious.AllResults"),
                                    Value = "0"
                                },
                                new SelectListItem
                                {
                                    Text = _localizationService.GetResource("Forum.Search.LimitResultsToPrevious.1day"),
                                    Value = "1"
                                },
                                new SelectListItem
                                {
                                    Text = _localizationService.GetResource("Forum.Search.LimitResultsToPrevious.7days"),
                                    Value = "7"
                                },
                                new SelectListItem
                                {
                                    Text = _localizationService.GetResource("Forum.Search.LimitResultsToPrevious.2weeks"),
                                    Value = "14"
                                },
                                new SelectListItem
                                {
                                    Text = _localizationService.GetResource("Forum.Search.LimitResultsToPrevious.1month"),
                                    Value = "30"
                                },
                                new SelectListItem
                                {
                                    Text = _localizationService.GetResource("Forum.Search.LimitResultsToPrevious.3months"),
                                    Value = "92"
                                },
                                new SelectListItem
                                {
                                    Text= _localizationService.GetResource("Forum.Search.LimitResultsToPrevious.6months"),
                                    Value = "183"
                                },
                                new SelectListItem
                                {
                                    Text = _localizationService.GetResource("Forum.Search.LimitResultsToPrevious.1year"),
                                    Value = "365"
                                }
                            };
            model.LimitList = limitList;

            // Create the values for the "Search in forum" select list
            var forumsSelectList = new List<SelectListItem>();
            forumsSelectList.Add(
                new SelectListItem
                {
                    Text = _localizationService.GetResource("Forum.Search.SearchInForum.All"),
                    Value = "0",
                    Selected = true,
                });
            var separator = "--";
            var forumGroups = _forumService.GetAllForumGroups();
            foreach (var fg in forumGroups)
            {
                // Add the forum group with value as '-' so it can't be used as a target forum id
                forumsSelectList.Add(new SelectListItem { Text = fg.Name, Value = "-" });

                var forums = _forumService.GetAllForumsByGroupId(fg.Id);
                foreach (var f in forums)
                {
                    forumsSelectList.Add(
                        new SelectListItem
                        {
                            Text = string.Format("{0}{1}", separator, f.Name),
                            Value = f.Id.ToString()
                        });
                }
            }
            model.ForumList = forumsSelectList;

            // Create the values for "Search within" select list            
            var withinList = new List<SelectListItem>
                                {
                                    new SelectListItem
                                    {
                                        Value = ((int)ForumSearchType.All).ToString(),
                                        Text = _localizationService.GetResource("Forum.Search.SearchWithin.All")
                                    },
                                    new SelectListItem
                                    {
                                        Value = ((int)ForumSearchType.TopicTitlesOnly).ToString(),
                                        Text = _localizationService.GetResource("Forum.Search.SearchWithin.TopicTitlesOnly")
                                    },
                                    new SelectListItem
                                    {
                                        Value = ((int)ForumSearchType.PostTextOnly).ToString(),
                                        Text = _localizationService.GetResource("Forum.Search.SearchWithin.PostTextOnly")
                                    }
                                };
            model.WithinList = withinList;

            int forumIdSelected;
            int.TryParse(forumId, out forumIdSelected);
            model.ForumIdSelected = forumIdSelected;

            int withinSelected;
            int.TryParse(within, out withinSelected);
            model.WithinSelected = withinSelected;

            int limitDaysSelected;
            int.TryParse(limitDays, out limitDaysSelected);
            model.LimitDaysSelected = limitDaysSelected;

            int searchTermMinimumLength = _forumSettings.ForumSearchTermMinimumLength;

            model.ShowAdvancedSearch = adv.GetValueOrDefault();
            model.SearchResultsVisible = false;
            model.NoResultsVisisble = false;
            model.PostsPageSize = _forumSettings.PostsPageSize;

            try
            {
                if (String.IsNullOrWhiteSpace(searchterms) == false)
                {
                    searchterms = searchterms.Trim();
                    model.SearchTerms = searchterms;

                    if (searchterms.Length < searchTermMinimumLength)
                    {
                        throw new WorkException(string.Format(_localizationService.GetResource("Forum.SearchTermMinimumLengthIsNCharacters"),
                            searchTermMinimumLength));
                    }

                    ForumSearchType searchWithin = 0;
                    int limitResultsToPrevious = 0;
                    if (adv.GetValueOrDefault() == true)
                    {
                        searchWithin = (ForumSearchType)withinSelected;
                        limitResultsToPrevious = limitDaysSelected;
                    }

                    if (_forumSettings.SearchResultsPageSize > 0)
                    {
                        pageSize = _forumSettings.SearchResultsPageSize;
                    }

                    var topics = _forumService.GetAllTopics(forumIdSelected, 0, searchterms, searchWithin,
                        limitResultsToPrevious, page - 1, pageSize);
                    model.TopicPageSize = topics.PageSize;
                    model.TopicTotalRecords = topics.TotalCount;
                    model.TopicPageIndex = topics.PageIndex;
                    foreach (var topic in topics)
                    {
                        var topicModel = PrepareForumTopicRowModel(topic);
                        model.ForumTopics.Add(topicModel);
                    }

                    model.SearchResultsVisible = (topics.Count > 0);
                    model.NoResultsVisisble = !(model.SearchResultsVisible);
                    
                    return View(model);
                }
                else
                {
                    model.SearchResultsVisible = false;
                }
            }
            catch (Exception ex)
            {
                model.Error = ex.Message;
            }

            //some exception raised
            model.TopicPageSize = pageSize;
            model.TopicTotalRecords = 0;
            model.TopicPageIndex = page - 1;

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult LastPost(int forumPostId, bool showTopic)
        {
            var post = _forumService.GetPostById(forumPostId);
            var model = new LastPostModel();
            if (post != null)
            {
                model.Id = post.Id;
                model.ForumTopicId = post.TopicId;
                model.ForumTopicSeName = post.ForumTopic.GetSeName();
                model.ForumTopicSubject = post.ForumTopic.StripTopicSubject();
                model.UserId = post.UserId;
                model.AllowViewingProfiles = _userSettings.AllowViewingProfiles;
                model.UserName = post.User.FormatUserName(true);
                model.IsUserGuest = post.User.IsGuest();
                //created on string
                if (_forumSettings.RelativeDateTimeFormattingEnabled)
                    model.PostCreatedOnStr = post.CreatedOnUtc.RelativeFormat(true, "f");
                else
                    model.PostCreatedOnStr = _dateTimeHelper.ConvertToUserTime(post.CreatedOnUtc, DateTimeKind.Utc).ToString("f");
            }
            model.ShowTopic = showTopic;
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ForumBreadcrumb(int? forumGroupId, int? forumId, int? forumTopicId)
        {
            var model = new ForumBreadcrumbModel();
            model.Separator = " / ";

            ForumTopic forumTopic = null;
            if (forumTopicId.HasValue)
            {
                forumTopic = _forumService.GetTopicById(forumTopicId.Value);
                if (forumTopic != null)
                {
                    model.ForumTopicId = forumTopic.Id;
                    model.ForumTopicSubject = forumTopic.Subject;
                    model.ForumTopicSeName = forumTopic.GetSeName();
                }
            }

            Forum forum = _forumService.GetForumById(forumTopic != null ? forumTopic.ForumId : (forumId.HasValue ? forumId.Value : 0));
            if (forum != null)
            {
                model.ForumId = forum.Id;
                model.ForumName = forum.Name;
                model.ForumSeName = forum.GetSeName();
            }

            var forumGroup = _forumService.GetForumGroupById(forum != null ? forum.ForumGroupId : (forumGroupId.HasValue ? forumGroupId.Value : 0));
            if (forumGroup != null)
            {
                model.ForumGroupId = forumGroup.Id;
                model.ForumGroupName = forumGroup.Name;
                model.ForumGroupSeName = forumGroup.GetSeName();
            }

            return PartialView(model);
        }

        #endregion
    }

}
