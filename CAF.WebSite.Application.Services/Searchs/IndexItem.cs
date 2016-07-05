using System;
using System.Collections.Generic;
namespace CAF.WebSite.Application.Services.Searchs
{


    public class IndexItem
    {
        public string ItemId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
        public string Path { get; set; }
        public int LanguageId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public Dictionary<string, string> MetaData { get; set; }
        public DateTime? PublishStart { get; set; }
        public DateTime? PublishStop { get; set; }

        public IndexItem()
        {
            MetaData = new Dictionary<string, string>();
        }

        public void EnsureCorrectValues()
        {
            Title = Title ?? string.Empty;
        }
    }
}
