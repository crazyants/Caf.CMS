namespace CAF.Infrastructure.Search.Core {
    using System.Collections.Generic;

    public class SearchResult {
        public SearchResult() {
            Hits = new List<SearchHit>();
        }

        public int NumberOfHits { get;  set; }
        public double SecondsTaken { get;  set; }
        public List<SearchHit> Hits { get;  set; }
    }
}