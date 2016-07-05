using CAF.Infrastructure.Core.Domain.Configuration;
using CAF.Infrastructure.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;



namespace CAF.Infrastructure.Core.Configuration
{
    /// <summary>
    /// Setting service interface
    /// </summary>
    public partial interface ISettingService
    {
        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>Setting</returns>
        Setting GetSettingById(int settingId);

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
		/// <param name="siteId">Site identifier</param>
		/// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>Setting value</returns>
		T GetSettingByKey<T>(string key, T defaultValue = default(T), int siteId = 0, bool loadSharedValueIfNotFound = false);

		/// <summary>
		/// Gets all settings
		/// </summary>
		/// <returns>Settings</returns>
		IList<Setting> GetAllSettings();

		/// <summary>
		/// Determines whether a setting exists
		/// </summary>
		/// <typeparam name="T">Entity type</typeparam>
		/// <typeparam name="TPropType">Property type</typeparam>
		/// <param name="settings">Settings</param>
		/// <param name="keySelector">Key selector</param>
		/// <param name="siteId">Site identifier</param>
		/// <returns>true -setting exists; false - does not exist</returns>
		bool SettingExists<T, TPropType>(T settings,
			Expression<Func<T, TPropType>> keySelector, int siteId = 0)
			where T : ISettings, new();

		/// <summary>
		/// Load settings
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="siteId">Site identifier for which settigns should be loaded</param>
		T LoadSetting<T>(int siteId = 0) where T : ISettings, new();

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
		/// <param name="siteId">Site identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        void SetSetting<T>(string key, T value, int siteId = 0, bool clearCache = true);

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
		/// <param name="settings">Setting instance</param>
		/// <param name="siteId">Site identifier</param>
		void SaveSetting<T>(T settings, int siteId = 0) where T : ISettings, new();

		/// <summary>
		/// Save settings object
		/// </summary>
		/// <typeparam name="T">Entity type</typeparam>
		/// <typeparam name="TPropType">Property type</typeparam>
		/// <param name="settings">Settings</param>
		/// <param name="keySelector">Key selector</param>
		/// <param name="siteId">Site ID</param>
		/// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
		void SaveSetting<T, TPropType>(T settings,
			Expression<Func<T, TPropType>> keySelector,
			int siteId = 0, bool clearCache = true) where T : ISettings, new();

		/// <remarks>codehint: sm-add</remarks>
		void UpdateSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, bool overrideForSite, int siteId = 0) where T : ISettings, new();

		void InsertSetting(Setting setting, bool clearCache = true);

		void UpdateSetting(Setting setting, bool clearCache = true);

		/// <summary>
		/// Deletes a setting
		/// </summary>
		/// <param name="setting">Setting</param>
		void DeleteSetting(Setting setting);

        /// <summary>
        /// Delete all settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        void DeleteSetting<T>() where T : ISettings, new();

		/// <summary>
		/// Delete settings object
		/// </summary>
		/// <typeparam name="T">Entity type</typeparam>
		/// <typeparam name="TPropType">Property type</typeparam>
		/// <param name="settings">Settings</param>
		/// <param name="keySelector">Key selector</param>
		/// <param name="siteId">Site ID</param>
		void DeleteSetting<T, TPropType>(T settings,
			Expression<Func<T, TPropType>> keySelector, int siteId = 0) where T : ISettings, new();

		/// <remarks>codehint: sm-add</remarks>
		void DeleteSetting(string key, int siteId = 0);

		/// <summary>
		/// Deletes all settings with its key beginning with rootKey.
		/// </summary>
		/// <remarks>codehint: sm-add</remarks>
		/// <returns>Number of deleted settings</returns>
		int DeleteSettings(string rootKey);

        /// <summary>
        /// Clear cache
        /// </summary>
        void ClearCache();
    }
}
