using System;
using System.Collections;
using System.Collections.Generic;
using Telerik.Web.Mvc.Infrastructure;
namespace CAF.WebSite.Application.WebUI
{
	public class SiteMapDictionary : IDictionary<string, SiteMapBase>, ICollection<KeyValuePair<string, SiteMapBase>>, IEnumerable<KeyValuePair<string, SiteMapBase>>, IEnumerable
	{
		private readonly IDictionary<string, SiteMapBase> innerDictionary = new Dictionary<string, SiteMapBase>(StringComparer.OrdinalIgnoreCase);
		private static Func<SiteMapBase> defaultSiteMapFactory = SiteMapDictionary.CreateDefaultSiteMapFactory();
		private SiteMapBase defaultSiteMap;
		public static Func<SiteMapBase> DefaultSiteMapFactory
		{
			get
			{
				return SiteMapDictionary.defaultSiteMapFactory;
			}
			set
			{
				
				SiteMapDictionary.defaultSiteMapFactory = value;
			}
		}
		public SiteMapBase DefaultSiteMap
		{
			get
			{
				if (this.defaultSiteMap == null)
				{
					this.defaultSiteMap = SiteMapDictionary.DefaultSiteMapFactory();
				}
				return this.defaultSiteMap;
			}
			set
			{
				this.defaultSiteMap = value;
			}
		}
		public int Count
		{
			get
			{
				return this.innerDictionary.Count;
			}
		}
		public bool IsReadOnly
		{
			get
			{
				return this.innerDictionary.IsReadOnly;
			}
		}
		public ICollection<string> Keys
		{
			get
			{
				return this.innerDictionary.Keys;
			}
		}
		public ICollection<SiteMapBase> Values
		{
			get
			{
				return this.innerDictionary.Values;
			}
		}
		public SiteMapBase this[string key]
		{
			get
			{
				return this.innerDictionary[key];
			}
			set
			{
				this.innerDictionary[key] = value;
			}
		}
		public SiteMapDictionary Register<TSiteMap>(string name, Action<TSiteMap> configure) where TSiteMap : SiteMapBase, new()
		{
		
			TSiteMap tSiteMap = Activator.CreateInstance<TSiteMap>();
			configure(tSiteMap);
			this.Add(name, tSiteMap);
			return this;
		}
		public void Add(KeyValuePair<string, SiteMapBase> item)
		{
			this.innerDictionary.Add(item);
		}
		public void Add(string key, SiteMapBase value)
		{
			this.innerDictionary.Add(key, value);
		}
		public void Clear()
		{
			this.innerDictionary.Clear();
		}
		public bool Contains(KeyValuePair<string, SiteMapBase> item)
		{
			return this.innerDictionary.Contains(item);
		}
		public bool ContainsKey(string key)
		{
			return this.innerDictionary.ContainsKey(key);
		}
		public void CopyTo(KeyValuePair<string, SiteMapBase>[] array, int arrayIndex)
		{
			this.innerDictionary.CopyTo(array, arrayIndex);
		}
		public IEnumerator<KeyValuePair<string, SiteMapBase>> GetEnumerator()
		{
			return this.innerDictionary.GetEnumerator();
		}
		public bool Remove(KeyValuePair<string, SiteMapBase> item)
		{
			return this.innerDictionary.Remove(item);
		}
		public bool Remove(string key)
		{
			return this.innerDictionary.Remove(key);
		}
		public bool TryGetValue(string key, out SiteMapBase value)
		{
			return this.innerDictionary.TryGetValue(key, out value);
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		private static Func<SiteMapBase> CreateDefaultSiteMapFactory()
		{
			return delegate
			{
				XmlSiteMap xmlSiteMap = new XmlSiteMap();
				xmlSiteMap.Load();
				return xmlSiteMap;
			};
		}
	}
}
