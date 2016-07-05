using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Validators.Discounts;


namespace CAF.WebSite.Mvc.Admin.Models.Discounts
{
    [Validator(typeof(DiscountValidator))]
    public class DiscountModel : EntityModelBase
    {
        public DiscountModel()
        {
            AppliedToCategoryModels = new List<AppliedToCategoryModel>();
            AppliedToProductModels = new List<AppliedToProductModel>();
            AvailableDiscountRequirementRules = new List<SelectListItem>();
            DiscountRequirementMetaInfos = new List<DiscountRequirementMetaInfo>();
        }

        [LangResourceDisplayName("Admin.Promotions.Discounts.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.Promotions.Discounts.Fields.DiscountType")]
        public int DiscountTypeId { get; set; }

        [LangResourceDisplayName("Admin.Promotions.Discounts.Fields.UsePercentage")]
        public bool UsePercentage { get; set; }

        [LangResourceDisplayName("Admin.Promotions.Discounts.Fields.DiscountPercentage")]
        public decimal DiscountPercentage { get; set; }

        [LangResourceDisplayName("Admin.Promotions.Discounts.Fields.DiscountAmount")]
        public decimal DiscountAmount { get; set; }
        public string PrimaryStoreCurrencyCode { get; set; }

        [LangResourceDisplayName("Admin.Promotions.Discounts.Fields.StartDate")]
        public DateTime? StartDateUtc { get; set; }

        [LangResourceDisplayName("Admin.Promotions.Discounts.Fields.EndDate")]
        public DateTime? EndDateUtc { get; set; }

        [LangResourceDisplayName("Admin.Promotions.Discounts.Fields.RequiresCouponCode")]
        public bool RequiresCouponCode { get; set; }

        [LangResourceDisplayName("Admin.Promotions.Discounts.Fields.CouponCode")]
        [AllowHtml]
        public string CouponCode { get; set; }

        [LangResourceDisplayName("Admin.Promotions.Discounts.Fields.DiscountLimitation")]
        public int DiscountLimitationId { get; set; }

        [LangResourceDisplayName("Admin.Promotions.Discounts.Fields.LimitationTimes")]
        public int LimitationTimes { get; set; }


        [LangResourceDisplayName("Admin.Promotions.Discounts.Fields.AppliedToCategories")]
        public IList<AppliedToCategoryModel> AppliedToCategoryModels { get; set; }

        [LangResourceDisplayName("Admin.Promotions.Discounts.Fields.AppliedToProducts")]
        public IList<AppliedToProductModel> AppliedToProductModels { get; set; }


        [LangResourceDisplayName("Admin.Promotions.Discounts.Requirements.DiscountRequirementType")]
        public string AddDiscountRequirement { get; set; }

        public IList<SelectListItem> AvailableDiscountRequirementRules { get; set; }

        public IList<DiscountRequirementMetaInfo> DiscountRequirementMetaInfos { get; set; }
        

        #region Nested classes

        public class DiscountRequirementMetaInfo : ModelBase
        {
            public int DiscountRequirementId { get; set; }
            public string RuleName { get; set; }
            public string ConfigurationUrl { get; set; }
        }

        public class DiscountUsageHistoryModel : EntityModelBase
        {
            public int DiscountId { get; set; }

            [LangResourceDisplayName("Admin.Promotions.Discounts.History.Order")]
            public int OrderId { get; set; }

            [LangResourceDisplayName("Admin.Promotions.Discounts.History.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public class AppliedToCategoryModel : ModelBase
        {
            public int CategoryId { get; set; }

            public string Name { get; set; }
        }

        public class AppliedToProductModel : ModelBase
        {
            public int ProductId { get; set; }

            public string ProductName { get; set; }
        }
        #endregion
    }
}