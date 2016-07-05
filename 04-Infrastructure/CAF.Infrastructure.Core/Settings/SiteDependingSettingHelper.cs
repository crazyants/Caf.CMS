using System.Linq;
using System.Web.Mvc;
using Fasterflect;
using System.Collections.Generic;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Configuration;

namespace CAF.Infrastructure.Core.Settings
{
    /// <remarks>codehint: sm-add</remarks>
    public class SiteDependingSettingHelper
    {
        private ViewDataDictionary _viewData;

        public SiteDependingSettingHelper(ViewDataDictionary viewData)
        {
            _viewData = viewData;
        }

        public static string ViewDataKey { get { return "SiteDependingSettingData"; } }
        public SiteDependingSettingData Data
        {
            get
            {
                return _viewData[ViewDataKey] as SiteDependingSettingData;
            }
        }

        private bool IsOverrideChecked(string settingKey, FormCollection form)
        {
            var rawOverrideKey = form.AllKeys.FirstOrDefault(k => k.IsCaseInsensitiveEqual(settingKey + "_OverrideForSite"));

            if (rawOverrideKey.HasValue())
            {
                var checkboxValue = form[rawOverrideKey].EmptyNull().ToLower();
                return checkboxValue.Contains("on") || checkboxValue.Contains("true");
            }
            return false;
        }
        public bool IsOverrideChecked(object settings, string name, FormCollection form)
        {
            var key = settings.GetType().Name + "." + name;
            return IsOverrideChecked(key, form);
        }
        public void AddOverrideKey(object settings, string name)
        {
            var key = settings.GetType().Name + "." + name;
            Data.OverrideSettingKeys.Add(key);
        }
        public void CreateViewDataObject(int activeSiteScopeConfiguration, string rootSettingClass = null)
        {
            _viewData[ViewDataKey] = new SiteDependingSettingData()
            {
                ActiveSiteScopeConfiguration = activeSiteScopeConfiguration,
                RootSettingClass = rootSettingClass
            };
        }

        public void GetOverrideKeys(object settings, object model, int siteId, ISettingService settingService, bool isRootModel = true)
        {
            if (siteId <= 0)
                return;		// single store mode -> there are no overrides

            var data = Data;
            if (data == null)
                data = new SiteDependingSettingData();

            var settingName = settings.GetType().Name;
            var properties = settings.GetType().GetProperties();

            var modelType = model.GetType();

            foreach (var prop in properties)
            {
                var name = prop.Name;
                var modelProperty = modelType.GetProperty(name);

                if (modelProperty == null)
                    continue;	// setting is not configurable or missing or whatever... however we don't need the override info

                var key = settingName + "." + name;
                var setting = settingService.GetSettingByKey<string>(key, siteId: siteId);

                if (setting != null)
                    data.OverrideSettingKeys.Add(key);
            }

            if (isRootModel)
            {
                data.ActiveSiteScopeConfiguration = siteId;
                data.RootSettingClass = settingName;

                _viewData[ViewDataKey] = data;
            }
        }
        public void UpdateSettings(object settings, FormCollection form, int siteId, ISettingService settingService)
        {
            var settingName = settings.GetType().Name;
            var properties = settings.GetType().GetProperties();

            foreach (var prop in properties)
            {
                var name = prop.Name;
                var key = settingName + "." + name;

                if (siteId == 0 || IsOverrideChecked(key, form))
                {
                    dynamic value = settings.TryGetPropertyValue(name);
                    settingService.SetSetting(key, value == null ? "" : value, siteId, false);
                }
                else if (siteId > 0)
                {
                    settingService.DeleteSetting(key, siteId);
                }
            }
        }
    }
}
