namespace CAF.Infrastructure.Search.Core {
    using System;
    using Analyzers;
    using Configuration;
    using Lucene.Net.Analysis;

    public class SearchAnalyzer {
        public static Analyzer GetConfiguredAnalyzer() {
            var analyzerTypeName = SearchSettings.Instance.Analyzer;

            Type analyzerType = Type.GetType(analyzerTypeName);

            if (analyzerType == null) {
                throw new NullReferenceException(string.Format("Type.GetType('{0}') returned null.", analyzerTypeName));
            }

            if (!typeof(IAnalyzer).IsAssignableFrom(analyzerType)) {
                throw new NotSupportedException(string.Format("Type '{0}') does not implement IAnalyzer.", analyzerTypeName));
            }

            var analyzer = (IAnalyzer)Activator.CreateInstance(analyzerType);

            if (analyzer == null) {
                throw new Exception(string.Format("Can't create instance of type '{0}'.", analyzerTypeName));
            }

            return analyzer.Analyzer;
        }
    }
}