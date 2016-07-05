
namespace CAF.WebSite.Application.Services.Searchs
{
    public class SearchQuery {
        public SearchQuery(string searchString) {
            SearchString = searchString;
            ReturnFromPosition = 0;
            NumberOfHitsToReturn = 100;
            MetaData = new string[] {};
        }

        public int ReturnFromPosition { get; set; }
        public int NumberOfHitsToReturn { get; set; }
        public string SearchString { get; set; }
        public string[] MetaData { get; set; }
    }
}
