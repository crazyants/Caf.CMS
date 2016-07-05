using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using System;
using System.ComponentModel.DataAnnotations;


namespace CAF.WebSite.Mvc.Admin.Models.Common
{
    public class MaintenanceModel : ModelBase
    {
        public MaintenanceModel()
        {
            DeleteGuests = new DeleteGuestsModel();
            DeleteExportedFiles = new DeleteExportedFilesModel();
            DeleteImageCache = new DeleteImageCacheModel();
        }

        public DeleteGuestsModel DeleteGuests { get; set; }
        public DeleteExportedFilesModel DeleteExportedFiles { get; set; }

        // codehint: sm-add
        public DeleteImageCacheModel DeleteImageCache { get; set; }

        // codehint: sm-add
        [LangResourceDisplayName("Admin.System.Maintenance.SqlQuery")]
        public string SqlQuery { get; set; }


        #region Nested classes

        public class DeleteGuestsModel : ModelBase
        {
            [LangResourceDisplayName("Admin.System.Maintenance.DeleteGuests.StartDate")]
            public DateTime? StartDate { get; set; }

            [LangResourceDisplayName("Admin.System.Maintenance.DeleteGuests.EndDate")]
            public DateTime? EndDate { get; set; }

            [LangResourceDisplayName("Admin.System.Maintenance.DeleteGuests.OnlyWithoutShoppingCart")]
            public bool OnlyWithoutShoppingCart { get; set; }

            public int? NumberOfDeletedUsers { get; set; }
        }

        public class DeleteExportedFilesModel : ModelBase
        {
            [LangResourceDisplayName("Admin.System.Maintenance.DeleteExportedFiles.StartDate")]
            public DateTime? StartDate { get; set; }

            [LangResourceDisplayName("Admin.System.Maintenance.DeleteExportedFiles.EndDate")]
            public DateTime? EndDate { get; set; }

            public int? NumberOfDeletedFiles { get; set; }
        }

        // codehint: sm-add
        public class DeleteImageCacheModel : ModelBase
        {
            [LangResourceDisplayName("Admin.System.Maintenance.DeleteImageCache.FileCount")]
            public long FileCount { get; set; }

            [LangResourceDisplayName("Admin.System.Maintenance.DeleteImageCache.TotalSize")]
            public string TotalSize { get; set; }
        }
        #endregion
    }
}
