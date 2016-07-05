
using CAF.WebSite.CustomBanner.Domain;
using System;
namespace CAF.WebSite.CustomBanner.Services
{
	public interface ICustomBannerService
	{
		CustomBannerRecord GetCustomBannerRecord(int entityId, string entityName);
		void InsertCustomBannerRecord(CustomBannerRecord record);
		void UpdateCustomBannerRecord(CustomBannerRecord record);
		void DeleteCustomBannerRecord(CustomBannerRecord record);
	}
}
