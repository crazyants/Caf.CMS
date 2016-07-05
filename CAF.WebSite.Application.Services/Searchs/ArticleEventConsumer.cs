
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.Services.Searchs;
using CAF.WebSite.Application.Services.Helpers;
using System;
using System.Web.Mvc;

namespace CAF.WebSite.Application.Services.Searchs
{
    public class ArticleEventConsumer : IConsumer<ArticleEvent>
    {
        private readonly IPluginFinder _pluginFinder;
        private readonly IDbContext _dbContext;
        private readonly ISearchProvider _searchProvide;
        private readonly IDateTimeHelper _dateTimeHelper;
        public ArticleEventConsumer(
                IPluginFinder pluginFinder,
                IDbContext dbContext,
                ISearchProvider searchProvide,
                IDateTimeHelper dateTimeHelper)
        {

            this._pluginFinder = pluginFinder;
            this._dbContext = dbContext;
            this._searchProvide = searchProvide;
            this._dateTimeHelper = dateTimeHelper;
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(ArticleEvent eventMessage)
        {
            this._searchProvide.IndexPage(MakeIndexItem(eventMessage));
        }

        private IndexItem MakeIndexItem(ArticleEvent articleEvent)
        {
            var article = articleEvent.Article;
            var indexItem = new IndexItem();
            indexItem.Created = _dateTimeHelper.ConvertToUserTime(article.CreatedOnUtc, DateTimeKind.Utc);
            indexItem.Modified = article.ModifiedOnUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(article.ModifiedOnUtc.Value, DateTimeKind.Utc) : DateTime.Now;
            indexItem.Path = articleEvent.Url;
            indexItem.ItemId = article.Id.ToString();
            indexItem.Title = article.Title;
            indexItem.Content = article.ShortContent + " " + article.FullContent;

            indexItem.Category = article.ArticleCategory.Name;

            return indexItem;
        }
    }
}