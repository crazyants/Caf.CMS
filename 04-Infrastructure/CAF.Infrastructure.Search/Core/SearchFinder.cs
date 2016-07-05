namespace CAF.Infrastructure.Search.Core {
    using System;
    using System.IO;
    using Analyzers;
    using Lucene.Net.Analysis;
    using Lucene.Net.Documents;
    using Lucene.Net.Index;
    using Lucene.Net.QueryParsers;
    using Lucene.Net.Search;
    using Lucene.Net.Search.Highlight;
    using Lucene.Net.Search.Similar;
    using Version = Lucene.Net.Util.Version;

    public class SearchFinder : IDisposable {
        public const int MaxHits = 1000;
        private readonly Analyzer _analyzer;
        private Searcher _searcher;
        private IndexReader _reader;
        private bool _open;

        public SearchFinder(SearchIndexer indexer, Analyzer analyzer) {
            _analyzer = analyzer;
            _reader = indexer.IndexWriter.GetReader();
            CreateIndexer();
            _open = true;
        }

        private void CreateIndexer() {
            _searcher = new IndexSearcher(_reader);
        }

        public void Dispose() {
            if(_open) {
                Close();
            }
        }

        public void Close() {
            CloseSearcher();
            CloseReader();
            _open = false;
        }

        private void CloseSearcher() {
            _searcher.Dispose();
        }

        private void CloseReader() {
            _reader.Dispose();
        }

        public void Refresh() {
            IndexReader newIndexReader = _reader.Reopen();
            CloseReader();
            CloseSearcher();

            _reader = newIndexReader;
            CreateIndexer();
        }

        public SearchResult FindSimular(string key, int resultOffset, int resultLength, bool matchCategory) {
            var pageQuery = new TermQuery(new Term("key", key));
            var topDocs = _searcher.Search(pageQuery, 1);
            if (topDocs.TotalHits == 0) {
                return new SearchResult();
            }

            var doc = topDocs.ScoreDocs[0].Doc;

            var moreLikeThis = new MoreLikeThis(_reader) {
                Analyzer = _analyzer, 
                MinWordLen = 3
            };
            moreLikeThis.SetFieldNames(new[] { "title", "summary", "content", "tags" });
            moreLikeThis.SetStopWords(StopWords.DefaultEnglish);
            moreLikeThis.MinDocFreq = 2;
            
            var query = moreLikeThis.Like(doc);
            var startTime = DateTime.Now;
            var ticks = DateTime.Now.ToUniversalTime().Ticks;

            Query publishStartQuery = NumericRangeQuery.NewLongRange("publishStart", null, ticks, true, false);
            Query publishStopQuery = NumericRangeQuery.NewLongRange("publishStop", ticks, null, false, true);

            var booleanQuery = new BooleanQuery {
                {query, Occur.MUST},
                {pageQuery, Occur.MUST_NOT},
                {publishStartQuery, Occur.MUST},
                {publishStopQuery, Occur.MUST}
            };

            if (matchCategory) {
                var document = _searcher.Doc(doc);
                var field = document.GetField("category");

                if (field != null && !string.IsNullOrEmpty(field.StringValue)) {
                    var categoryQuery = new TermQuery(new Term("category", field.StringValue.ToLowerInvariant()));
                    booleanQuery.Add(categoryQuery, Occur.MUST);
                }
            }

            var scoreDocs = _searcher.Search(booleanQuery, null, MaxHits, Sort.RELEVANCE).ScoreDocs;

            var result = new SearchResult { NumberOfHits = scoreDocs.Length };

            if (resultOffset < scoreDocs.Length) {
                var resultUpperOffset = resultOffset + resultLength;
                if (resultUpperOffset > scoreDocs.Length) {
                    resultUpperOffset = scoreDocs.Length;
                }

                for (int i = resultOffset; i < resultUpperOffset; i++) {
                    Document document = _searcher.Doc(scoreDocs[i].Doc);

                    Guid pageId;
                    (document.Get("pageId") ?? string.Empty).TryParseGuid(out pageId);

                    var hit = new SearchHit {
                        PageId = pageId,
                        Path = document.Get("path"),
                        Title = document.Get("title"),
                        Excerpt = document.Get("summary")
                    };

                    //foreach (string key in metaData) {
                    //    hit.MetaData.Add(key, document.Get(key));
                    //}

                    result.Hits.Add(hit);
                }
            }

            var timeTaken = DateTime.Now - startTime;
            result.SecondsTaken = timeTaken.TotalSeconds;

            return result;
        }

        public SearchResult Search(string queryString, string[] searchFields, string[] metaData, int resultOffset, int resultLength) {
            if(resultOffset< 0) {
                resultOffset = 0;
            }

            if(resultLength < 1) {
                resultLength = 1;
            }

            var parser = new MultiFieldQueryParser(Version.LUCENE_30, searchFields, _analyzer) {DefaultOperator = QueryParser.Operator.AND};
            var query = ParseQuery(queryString, parser);

            return ExecuteQuery(metaData, resultOffset, resultLength, query);
        }

        private SearchResult ExecuteQuery(string[] metaData, int resultOffset, int resultLength, Query query) {
            var startTime = DateTime.Now;
            var ticks = DateTime.Now.ToUniversalTime().Ticks;

            //针对多个域的一次性查询
            //Query publishStartQuery = NumericRangeQuery.NewLongRange("publishStart", null, ticks, true, false);
            //Query publishStopQuery = NumericRangeQuery.NewLongRange("publishStop", ticks, null, false, true);

            var booleanQuery = new BooleanQuery {
                {query, Occur.MUST},
                //{publishStartQuery, Occur.MUST},
                //{publishStopQuery, Occur.MUST}
            };

            //TopDocs docs = _searcher.Search(query, (Filter)null, MaxHits);
            var scoreDocs = _searcher.Search(booleanQuery, null, MaxHits, Sort.RELEVANCE).ScoreDocs;
            var result = new SearchResult {NumberOfHits = scoreDocs.Length};

            // Create highlighter
            IFormatter formatter = new SimpleHTMLFormatter("<span class=\"search-highlight;\">", "</span>");
            var fragmenter = new SimpleFragmenter(120);
            var scorer = new QueryScorer(query);
            var highlighter = new Highlighter(formatter, scorer) {TextFragmenter = fragmenter};

            if (resultOffset < scoreDocs.Length) {
                var resultUpperOffset = resultOffset + resultLength;
                if (resultUpperOffset > scoreDocs.Length) {
                    resultUpperOffset = scoreDocs.Length;
                }

                for (var i = resultOffset; i < resultUpperOffset; i++) {
                    var doc = scoreDocs[i];
                    var document = _searcher.Doc(doc.Doc);
                    var content = document.Get("content");
                    var excerpt = "";

                    if (content != null) {
                        var stream = _analyzer.TokenStream("", new StringReader(document.Get("content")));
                        excerpt = highlighter.GetBestFragments(stream, document.Get("content"), 2, "...");
                    }

                    Guid pageId;
                    (document.Get("pageId") ?? string.Empty).TryParseGuid(out pageId);

                    var hit = new SearchHit {
                        PageId = pageId,
                        Path = document.Get("path"),
                        Title = document.Get("title"), 
                        Excerpt = excerpt
                    };


                    foreach (var key in metaData) {
                        hit.MetaData.Add(key, document.Get(key));
                    }

                    result.Hits.Add(hit);
                }
            }

            var timeTaken = DateTime.Now - startTime;
            result.SecondsTaken = timeTaken.TotalSeconds;

            return result;
        }

        private static Query ParseQuery(string searchQuery, QueryParser parser) {
            if(string.IsNullOrEmpty(searchQuery)) {
                return new TermQuery(new Term("path"));
            }

            Query query;
            try {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException) {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        } 
    }
}