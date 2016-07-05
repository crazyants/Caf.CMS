using CAF.Infrastructure.Core.Configuration;
using System;
using System.Configuration;
using Version = Lucene.Net.Util.Version;

namespace CAF.Infrastructure.Search.Configuration
{
    public class SearchSettings : ISettings
    {
        public SearchSettings()
        {
            this.DataStorePath = "|DataDirectory|\\SearchIndex\\CWSharp";
            this.Language = "Language";
            this.LuceneVersion = Version.LUCENE_30;
        }

        private string _dataStorePath;
        public string DataStorePath
        {
            get
            {
                return _dataStorePath.Replace("|DataDirectory|", (string)AppDomain.CurrentDomain.GetData("DataDirectory"));
            }
            set
            {
                _dataStorePath = value;
            }
        }

        public Version LuceneVersion { get; set; }
        public string Language { get; set; }
    }
}