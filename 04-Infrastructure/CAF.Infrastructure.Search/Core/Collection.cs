namespace CAF.Infrastructure.Search.Core
{
    using System;
    using System.IO;
    using System.Web;
    using Configuration;
    using Lucene.Net.Analysis;
    using Lucene.Net.Store;
    using CAF.Infrastructure.Search.Analyzers;

    public class Collection : IDisposable
    {
        private readonly string _collectionName;
        private readonly SearchFinder _searchFinder;
        private readonly SearchIndexer _searchIndexer;
        private readonly IAnalyzer _analyzer;
        private readonly SearchSettings _setting;
        // TODO: Overload to allow usage of different parameters than stored in the configuration.
        public Collection(IAnalyzer analyzer,SearchSettings setting, string collectionName)
        {
            _collectionName = collectionName;
            this._analyzer = analyzer;
            this._setting = setting;
            var directory = GetDataDirectory();
            _searchIndexer = new SearchIndexer(directory, this._analyzer.Analyzer);
            _searchFinder = new SearchFinder(_searchIndexer, this._analyzer.Analyzer);
        }

        public void AddDocument(IndexDocument document)
        {
            _searchIndexer.AddDocument(document);
            _searchFinder.Refresh();
        }

        public void RemoveDocument(string key)
        {
            _searchIndexer.RemoveDocument(key);
            _searchFinder.Refresh();
        }

        public void RemoveAll()
        {
            _searchIndexer.RemoveAll();
            _searchFinder.Refresh();
        }

        public void OptimizeIndex()
        {
            _searchIndexer.OptimizeIndex();
            _searchFinder.Refresh();
        }

        public SearchResult Search(string queryString, string[] searchFields, string[] metaData, int resultOffset, int resultLength)
        {
            return _searchFinder.Search(queryString, searchFields, metaData, resultOffset, resultLength);
        }

        public SearchResult FindSimular(string key, int resultOffset = 0, int resultLength = 10, bool matchCategory = true)
        {
            return _searchFinder.FindSimular(key, resultOffset, resultLength, matchCategory);
        }

        private FSDirectory GetDataDirectory()
        {
            return FSDirectory.Open(new DirectoryInfo(DataDirectory), new NativeFSLockFactory());
        }

        private string DataDirectory
        {
            get
            {
                string dataStorePath = this._setting.DataStorePath;
                if (!dataStorePath.EndsWith("/"))
                {
                    dataStorePath += "/";
                }

                if (dataStorePath.StartsWith("/"))
                {
                    return HttpContext.Current.Server.MapPath(dataStorePath + _collectionName + "/");
                }
                else
                {
                    return dataStorePath + _collectionName + "/";
                }
            }
        }

        public void Dispose()
        {
            _searchIndexer.Close();
        }
    }
}