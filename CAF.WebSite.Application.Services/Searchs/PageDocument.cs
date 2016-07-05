using System;
using System.Collections.Generic;
using System.Globalization;
using HtmlAgilityPack;
using CAF.Infrastructure.Search.Core;

namespace CAF.WebSite.Application.Services.Searchs
{

    public class PageDocument : IndexDocument {
        public PageDocument(string key, IndexItem item) {
            Key = key;
            Category = item.Category;
            Content = item.Content;
            Created = item.Created;
            LanguageId = item.LanguageId;
            MetaData = item.MetaData;
            Modified = item.Modified;
            Path = item.Path;
            Title = item.Title;
            ItemId = item.ItemId;
            PublishStart = item.PublishStart ?? DateTime.MaxValue;
            PublishStop = item.PublishStop ?? DateTime.MaxValue;
        }

        public string ItemId
        {
            get { return GetFieldValue("itemid"); }
            set
            {
                AddField("itemid", value, FieldStore.Store, FieldIndex.Analyzed, 3.0f);
            }
        }

        protected DateTime PublishStop {
            get {
                return ConvertLongToDate(GetNumericFieldValue("publishStop"));
            }
            set {
                AddNumericField("publishStop", ConvertDateToLong(value));
            }
        }

        protected DateTime PublishStart {
            get {
                return ConvertLongToDate(GetNumericFieldValue("publishStart"));
            }
            set {
                AddNumericField("publishStart", ConvertDateToLong(value));
            }
        }
 
        protected DateTime Modified {
            get {
                return ConvertStringToDate(GetFieldValue("modified"));
            }
            set {
                AddField("modified", ConvertDateToString(value), FieldStore.Store, FieldIndex.DontIndex);
            }
        }

        protected int LanguageId {
            get {
                return int.Parse(GetFieldValue("languageId"));
            }
            set {
                AddField("languageId", value.ToString(CultureInfo.InvariantCulture), FieldStore.Store, FieldIndex.DontIndex);
            }
        }

        protected DateTime Created {
            get {
                return ConvertStringToDate(GetFieldValue("created"));
            }
            set {
                AddField("created", ConvertDateToString(value), FieldStore.Store, FieldIndex.DontIndex);
            }
        }

        protected string Content {
            get {
                return GetFieldValue("content");
            }
            set {
                //TODO: Bryt ut!!
                var doc = new HtmlDocument();
                doc.LoadHtml(value ?? string.Empty);
                var innerText = doc.DocumentNode.InnerText;

                AddField("content", innerText, FieldStore.Store, FieldIndex.Analyzed);
            }
        }

        protected string Category {
            get {
                return GetFieldValue("category");
            }
            set {
                AddField("category", value, FieldStore.Store, FieldIndex.Analyzed);
            }
        }


        // TODO: Replace dictionary with list containing store and index flags
        protected Dictionary<string, string> MetaData
        {
            set
            {
                foreach (var keyValuePair in value)
                {
                    AddField(keyValuePair.Key, keyValuePair.Value, FieldStore.Store, FieldIndex.Analyzed);
                }
            }
        }
    }
}
