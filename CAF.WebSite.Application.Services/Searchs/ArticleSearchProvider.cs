using CAF.Infrastructure.Search.Analyzers;
using CAF.Infrastructure.Search.Configuration;
using CAF.Infrastructure.Search.Core;
using System;
using System.Collections.ObjectModel;
namespace CAF.WebSite.Application.Services.Searchs
{

    public class ArticleSearchProvider : SearchProviderBase
    {
        private readonly Collection _collection;
        private static readonly string[] SearchFields = new[] { "title", "content", "category", "tags" };
        private readonly IAnalyzer _analyzer;
        private readonly SearchSettings _setting;
        public ArticleSearchProvider(IAnalyzer analyzer, SearchSettings setting)
        {
            _collection = new Collection(analyzer,setting,"BasCMS");
        }

        public override void AddToIndex(IndexItem item)
        {
            var key = GetKey(item.ItemId, item.LanguageId);
            var pageDocument = new PageDocument(key, item);
            _collection.RemoveDocument(key);
            _collection.AddDocument(pageDocument);

            IndexingFinished();
        }

        public override void RemoveAll()
        {
            _collection.RemoveAll();
        }

        public override void RemoveFromIndex(string itemId, int languageId)
        {
            string key = GetKey(itemId, languageId);
            _collection.RemoveDocument(key);
        }

        public override void RemoveFromIndex(Collection<string> itemIds, int languageId)
        {
            foreach (var itemId in itemIds)
            {
                string key = GetKey(itemId, languageId);
                _collection.RemoveDocument(key);
            }
        }

        public override void IndexingFinished()
        {
            _collection.OptimizeIndex();
        }

        private string GetKey(string itemId, int languageId)
        {
            string key = string.Format("{0}:{1}", itemId, languageId);
            return key;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init()
        {
            // IndexPage();
        }


        public override SearchResult Search(SearchQuery query)
        {
            var searchResult = _collection.Search(query.SearchString, SearchFields, query.MetaData, query.ReturnFromPosition, query.NumberOfHitsToReturn);
            var result = ConvertResult(searchResult);

            return result;
        }

        public override SearchResult FindSimular(string itemId, int languageId, int resultOffset = 0, int resultSize = 10, bool matchCategory = true)
        {
            var key = GetKey(itemId, languageId);
            var searchResult = _collection.FindSimular(key, resultOffset, resultSize, matchCategory);
            var result = ConvertResult(searchResult);

            return result;
        }

        private SearchResult ConvertResult(SearchResult searchResult)
        {
            var result = new SearchResult
            {
                NumberOfHits = searchResult.NumberOfHits,
                SecondsTaken = searchResult.SecondsTaken
            };

            foreach (var hit in searchResult.Hits)
            {
                result.Hits.Add(new SearchHit
                {
                    Excerpt = hit.Excerpt,
                    Path = hit.Path,
                    Title = hit.Title,
                    MetaData = hit.MetaData,
                    PageId = hit.PageId
                });
            }

            return result;
        }

    }
}
