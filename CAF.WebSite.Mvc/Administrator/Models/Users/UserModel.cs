using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Mvc.Admin.Validators.Users;
using System.Collections.Generic;
using System;

namespace CAF.WebSite.Mvc.Admin.Models.Users
{
    [Validator(typeof(UserValidator))]
    public class UserModel : TabbableModel
    {
        public UserModel()
        {
            AvailableTimeZones = new List<SelectListItem>();
            SendEmail = new SendEmailModel();
            SendPm = new SendPmModel();
            AvailableUserRoles = new List<UserRoleModel>();
            AssociatedExternalAuthRecords = new List<AssociatedExternalAuthModel>();
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
        }

        public bool AllowUsersToChangeUserNames { get; set; }
        public bool UserNamesEnabled { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.Fields.UserName")]
        [AllowHtml]
        public string UserName { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.Fields.Password")]
        [AllowHtml]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }
        [LangResourceDisplayName("Admin.Users.Users.Fields.Gender")]
        public string Gender { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.Fields.FirstName")]
        [AllowHtml]
        public string FirstName { get; set; }
        [LangResourceDisplayName("Admin.Users.Users.Fields.LastName")]
        [AllowHtml]
        public string LastName { get; set; }
        [LangResourceDisplayName("Admin.Users.Users.Fields.FullName")]
        public string FullName { get; set; }

        public bool DateOfBirthEnabled { get; set; }
        [LangResourceDisplayName("Admin.Users.Users.Fields.DateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        public bool CompanyEnabled { get; set; }
        [LangResourceDisplayName("Admin.Users.Users.Fields.Company")]
        [AllowHtml]
        public string Company { get; set; }

        public bool StreetAddressEnabled { get; set; }
        [LangResourceDisplayName("Admin.Users.Users.Fields.StreetAddress")]
        [AllowHtml]
        public string StreetAddress { get; set; }

        public bool StreetAddress2Enabled { get; set; }
        [LangResourceDisplayName("Admin.Users.Users.Fields.StreetAddress2")]
        [AllowHtml]
        public string StreetAddress2 { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }
        [LangResourceDisplayName("Admin.Users.Users.Fields.ZipPostalCode")]
        [AllowHtml]
        public string ZipPostalCode { get; set; }

        public bool CityEnabled { get; set; }
        [LangResourceDisplayName("Admin.Users.Users.Fields.City")]
        [AllowHtml]
        public string City { get; set; }

        public bool CountryEnabled { get; set; }
        [LangResourceDisplayName("Admin.Users.Users.Fields.Country")]
        public int CountryId { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }

        public bool StateProvinceEnabled { get; set; }
        [LangResourceDisplayName("Admin.Users.Users.Fields.StateProvince")]
        public int StateProvinceId { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }

        public bool PhoneEnabled { get; set; }
        [LangResourceDisplayName("Admin.Users.Users.Fields.Phone")]
        [AllowHtml]
        public string Phone { get; set; }

        public bool FaxEnabled { get; set; }
        [LangResourceDisplayName("Admin.Users.Users.Fields.Fax")]
        [AllowHtml]
        public string Fax { get; set; }







        [LangResourceDisplayName("Admin.Users.Users.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.Fields.IsTaxExempt")]
        public bool IsTaxExempt { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.Fields.Active")]
        public bool Active { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.Fields.Affiliate")]
        public int AffiliateId { get; set; }
        public string AffiliateFullName { get; set; }



        //time zone
        [LangResourceDisplayName("Admin.Users.Users.Fields.TimeZoneId")]
        [AllowHtml]
        public string TimeZoneId { get; set; }

        public bool AllowUsersToSetTimeZone { get; set; }

        public IList<SelectListItem> AvailableTimeZones { get; set; }





        //EU VAT
        [LangResourceDisplayName("Admin.Users.Users.Fields.VatNumber")]
        [AllowHtml]
        public string VatNumber { get; set; }

        public string VatNumberStatusNote { get; set; }

        public bool DisplayVatNumber { get; set; }





        //registration date
        [LangResourceDisplayName("Admin.Users.Users.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [LangResourceDisplayName("Admin.Users.Users.Fields.LastActivityDate")]
        public DateTime LastActivityDate { get; set; }

        //IP adderss
        [LangResourceDisplayName("Admin.Users.Users.Fields.IPAddress")]
        public string LastIpAddress { get; set; }


        [LangResourceDisplayName("Admin.Users.Users.Fields.LastVisitedPage")]
        public string LastVisitedPage { get; set; }


        //user roles
        [LangResourceDisplayName("Admin.Users.Users.Fields.UserRoles")]
        public string UserRoleNames { get; set; }
        public List<UserRoleModel> AvailableUserRoles { get; set; }
        public int[] SelectedUserRoleIds { get; set; }
        public bool AllowManagingUserRoles { get; set; }






        //reward points history
        public bool DisplayRewardPointsHistory { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.AddRewardPointsValue")]
        public int AddRewardPointsValue { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.AddRewardPointsMessage")]
        [AllowHtml]
        public string AddRewardPointsMessage { get; set; }

        //send email model
        public SendEmailModel SendEmail { get; set; }
        //send PM model
        public SendPmModel SendPm { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth")]
        public IList<AssociatedExternalAuthModel> AssociatedExternalAuthRecords { get; set; }


        #region Nested classes

        public class AssociatedExternalAuthModel : EntityModelBase
        {
            [LangResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth.Fields.Email")]
            public string Email { get; set; }

            [LangResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth.Fields.ExternalIdentifier")]
            public string ExternalIdentifier { get; set; }

            [LangResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth.Fields.AuthMethodName")]
            public string AuthMethodName { get; set; }
        }

        public class RewardPointsHistoryModel : EntityModelBase
        {
            [LangResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.Points")]
            public int Points { get; set; }

            [LangResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.PointsBalance")]
            public int PointsBalance { get; set; }

            [LangResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.Message")]
            [AllowHtml]
            public string Message { get; set; }

            [LangResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.Date")]
            public DateTime CreatedOn { get; set; }
        }

        public class SendEmailModel : ModelBase
        {
            [LangResourceDisplayName("Admin.Users.Users.SendEmail.Subject")]
            [AllowHtml]
            public string Subject { get; set; }

            [LangResourceDisplayName("Admin.Users.Users.SendEmail.Body")]
            [AllowHtml]
            public string Body { get; set; }
        }

        public class SendPmModel : ModelBase
        {
            [LangResourceDisplayName("Admin.Users.Users.SendPM.Subject")]
            public string Subject { get; set; }

            [LangResourceDisplayName("Admin.Users.Users.SendPM.Message")]
            public string Message { get; set; }
        }

        public class OrderModel : EntityModelBase
        {
            [LangResourceDisplayName("Admin.Users.Users.Orders.ID")]
            public override int Id { get; set; }

            [LangResourceDisplayName("Admin.Users.Users.Orders.OrderStatus")]
            public string OrderStatus { get; set; }

            [LangResourceDisplayName("Admin.Users.Users.Orders.PaymentStatus")]
            public string PaymentStatus { get; set; }

            [LangResourceDisplayName("Admin.Users.Users.Orders.ShippingStatus")]
            public string ShippingStatus { get; set; }

            [LangResourceDisplayName("Admin.Users.Users.Orders.OrderTotal")]
            public string OrderTotal { get; set; }

            [LangResourceDisplayName("Admin.Users.Users.Orders.Store")]
            public string StoreName { get; set; }

            [LangResourceDisplayName("Admin.Users.Users.Orders.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class ActivityLogModel : EntityModelBase
        {
            [LangResourceDisplayName("Admin.Users.Users.ActivityLog.ActivityLogType")]
            public string ActivityLogTypeName { get; set; }
            [LangResourceDisplayName("Admin.Users.Users.ActivityLog.Comment")]
            public string Comment { get; set; }
            [LangResourceDisplayName("Admin.Users.Users.ActivityLog.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        #endregion
    }
}