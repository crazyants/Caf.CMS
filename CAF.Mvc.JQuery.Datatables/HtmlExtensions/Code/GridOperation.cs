

using Newtonsoft.Json;

namespace CAF.Mvc.JQuery.Datatables.Core
{
    public class GridOperation
    {
        private string _edittext;
        private string _addtext;
        private string _searchtext;
        private string _deltext;
        private string _refreshtext;

        [JsonProperty("edit")]
        public bool Edit { get; set; }

        [JsonProperty("edittext")]
        public string EditText
        {
            get { return _edittext ?? "编辑"; }
            set { _edittext = value; }
        }

        [JsonProperty("add")]
        public bool Add { get; set; }

        [JsonProperty("addtext")]
        public string AddText
        {
            get { return _addtext ?? "添加"; }
            set { _addtext = value; }
        }

        [JsonProperty("del")]
        public bool Delete { get; set; }

        [JsonProperty("deltext")]
        public string DeleteText
        {
            get { return _deltext ?? "删除"; }
            set { _deltext = value; }
        }

        [JsonProperty("search")]
        public bool Search { get; set; }

        [JsonProperty("searchtext")]
        public string SearchText
        {
            get { return _searchtext ?? "查找"; }
            set { _searchtext = value; }
        }

        [JsonProperty("refresh")]
        public bool Refresh { get; set; }

        public string RefreshText
        {
            get { return _refreshtext ?? "刷新"; }
            set { _refreshtext = value; }
        }
    }
}