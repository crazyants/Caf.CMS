using CAF.WebSite.Application.WebUI.Events;
using CAF.Infrastructure.Core.Domain.Configuration;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Themes;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Cms.Polls;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.Infrastructure.Core.Domain.Cms.Topic;
using CAF.Infrastructure.Core.Domain.Cms.RegionalContents;


namespace CAF.WebSite.Application.WebUI
{

    public class ModelCacheEventConsumer :
        IConsumer<EntityInserted<ThemeVariable>>,
        IConsumer<EntityUpdated<ThemeVariable>>,
        IConsumer<EntityDeleted<ThemeVariable>>,
        IConsumer<EntityInserted<Language>>,
        IConsumer<EntityUpdated<Language>>,
        IConsumer<EntityDeleted<Language>>,
        //settings
        IConsumer<EntityUpdated<Setting>>,
        //categories
        IConsumer<EntityInserted<ArticleCategory>>,
        IConsumer<EntityUpdated<ArticleCategory>>,
        IConsumer<EntityDeleted<ArticleCategory>>,
        //articles
        IConsumer<EntityInserted<Article>>,
        IConsumer<EntityUpdated<Article>>,
        IConsumer<EntityDeleted<Article>>,
        //Pictures
        IConsumer<EntityInserted<Picture>>,
        IConsumer<EntityUpdated<Picture>>,
        IConsumer<EntityDeleted<Picture>>,
        //Article picture mapping
        IConsumer<EntityInserted<ArticleAlbum>>,
        IConsumer<EntityUpdated<ArticleAlbum>>,
        IConsumer<EntityDeleted<ArticleAlbum>>,
        //polls
        IConsumer<EntityInserted<Poll>>,
        IConsumer<EntityUpdated<Poll>>,
        IConsumer<EntityDeleted<Poll>>,
        //news items
        IConsumer<EntityInserted<Topic>>,
        IConsumer<EntityUpdated<Topic>>,
        IConsumer<EntityDeleted<Topic>>,
        //states/province
        IConsumer<EntityInserted<StateProvince>>,
        IConsumer<EntityUpdated<StateProvince>>,
        IConsumer<EntityDeleted<StateProvince>>,
        //customer roles
        IConsumer<EntityUpdated<UserRole>>,
        IConsumer<EntityDeleted<UserRole>>,
        //stores
        IConsumer<EntityUpdated<Site>>,
        IConsumer<EntityDeleted<Site>>,
        //RegionalContent
        IConsumer<EntityInserted<RegionalContent>>,
        IConsumer<EntityUpdated<RegionalContent>>,
        IConsumer<EntityDeleted<RegionalContent>>,

        IConsumer<ThemeTouched>
    {
        /// <summary>
        /// Key for ManufacturerNavigationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : current manufacturer id
        /// {1} : language id
        /// {2} : current store ID
        /// </remarks>
        public const string CLIENT_NAVIGATION_MODEL_KEY = "caf.pres.client.navigation-{0}-{1}-{2}";
        public const string CLIENT_NAVIGATION_PATTERN_KEY = "caf.pres.client.navigation";
        /// <summary>
        /// Key for manufacturer picture caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized manufacturer name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string CLIENT_PICTURE_MODEL_KEY = "caf.pres.client.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CLIENT_PICTURE_PATTERN_KEY = "caf.pres.client.picture";

        /// <summary>
        /// Key for states by country id
        /// </summary>
        /// <remarks>
        /// {0} : country ID
        /// {1} : addEmptyStateIfRequired value
        /// {2} : language ID
        /// </remarks>
        public const string STATEPROVINCES_BY_COUNTRY_MODEL_KEY = "caf.pres.stateprovinces.bycountry-{0}-{1}-{2}";
        public const string STATEPROVINCES_PATTERN_KEY = "caf.pres.stateprovinces.";

        /// <summary>
        /// Key for ThemeVariables caching
        /// </summary>
        /// <remarks>
        /// {0} : theme name
        /// {1} : site identifier
        /// </remarks>
        public const string THEMEVARS_LESSCSS_KEY = "caf.pres.themevars-lesscss-{0}-{1}";
        public const string THEMEVARS_LESSCSS_THEME_KEY = "caf.pres.themevars-lesscss-{0}";


        /// <summary>
        /// Key for tax display type caching
        /// </summary>
        /// <remarks>
        /// {0} : user role ids
        /// {1} : site identifier
        /// </remarks>
        public const string USERRROLES_TAX_DISPLAY_TYPES_KEY = "caf.fw.userroles.taxdisplaytypes-{0}-{1}";
        public const string USERRROLES_TAX_DISPLAY_TYPES_PATTERN_KEY = "caf.fw.userroles.taxdisplaytypes";

        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic id
        /// {1} : language id
        /// {2} : site id
        /// </remarks>
        public const string TOPIC_MODEL_KEY = "caf.pres.topic.details-{0}-{1}-{2}";
        public const string TOPIC_PATTERN_KEY = "caf.pres.topic.details";
        /// <summary>
        /// Key for TopicWidget caching
        /// </summary>
        /// <remarks>
        /// {0} : site id
        /// {1} : language id
        /// </remarks>
        public const string TOPIC_WIDGET_PATTERN_KEY = "caf.pres.topic.widget";
        public const string TOPIC_WIDGET_ALL_MODEL_KEY = "caf.pres.topic.widget-all-{0}-{1}";
        /// <summary>
        /// Key for LinkModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : site id
        /// </remarks>
        public const string LINK_MODEL_KEY = "caf.pres.link.details-{0}-{1}";
        public const string LINK_PATTERN_KEY = "caf.pres.link.details";
        /// <summary>
        /// Key for category picture caching
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized category name)
        /// {4} : is connection SSL secured?
        /// {5} : current site ID
        /// </remarks>
        public const string CATEGORY_PICTURE_MODEL_KEY = "caf.pres.articlecategory.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CATEGORY_PICTURE_PATTERN_KEY = "caf.pres.articlecategory.picture";

        /// <summary>
        /// Key for CategoryNavigationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : comma separated list of customer roles
        /// {2} : current store id
        /// </remarks>
        public const string CATEGORY_NAVIGATION_MODEL_KEY = "caf.pres.category.navigation-{0}-{1}-{2}";
        public const string CATEGORY_NAVIGATION_PATTERN_KEY = "caf.pres.category.navigation";
        /// <summary>
        /// Key for GetChildCategoryIds method results caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category id
        /// {1} : show hidden?
        /// {2} : comma separated list of customer roles
        /// {3} : current store id
        /// </remarks>
        public const string CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY = "caf.pres.category.childidentifiers-{0}-{1}-{2}-{3}";
        public const string CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY = "caf.pres.category.childidentifiers";
        /// <summary>
        /// Key for site header data
        /// </summary>
        /// <remarks>
        /// {0} : current site ID
        /// </remarks>
        public const string SITEHEADER_MODEL_KEY = "caf.pres.shopheader-{0}";
        public const string SITEHEADER_MODEL_PATTERN_KEY = "caf.pres.shopheader";
        /// <summary>
        /// Key for modelTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : category template id
        /// </remarks>
        public const string ARTICLECATEGORY_TEMPLATE_MODEL_KEY = "caf.pres.ModelTemplate-{0}";
        public const string ARTICLECATEGORY_TEMPLATE_PATTERN_KEY = "caf.pres.ModelTemplate";

        /// <summary>
        /// Key for ArticleTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : article template id
        /// </remarks>
        public const string ARTICLE_TEMPLATE_MODEL_KEY = "caf.pres.articletemplate-{0}";
        public const string ARTICLE_TEMPLATE_PATTERN_KEY = "caf.pres.articletemplate";
        /// <summary>
        /// Key for ArticleTagModel caching
        /// </summary>
        /// <remarks>
        /// {0} : article id
        /// {1} : language id
        /// {2} : current store ID
        /// </remarks>
        public const string ARTICLETAG_BY_ARTICLE_MODEL_KEY = "caf.pres.articletag.byarticle-{0}-{1}-{2}";
        public const string ARTICLETAG_BY_ARTICLE_PATTERN_KEY = "caf.pres.articletag.byarticle";
        /// <summary>
        /// Key for PopularArticleTagsModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// </remarks>`
        public const string ARTICLETAG_POPULAR_MODEL_KEY = "caf.pres.articletag.popular-{0}-{1}";
        public const string ARTICLETAG_POPULAR_PATTERN_KEY = "caf.pres.articletag.popular";
        /// <summary>
        /// Key for default product picture caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string ARTICLE_DEFAULTPICTURE_MODEL_KEY = "caf.pres.article.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string ARTICLE_DEFAULTPICTURE_PATTERN_KEY = "caf.pres.article.picture";

        /// <summary>
        /// Key for home page news
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : store ID
        /// {2} : category ID
        /// {3} : partview
        /// </remarks>
        public const string HOMEPAGE_TOPARTICLESMODEL_KEY = "caf.pres.articles.homepage-{0}-{1}-{2}";
        public const string HOMEPAGE_TOPARTICLESMODEL_PARTVIEW_KEY = "caf.pres.articles.homepage-{0}-{1}-{2}-{3}";
        public const string ARTICLES_PATTERN_KEY = "caf.pres.articles.";
        /// <summary>
        /// Key for sitemap
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current user roles
        /// {2} : current store id
        /// </remarks>
        public const string SITEMAP_PAGE_MODEL_KEY = "caf.pres.sitemap.page-{0}-{1}-{2}";

        /// <summary>
        /// Key for seo sitemap
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current user roles
        /// {2} : current store id
        /// </remarks>
        public const string SITEMAP_XML_MODEL_KEY = "caf.pres.sitemap.xml-{0}-{1}-{2}";
        public const string SITEMAP_PATTERN_KEY = "caf.pres.sitemap";

        /// <summary>
        /// Key for available languages
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        public const string AVAILABLE_LANGUAGES_MODEL_KEY = "caf.pres.languages.all-{0}";
        public const string AVAILABLE_LANGUAGES_PATTERN_KEY = "caf.pres.languages.";

        /// <summary>
        /// Key for RegionalContentModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : site id
        /// </remarks>
        public const string REGIONALCONTENT_MODEL_KEY = "caf.pres.regionalcontent.details-{0}-{1}-{2}";
        public const string REGIONALCONTENT_PATTERN_KEY = "caf.pres.regionalcontent.details";
        /// <summary>
        /// Key for home page polls
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public const string HOMEPAGE_POLLS_MODEL_KEY = "caf.pres.poll.homepage-{0}-{1}";
        /// <summary>
        /// Key for polls by system name
        /// </summary>
        /// <remarks>
        /// {0} : poll system name
        /// {1} : language ID
        /// {2} : current store ID
        /// </remarks>
        public const string POLL_BY_SYSTEMNAME_MODEL_KEY = "caf.pres.poll.systemname-{0}-{1}-{2}";
        public const string POLLS_PATTERN_KEY = "caf.pres.poll.";

        public const string STORE_LANGUAGE_MAP_KEY = "caf.fw.sitelangmap";


        private readonly ICacheManager _cacheManager;
        private readonly ICacheManager _aspCache;

        public ModelCacheEventConsumer(Func<string, ICacheManager> cache)
        {
            this._cacheManager = cache("static");
            this._aspCache = cache("aspnet");
        }

        public void HandleEvent(EntityInserted<ThemeVariable> eventMessage)
        {
            _aspCache.Remove(BuildThemeVarsCacheKey(eventMessage.Entity));
        }

        public void HandleEvent(EntityUpdated<ThemeVariable> eventMessage)
        {
            _aspCache.Remove(BuildThemeVarsCacheKey(eventMessage.Entity));
        }

        public void HandleEvent(EntityDeleted<ThemeVariable> eventMessage)
        {
            _aspCache.Remove(BuildThemeVarsCacheKey(eventMessage.Entity));
        }

        public void HandleEvent(ThemeTouched eventMessage)
        {
            var cacheKey = BuildThemeVarsCacheKey(eventMessage.ThemeName, 0);
            _aspCache.RemoveByPattern(cacheKey);
        }



        public void HandleEvent(EntityInserted<Language> eventMessage)
        {
            _cacheManager.Remove(STORE_LANGUAGE_MAP_KEY);
        }

        public void HandleEvent(EntityUpdated<Language> eventMessage)
        {
            _cacheManager.Remove(STORE_LANGUAGE_MAP_KEY);
        }

        public void HandleEvent(EntityDeleted<Language> eventMessage)
        {
            _cacheManager.Remove(STORE_LANGUAGE_MAP_KEY);
        }

        public void HandleEvent(EntityUpdated<Setting> eventMessage)
        {
            // clear models which depend on settings
            _cacheManager.RemoveByPattern(USERRROLES_TAX_DISPLAY_TYPES_PATTERN_KEY); // depends on TaxSettings.TaxDisplayType
        }

        //categories
        public void HandleEvent(EntityInserted<ArticleCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(HOMEPAGE_TOPARTICLESMODEL_KEY);
            _cacheManager.RemoveByPattern(HOMEPAGE_TOPARTICLESMODEL_PARTVIEW_KEY);

        }
        public void HandleEvent(EntityUpdated<ArticleCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(HOMEPAGE_TOPARTICLESMODEL_KEY);
            _cacheManager.RemoveByPattern(HOMEPAGE_TOPARTICLESMODEL_PARTVIEW_KEY);
        }
        public void HandleEvent(EntityDeleted<ArticleCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(HOMEPAGE_TOPARTICLESMODEL_KEY);
            _cacheManager.RemoveByPattern(HOMEPAGE_TOPARTICLESMODEL_PARTVIEW_KEY);
        }

        //products
        public void HandleEvent(EntityInserted<Article> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(HOMEPAGE_TOPARTICLESMODEL_KEY);
            _cacheManager.RemoveByPattern(HOMEPAGE_TOPARTICLESMODEL_PARTVIEW_KEY);
        }
        public void HandleEvent(EntityUpdated<Article> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(HOMEPAGE_TOPARTICLESMODEL_KEY);
            _cacheManager.RemoveByPattern(HOMEPAGE_TOPARTICLESMODEL_PARTVIEW_KEY);
        }
        public void HandleEvent(EntityDeleted<Article> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(HOMEPAGE_TOPARTICLESMODEL_KEY);
            _cacheManager.RemoveByPattern(HOMEPAGE_TOPARTICLESMODEL_PARTVIEW_KEY);
        }

        //product tags
        public void HandleEvent(EntityInserted<ArticleTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLETAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLETAG_BY_ARTICLE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ArticleTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLETAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLETAG_BY_ARTICLE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ArticleTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLETAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLETAG_BY_ARTICLE_PATTERN_KEY);
        }

        //Pictures
        public void HandleEvent(EntityInserted<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_PICTURE_PATTERN_KEY);

        }
        public void HandleEvent(EntityUpdated<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_PICTURE_PATTERN_KEY);

        }
        public void HandleEvent(EntityDeleted<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_PICTURE_PATTERN_KEY);

        }

        //Article picture mappings
        public void HandleEvent(EntityInserted<ArticleAlbum> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_DEFAULTPICTURE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ArticleAlbum> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_DEFAULTPICTURE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ArticleAlbum> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_DEFAULTPICTURE_PATTERN_KEY);
        }

        //Polls
        public void HandleEvent(EntityInserted<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
        }


        //Topic
        public void HandleEvent(EntityInserted<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_MODEL_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_MODEL_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_MODEL_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
        }

        //State/province
        public void HandleEvent(EntityInserted<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }

        //customer roles
        public void HandleEvent(EntityUpdated<UserRole> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<UserRole> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
        }

        //stores
        public void HandleEvent(EntityUpdated<Site> eventMessage)
        {
            _cacheManager.RemoveByPattern(SITEHEADER_MODEL_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Site> eventMessage)
        {
            _cacheManager.RemoveByPattern(SITEHEADER_MODEL_PATTERN_KEY);
        }

        //RegionalContent
        public void HandleEvent(EntityInserted<RegionalContent> eventMessage)
        {
            _cacheManager.RemoveByPattern(REGIONALCONTENT_MODEL_KEY);
            _cacheManager.RemoveByPattern(REGIONALCONTENT_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<RegionalContent> eventMessage)
        {
            _cacheManager.RemoveByPattern(REGIONALCONTENT_MODEL_KEY);
            _cacheManager.RemoveByPattern(REGIONALCONTENT_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<RegionalContent> eventMessage)
        {
            _cacheManager.RemoveByPattern(REGIONALCONTENT_MODEL_KEY);
            _cacheManager.RemoveByPattern(REGIONALCONTENT_PATTERN_KEY);
        }


        #region Helpers

        public static string BuildThemeVarsCacheKey(ThemeVariable entity)
        {
            return BuildThemeVarsCacheKey(entity.Theme, entity.SiteId);
        }

        public static string BuildThemeVarsCacheKey(string themeName, int siteId)
        {
            if (siteId > 0)
            {
                return THEMEVARS_LESSCSS_KEY.FormatInvariant(themeName, siteId);
            }

            return THEMEVARS_LESSCSS_THEME_KEY.FormatInvariant(themeName);
        }

        #endregion

    }

}
