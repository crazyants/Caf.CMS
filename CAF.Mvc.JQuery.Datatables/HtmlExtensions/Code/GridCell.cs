

using Newtonsoft.Json;

namespace CAF.Mvc.JQuery.Datatables.Core
{
    [JsonObject]
    public class GridCell
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("cell")]
        public string[] Cell { get; set; }
    }
}