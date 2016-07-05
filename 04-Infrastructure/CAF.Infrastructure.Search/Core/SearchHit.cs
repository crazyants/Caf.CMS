namespace CAF.Infrastructure.Search.Core {
    using System;
    using System.Collections.Generic;

    public class SearchHit {
        public SearchHit() {
            MetaData = new Dictionary<string, string>();
        }

        public Guid PageId { get;  set; }
        public string Title { get;  set; }
        public string Path { get;  set; }
        public string Excerpt { get;  set; }
        public Dictionary<string, string> MetaData { get;  set; }
    }
}