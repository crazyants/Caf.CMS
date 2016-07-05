using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Messages;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Users;


namespace CAF.WebSite.Application.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class ExportManager : IExportManager
    {
        #region Fields

        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly ILanguageService _languageService;

        #endregion

        #region Ctor

        public ExportManager(
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            ILanguageService languageService)
        {
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._languageService = languageService;
        }

        #endregion

        #region Utilities



        #endregion

        #region Methods



        /// <summary>
        /// Export user list to XLSX
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="users">Users</param>
        public virtual void ExportUsersToXlsx(Stream stream, IList<User> users)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(stream))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Users");
                //Create Headers and format them
                var properties = new string[]
                    {
                        "UserId",
                        "UserGuid",
                        "Email",
                        "UserName",
                        "PasswordStr",//why can't we use 'Password' name?
                        "PasswordFormatId",
                        "PasswordSalt",
                        "LanguageId",
                        "CurrencyId",
                        "TaxDisplayTypeId",
                        "IsTaxExempt",
                        "VatNumber",
                        "VatNumberStatusId",
                        "TimeZoneId",
                        "AffiliateId",
                        "Active",
                        "IsGuest",
                        "IsRegistered",
                        "IsAdministrator",
                        "IsForumModerator",
                        "FirstName",
                        "LastName",
                        "Gender",
                        "Company",
                        "StreetAddress",
                        "StreetAddress2",
                        "ZipPostalCode",
                        "City",
                        "CountryId",
                        "StateProvinceId",
                        "Phone",
                        "Fax",
                        "AvatarPictureId",
                        "ForumPostCount",
                        "Signature",
                    };
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }


                int row = 2;
                foreach (var user in users)
                {
                    int col = 1;

                    worksheet.Cells[row, col].Value = user.Id;
                    col++;

                    worksheet.Cells[row, col].Value = user.UserGuid;
                    col++;

                    worksheet.Cells[row, col].Value = user.Email;
                    col++;

                    worksheet.Cells[row, col].Value = user.UserName;
                    col++;

                    worksheet.Cells[row, col].Value = user.Password;
                    col++;

                    worksheet.Cells[row, col].Value = user.PasswordFormatId;
                    col++;

                    worksheet.Cells[row, col].Value = user.PasswordSalt;
                    col++;

                    worksheet.Cells[row, col].Value = user.IsTaxExempt;
                    col++;

                    worksheet.Cells[row, col].Value = user.AffiliateId;
                    col++;

                    worksheet.Cells[row, col].Value = user.Active;
                    col++;

                    //roles
                    worksheet.Cells[row, col].Value = user.IsGuest();
                    col++;

                    worksheet.Cells[row, col].Value = user.IsRegistered();
                    col++;

                    worksheet.Cells[row, col].Value = user.IsAdmin();
                    col++;

                    worksheet.Cells[row, col].Value = user.IsForumModerator();
                    col++;

                    //attributes
                    var firstName = user.GetAttribute<string>(SystemUserAttributeNames.FirstName);
                    var lastName = user.GetAttribute<string>(SystemUserAttributeNames.LastName);
                    var gender = user.GetAttribute<string>(SystemUserAttributeNames.Gender);
                    var company = user.GetAttribute<string>(SystemUserAttributeNames.Company);
                    var streetAddress = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress);
                    var streetAddress2 = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress2);
                    var zipPostalCode = user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode);
                    var city = user.GetAttribute<string>(SystemUserAttributeNames.City);
                    var countryId = user.GetAttribute<int>(SystemUserAttributeNames.CountryId);
                    var stateProvinceId = user.GetAttribute<int>(SystemUserAttributeNames.StateProvinceId);
                    var phone = user.GetAttribute<string>(SystemUserAttributeNames.Phone);
                    var fax = user.GetAttribute<string>(SystemUserAttributeNames.Fax);
                    var vatNumber = user.GetAttribute<string>(SystemUserAttributeNames.VatNumber);
                    var vatNumberStatusId = user.GetAttribute<string>(SystemUserAttributeNames.VatNumberStatusId);
                    var timeZoneId = user.GetAttribute<string>(SystemUserAttributeNames.TimeZoneId);

                    var avatarPictureId = user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId);
                    var forumPostCount = user.GetAttribute<int>(SystemUserAttributeNames.ForumPostCount);
                    var signature = user.GetAttribute<string>(SystemUserAttributeNames.Signature);

                    worksheet.Cells[row, col].Value = firstName;
                    col++;

                    worksheet.Cells[row, col].Value = lastName;
                    col++;

                    worksheet.Cells[row, col].Value = gender;
                    col++;

                    worksheet.Cells[row, col].Value = company;
                    col++;

                    worksheet.Cells[row, col].Value = streetAddress;
                    col++;

                    worksheet.Cells[row, col].Value = streetAddress2;
                    col++;

                    worksheet.Cells[row, col].Value = zipPostalCode;
                    col++;

                    worksheet.Cells[row, col].Value = city;
                    col++;

                    worksheet.Cells[row, col].Value = countryId;
                    col++;

                    worksheet.Cells[row, col].Value = stateProvinceId;
                    col++;

                    worksheet.Cells[row, col].Value = phone;
                    col++;

                    worksheet.Cells[row, col].Value = fax;
                    col++;

                    worksheet.Cells[row, col].Value = vatNumber;
                    col++;

                    worksheet.Cells[row, col].Value = vatNumberStatusId;
                    col++;

                    worksheet.Cells[row, col].Value = avatarPictureId;
                    col++;

                    worksheet.Cells[row, col].Value = timeZoneId;
                    col++;

                    worksheet.Cells[row, col].Value = forumPostCount;
                    col++;

                    worksheet.Cells[row, col].Value = signature;
                    col++;

                    row++;
                }








                // we had better add some document properties to the spreadsheet 

                // set some core property values
                //var storeName = _storeInformationSettings.SiteName;
                //var storeUrl = _storeInformationSettings.SiteUrl;
                //xlPackage.Workbook.Properties.Title = string.Format("{0} users", storeName);
                //xlPackage.Workbook.Properties.Author = storeName;
                //xlPackage.Workbook.Properties.Subject = string.Format("{0} users", storeName);
                //xlPackage.Workbook.Properties.Keywords = string.Format("{0} users", storeName);
                //xlPackage.Workbook.Properties.Category = "Users";
                //xlPackage.Workbook.Properties.Comments = string.Format("{0} users", storeName);

                // set some extended property values
                //xlPackage.Workbook.Properties.Company = storeName;
                //xlPackage.Workbook.Properties.HyperlinkBase = new Uri(storeUrl);

                // save the new spreadsheet
                xlPackage.Save();
            }
        }

        /// <summary>
        /// Export user list to xml
        /// </summary>
        /// <param name="users">Users</param>
        /// <returns>Result in XML format</returns>
        public virtual string ExportUsersToXml(IList<User> users)
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Users");
            xmlWriter.WriteAttributeString("Version", WorkVersion.CurrentVersion);

            foreach (var user in users)
            {
                xmlWriter.WriteStartElement("User");
                xmlWriter.WriteElementString("UserId", null, user.Id.ToString());
                xmlWriter.WriteElementString("UserGuid", null, user.UserGuid.ToString());
                xmlWriter.WriteElementString("Email", null, user.Email);
                xmlWriter.WriteElementString("UserName", null, user.UserName);
                xmlWriter.WriteElementString("Password", null, user.Password);
                xmlWriter.WriteElementString("PasswordFormatId", null, user.PasswordFormatId.ToString());
                xmlWriter.WriteElementString("PasswordSalt", null, user.PasswordSalt);
                xmlWriter.WriteElementString("IsTaxExempt", null, user.IsTaxExempt.ToString());
                xmlWriter.WriteElementString("AffiliateId", null, user.AffiliateId.ToString());
                xmlWriter.WriteElementString("Active", null, user.Active.ToString());


                xmlWriter.WriteElementString("IsGuest", null, user.IsGuest().ToString());
                xmlWriter.WriteElementString("IsRegistered", null, user.IsRegistered().ToString());
                xmlWriter.WriteElementString("IsAdministrator", null, user.IsAdmin().ToString());
                xmlWriter.WriteElementString("IsForumModerator", null, user.IsForumModerator().ToString());

                xmlWriter.WriteElementString("FirstName", null, user.GetAttribute<string>(SystemUserAttributeNames.FirstName));
                xmlWriter.WriteElementString("LastName", null, user.GetAttribute<string>(SystemUserAttributeNames.LastName));
                xmlWriter.WriteElementString("Gender", null, user.GetAttribute<string>(SystemUserAttributeNames.Gender));
                xmlWriter.WriteElementString("Company", null, user.GetAttribute<string>(SystemUserAttributeNames.Company));

                xmlWriter.WriteElementString("CountryId", null, user.GetAttribute<int>(SystemUserAttributeNames.CountryId).ToString());
                xmlWriter.WriteElementString("StreetAddress", null, user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress));
                xmlWriter.WriteElementString("StreetAddress2", null, user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress2));
                xmlWriter.WriteElementString("ZipPostalCode", null, user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode));
                xmlWriter.WriteElementString("City", null, user.GetAttribute<string>(SystemUserAttributeNames.City));
                xmlWriter.WriteElementString("CountryId", null, user.GetAttribute<int>(SystemUserAttributeNames.CountryId).ToString());
                xmlWriter.WriteElementString("StateProvinceId", null, user.GetAttribute<int>(SystemUserAttributeNames.StateProvinceId).ToString());
                xmlWriter.WriteElementString("Phone", null, user.GetAttribute<string>(SystemUserAttributeNames.Phone));
                xmlWriter.WriteElementString("Fax", null, user.GetAttribute<string>(SystemUserAttributeNames.Fax));
                xmlWriter.WriteElementString("VatNumber", null, user.GetAttribute<string>(SystemUserAttributeNames.VatNumber));
                xmlWriter.WriteElementString("VatNumberStatusId", null, user.GetAttribute<int>(SystemUserAttributeNames.VatNumberStatusId).ToString());
                xmlWriter.WriteElementString("TimeZoneId", null, user.GetAttribute<string>(SystemUserAttributeNames.TimeZoneId));

                var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmail(user.Email);
                bool subscribedToNewsletters = newsletter != null && newsletter.Active;
                xmlWriter.WriteElementString("Newsletter", null, subscribedToNewsletters.ToString());

                xmlWriter.WriteElementString("AvatarPictureId", null, user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId).ToString());
                xmlWriter.WriteElementString("ForumPostCount", null, user.GetAttribute<int>(SystemUserAttributeNames.ForumPostCount).ToString());
                xmlWriter.WriteElementString("Signature", null, user.GetAttribute<string>(SystemUserAttributeNames.Signature));

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }
        #endregion
    }
}
