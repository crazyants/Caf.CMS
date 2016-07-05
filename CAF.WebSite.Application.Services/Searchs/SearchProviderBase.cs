
using CAF.Infrastructure.Search.Core;
using System;
using System.Collections.ObjectModel;
namespace CAF.WebSite.Application.Services.Searchs
{


    public abstract class SearchProviderBase : ISearchProvider
    {
        /// <summary>
        /// 添加索引
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddToIndex(IndexItem item)
        { }
        /// <summary>
        /// 移除所有
        /// </summary>
        public virtual void RemoveAll() { }
        /// <summary>
        /// 移除索引
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="languageId"></param>
        public virtual void RemoveFromIndex(string itemId, int languageId) { }

        public virtual void RemoveFromIndex(Collection<string> itemIds, int languageId) { }

        public virtual void IndexingFinished() { }

        public virtual void Init() { }

        public virtual SearchResult Search(SearchQuery query)
        {
            throw new Exception("Can't search using NullSearchProvider, change to a valid provider in config.");
        }

        public virtual SearchResult FindSimular(IndexItem page, int resultOffset = 0, int resultSize = 10, bool matchCategory = true)
        {
            return FindSimular(page.ItemId, page.LanguageId, resultOffset, resultSize, matchCategory);
        }

        public virtual SearchResult FindSimular(string itemId, int languageId, int resultOffset = 0, int resultSize = 10, bool matchCategory = true)
        {
            throw new Exception("Can't search using NullSearchProvider, change to a valid provider in config.");
        }
        public void IndexPage(IndexItem indexItem)
        {

            AddToIndex(indexItem);

            IndexingFinished();
        }
    }
}
