using CAF.Infrastructure.Core.IO.VirtualPath;
using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Caching;
using System.Web.Routing;
using System.Xml;
using CAF.Infrastructure.Core.IO;

namespace CAF.WebSite.Application.WebUI
{
    public class XmlSiteMap : SiteMapBase
    {
        private const string SiteMapNodeName = "siteMapNode";
        private const string RouteValuesNodeName = "routeValues";
        private const string TitleAttributeName = "title";
        private const string VisibleAttributeName = "visible";
        private const string RouteAttributeName = "route";
        private const string ControllerAttributeName = "controller";
        private const string ActionAttributeName = "action";
        private const string UrlAttributeName = "url";
        private const string LastModifiedAttributeName = "lastModifiedAt";
        private const string ChangeFrequencyAttributeName = "changeFrequency";
        private const string UpdatePriorityAttributeName = "updatePriority";
        private const string IncludeInSearchEngineIndexAttributeName = "includeInSearchEngineIndex";
        private const string AreaAttributeName = "area";
        private static readonly string[] knownAttributes = XmlSiteMap.CreateKnownAttributes();
        private readonly ICacheManager cacheProvider;
      
        private static string defaultPath = "~/Web.sitemap";
        public static string DefaultPath
        {
            get
            {
                return XmlSiteMap.defaultPath;
            }
            set
            {

                XmlSiteMap.defaultPath = value;
            }
        }
        public XmlSiteMap(ICacheManager cacheManager)
        {
            this.cacheProvider = cacheManager;
        }
        public void Load()
        {
            this.LoadFrom(XmlSiteMap.DefaultPath);
        }
        public virtual void LoadFrom(string relativeVirtualPath)
        {

            if (!string.IsNullOrEmpty(relativeVirtualPath))
            {
                this.InternalLoad(relativeVirtualPath);
            }
        }
        internal void InsertInCache(string filePath)
        {
            string key = base.GetType().AssemblyQualifiedName + ":" + filePath;
            cacheProvider.Get(key, () =>
               {
                   return new string[] { };
               });

        }
        internal virtual void InternalLoad(string physicalPath)
        {

            string text = CFiles.ReadFile(physicalPath);
            if (!string.IsNullOrEmpty(text))
            {
                using (StringReader stringReader = new StringReader(text))
                {
                    using (XmlReader xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings
                    {
                        CloseInput = true,
                        IgnoreWhitespace = true,
                        IgnoreComments = true,
                        IgnoreProcessingInstructions = true
                    }))
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(xmlReader);
                        this.Reset();
                        if (xmlDocument.DocumentElement != null && xmlDocument.HasChildNodes)
                        {
                            base.CacheDurationInMinutes = XmlSiteMap.GetFloatValueFromAttribute(xmlDocument.DocumentElement, "cacheDurationInMinutes", SiteMapBase.DefaultCacheDurationInMinutes);
                            base.Compress = XmlSiteMap.GetBooleanValueFromAttribute(xmlDocument.DocumentElement, "compress", true);
                            base.GenerateSearchEngineMap = XmlSiteMap.GetBooleanValueFromAttribute(xmlDocument.DocumentElement, "generateSearchEngineMap", true);
                            XmlNode firstChild = xmlDocument.DocumentElement.FirstChild;
                            XmlSiteMap.Iterate(base.RootNode, firstChild);
                            this.InsertInCache(physicalPath);
                        }
                    }
                }
            }
        }
        internal void OnCacheItemRemoved(string key, object value, CacheItemRemovedReason reason)
        {
            if (reason == CacheItemRemovedReason.DependencyChanged)
            {
                this.InternalLoad((string)value);
            }
        }
        private static void Iterate(SiteMapNode siteMapNode, XmlNode xmlNode)
        {
            XmlSiteMap.PopulateNode(siteMapNode, xmlNode);
            foreach (XmlNode xmlNode2 in xmlNode.ChildNodes)
            {
                if (xmlNode2.LocalName.IsCaseSensitiveEqual("siteMapNode"))
                {
                    SiteMapNode siteMapNode2 = new SiteMapNode();
                    siteMapNode.ChildNodes.Add(siteMapNode2);
                    XmlSiteMap.Iterate(siteMapNode2, xmlNode2);
                }
            }
        }
        private static void PopulateNode(SiteMapNode siteMapNode, XmlNode xmlNode)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            XmlNode firstChild = xmlNode.FirstChild;
            if (firstChild != null && firstChild.LocalName.IsCaseSensitiveEqual("routeValues"))
            {
                foreach (XmlNode xmlNode2 in firstChild.ChildNodes)
                {
                    routeValueDictionary[xmlNode2.LocalName] = xmlNode2.InnerText;
                }
            }
            siteMapNode.Title = XmlSiteMap.GetStringValueFromAttribute(xmlNode, "title");
            siteMapNode.Visible = XmlSiteMap.GetBooleanValueFromAttribute(xmlNode, "visible", true);
            string stringValueFromAttribute = XmlSiteMap.GetStringValueFromAttribute(xmlNode, "route");
            string stringValueFromAttribute2 = XmlSiteMap.GetStringValueFromAttribute(xmlNode, "controller");
            string stringValueFromAttribute3 = XmlSiteMap.GetStringValueFromAttribute(xmlNode, "action");
            string stringValueFromAttribute4 = XmlSiteMap.GetStringValueFromAttribute(xmlNode, "url");
            string stringValueFromAttribute5 = XmlSiteMap.GetStringValueFromAttribute(xmlNode, "area");
            if (stringValueFromAttribute5 != null)
            {
                routeValueDictionary["area"] = stringValueFromAttribute5;
            }
            if (!string.IsNullOrEmpty(stringValueFromAttribute))
            {
                siteMapNode.RouteName = stringValueFromAttribute;
                siteMapNode.RouteValues.Clear();
                siteMapNode.RouteValues.Merge(routeValueDictionary);
            }
            else
            {
                if (!string.IsNullOrEmpty(stringValueFromAttribute2) && !string.IsNullOrEmpty(stringValueFromAttribute3))
                {
                    siteMapNode.ControllerName = stringValueFromAttribute2;
                    siteMapNode.ActionName = stringValueFromAttribute3;
                    siteMapNode.RouteValues.Clear();
                    siteMapNode.RouteValues.Merge(routeValueDictionary);
                }
                else
                {
                    if (!string.IsNullOrEmpty(stringValueFromAttribute4))
                    {
                        siteMapNode.Url = stringValueFromAttribute4;
                    }
                }
            }
            DateTime? dateValueFromAttribute = XmlSiteMap.GetDateValueFromAttribute(xmlNode, "lastModifiedAt");
            if (dateValueFromAttribute.HasValue)
            {
                siteMapNode.LastModifiedAt = new DateTime?(dateValueFromAttribute.Value);
            }
            siteMapNode.IncludeInSearchEngineIndex = XmlSiteMap.GetBooleanValueFromAttribute(xmlNode, "includeInSearchEngineIndex", true);
            foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
            {
                if (!string.IsNullOrEmpty(xmlAttribute.LocalName) && Array.BinarySearch<string>(XmlSiteMap.knownAttributes, xmlAttribute.LocalName, StringComparer.OrdinalIgnoreCase) < 0)
                {
                    siteMapNode.Attributes.Add(xmlAttribute.LocalName, xmlAttribute.Value);
                }
            }
        }
        private static string GetStringValueFromAttribute(XmlNode node, string attributeName)
        {
            string result = null;
            if (node.Attributes.Count > 0)
            {
                XmlAttribute xmlAttribute = node.Attributes[attributeName];
                if (xmlAttribute != null)
                {
                    result = xmlAttribute.Value;
                }
            }
            return result;
        }
        private static bool GetBooleanValueFromAttribute(XmlNode node, string attributeName, bool defaultValue)
        {
            bool result = defaultValue;
            string stringValueFromAttribute = XmlSiteMap.GetStringValueFromAttribute(node, attributeName);
            bool flag;
            if (!string.IsNullOrEmpty(stringValueFromAttribute) && bool.TryParse(stringValueFromAttribute, out flag))
            {
                result = flag;
            }
            return result;
        }
        private static float GetFloatValueFromAttribute(XmlNode node, string attributeName, float defaultValue)
        {
            float result = defaultValue;
            string stringValueFromAttribute = XmlSiteMap.GetStringValueFromAttribute(node, attributeName);
            float num;
            if (!string.IsNullOrEmpty(stringValueFromAttribute) && float.TryParse(stringValueFromAttribute, out num))
            {
                result = num;
            }
            return result;
        }
        private static DateTime? GetDateValueFromAttribute(XmlNode node, string attributeName)
        {
            string stringValueFromAttribute = XmlSiteMap.GetStringValueFromAttribute(node, attributeName);
            DateTime? result = null;
            DateTime value;
            if (!string.IsNullOrEmpty(stringValueFromAttribute) && DateTime.TryParse(stringValueFromAttribute, out value))
            {
                result = new DateTime?(value);
            }
            return result;
        }
     
        private static string[] CreateKnownAttributes()
        {
            List<string> list = new List<string>(new string[]
			{
				"title",
				"visible",
				"route",
				"controller",
				"action",
				"url",
				"lastModifiedAt",
				"changeFrequency",
				"updatePriority",
				"includeInSearchEngineIndex"
			});
            list.Sort(StringComparer.OrdinalIgnoreCase);
            return list.ToArray();
        }
        private static T ToEnum<T>(string value, T defaultValue) where T : IComparable, IFormattable
        {
            Type typeFromHandle = typeof(T);
            if (!Enum.IsDefined(typeFromHandle, value))
            {
                return defaultValue;
            }
            return (T)((object)Enum.Parse(typeFromHandle, value, true));
        }
    }
}
