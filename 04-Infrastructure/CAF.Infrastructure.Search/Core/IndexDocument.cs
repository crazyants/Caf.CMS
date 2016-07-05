namespace CAF.Infrastructure.Search.Core {
    using System;
    using Lucene.Net.Documents;

    public class IndexDocument {
        private readonly Document _document;
        private string _key;

        internal Document Document {
            get {
                return _document;
            }
        }

        public IndexDocument() {
            _document = new Document();
        }

        public string Key {
            get {
                return _key;
            }
            set {
                AddField("key", value, FieldStore.Store, FieldIndex.IndexOnly);
                _key = value;
            }
        }

        public string Path {
            get {
                return GetFieldValue("path");
            }
            set {
                AddField("path", value, FieldStore.Store, FieldIndex.DontIndex);
            }
        }

        public string Title {
            get {
                return GetFieldValue("title");
            }
            set {
                AddField("title", value, FieldStore.Store, FieldIndex.Analyzed);
            }
        }

        protected string GetFieldValue(string fieldName) {
            Field field = _document.GetField(fieldName);
            if (field != null) {
                return field.StringValue;
            }
            return null;
        }

        protected long GetNumericFieldValue(string fieldName) {
            NumericField field = (NumericField)_document.GetFieldable(fieldName);
            if (field != null) {
                return (long)field.NumericValue;
            }
            return 0;
        }
        /// <summary>
        /// 创建索引 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="store"></param>
        /// <param name="index"></param>
        /// <param name="boost"></param>
        protected void AddField(string name, string value, FieldStore store, FieldIndex index, float? boost = null) {
            if(string.IsNullOrEmpty(value)) {
                return;
            }

            Field.Store fieldStore = GetFromStoreEnum(store);
            Field.Index fieldIndex = GetFromIndexEnum(index);

            var field = new Field(name, value, fieldStore, fieldIndex);

            if (boost != null) {
                field.Boost = (float)boost;
            }

            _document.Add(field);
        }

        protected void AddNumericField(string name, long value) {
            NumericField numericField = new NumericField(name, 4, Field.Store.YES, true);
            numericField.SetLongValue(value);

            _document.Add(numericField);
        }

        private Field.Index GetFromIndexEnum(FieldIndex index) {
            if(index==FieldIndex.Analyzed) {
                return Field.Index.ANALYZED;
            }
            if(index==FieldIndex.IndexOnly) {
                return Field.Index.NOT_ANALYZED;
            }

            return Field.Index.NO;
        }

        private Field.Store GetFromStoreEnum(FieldStore store) {
            if(store==FieldStore.Store) {
                return Field.Store.YES;
            }

            return Field.Store.NO;
        }

        protected DateTime ConvertStringToDate(string dateString) {
            return DateTools.StringToDate(dateString);
        }

        protected string ConvertDateToString(DateTime dateTime) {
            return DateTools.DateToString(dateTime, DateTools.Resolution.MINUTE);
        }

        protected DateTime ConvertLongToDate(long ticks) {
            return new DateTime(ticks);
        }

        protected DateTime ConvertLongToDate(string dateString) {
            long ticks;
            long.TryParse(dateString, out ticks);

            return new DateTime(ticks);
        }

        protected long ConvertDateToLong(DateTime dateTime) {
            return dateTime.Ticks;
        }
    }
}