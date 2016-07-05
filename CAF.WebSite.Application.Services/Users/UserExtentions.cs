using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Application.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Localization;
//using CAF.Infrastructure.Core.Domain.Shop.Orders;
//using CAF.WebSite.Application.Services.Orders;

namespace CAF.WebSite.Application.Services.Users
{
    public static class UserExtentions
    {
        /// <summary>
        /// Gets a value indicating whether user is in a certain user role
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="userRoleSystemName">User role system name</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>Result</returns>
        public static bool IsInUserRole(this User user,
            string userRoleSystemName, bool onlyActiveUserRoles = true)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (String.IsNullOrEmpty(userRoleSystemName))
                throw new ArgumentNullException("userRoleSystemName");

            var result = user.UserRoles
                .Where(cr => !onlyActiveUserRoles || cr.Active)
                .Where(cr => cr.SystemName == userRoleSystemName)
                .FirstOrDefault() != null;
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether the user is a built-in record for background tasks
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Result</returns>
        public static bool IsBackgroundTaskAccount(this User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (!user.IsSystemAccount || String.IsNullOrEmpty(user.UserName))
                return false;

            var result = user.UserName.Equals(SystemUserNames.BackgroundTask, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether user a search engine
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Result</returns>
        public static bool IsSearchEngineAccount(this User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (!user.IsSystemAccount || String.IsNullOrEmpty(user.UserName))
                return false;

            var result = user.UserName.Equals(SystemUserNames.SearchEngine, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether user is administrator
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>Result</returns>
        public static bool IsAdmin(this User user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.Administrators, onlyActiveUserRoles);
        }

        /// <summary>
        /// Gets a value indicating whether user is super administrator
        /// </summary>
        /// <remarks>codehint: sm-add</remarks>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>Result</returns>
        public static bool IsSuperAdmin(this User user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.SuperAdministrators, onlyActiveUserRoles);
        }

        /// <summary>
        /// Gets a value indicating whether user is a forum moderator
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>Result</returns>
        public static bool IsForumModerator(this User user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.ForumModerators, onlyActiveUserRoles);
        }

        /// <summary>
        /// Gets a value indicating whether user is registered
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>Result</returns>
        public static bool IsRegistered(this User user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.Registered, onlyActiveUserRoles);
        }

        /// <summary>
        /// Gets a value indicating whether user is guest
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>Result</returns>
        public static bool IsGuest(this User user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.Guests, onlyActiveUserRoles);
        }

        public static string GetFullName(this User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            var firstName = user.GetAttribute<string>(SystemUserAttributeNames.FirstName);
            var lastName = user.GetAttribute<string>(SystemUserAttributeNames.LastName);

            string fullName = "";
            if (!String.IsNullOrWhiteSpace(firstName) && !String.IsNullOrWhiteSpace(lastName))
            {
                fullName = string.Format("{0} {1}", firstName, lastName);
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(firstName))
                    fullName = firstName;

                if (!String.IsNullOrWhiteSpace(lastName))
                    fullName = lastName;

                if (String.IsNullOrWhiteSpace(firstName) && String.IsNullOrWhiteSpace(lastName))
                {
                    var address = user.Addresses.FirstOrDefault();
                    if (address != null)
                        fullName = string.Format("{0} {1}", address.FirstName, address.LastName);
                }
            }
            return fullName;
        }

        /// <summary>
        /// Formats the user name
        /// </summary>
        /// <param name="user">Source</param>
        /// <returns>Formatted text</returns>
        public static string FormatUserName(this User user)
        {
            return FormatUserName(user, false);
        }

        /// <summary>
        /// Formats the user name
        /// </summary>
        /// <param name="user">Source</param>
        /// <param name="stripTooLong">Strip too long user name</param>
        /// <returns>Formatted text</returns>
        public static string FormatUserName(this User user, bool stripTooLong)
        {
            if (user == null)
                return string.Empty;

            if (user.IsGuest())
            {
                return EngineContext.Current.Resolve<ILocalizationService>().GetResource("User.Guest");
            }

            string result = string.Empty;
            switch (EngineContext.Current.Resolve<UserSettings>().UserNameFormat)
            {
                case UserNameFormat.ShowEmails:
                    result = user.Email;
                    break;
                case UserNameFormat.ShowFullNames:
                    result = user.GetFullName();
                    break;
                case UserNameFormat.ShowUsernames:
                    result = user.UserName;
                    break;
                case UserNameFormat.ShowFirstName:
                    result = user.GetAttribute<string>(SystemUserAttributeNames.FirstName);
                    break;
                case UserNameFormat.ShowNameAndCity:
                    {
                        var firstName = user.GetAttribute<string>(SystemUserAttributeNames.FirstName);
                        var lastName = user.GetAttribute<string>(SystemUserAttributeNames.LastName);
                        var city = user.GetAttribute<string>(SystemUserAttributeNames.City);

                        if (firstName.IsNullOrEmpty())
                        {
                            var address = user.Addresses.FirstOrDefault();
                            if (address != null)
                            {
                                firstName = address.FirstName;
                                lastName = address.LastName;
                                city = address.City;
                            }
                        }

                        result = firstName;
                        if (lastName.HasValue())
                        {
                            result = "{0} {1}.".FormatWith(result, lastName.First());
                        }

                        if (city.HasValue())
                        {
                            var from = EngineContext.Current.Resolve<ILocalizationService>().GetResource("Common.ComingFrom");
                            result = "{0} {1} {2}".FormatWith(result, from, city);
                        }
                    }
                    break;
                default:
                    break;
            }

            if (stripTooLong && result.HasValue())
            {
                int maxLength = EngineContext.Current.Resolve<UserSettings>().UserNameFormatMaxLength;
                if (maxLength > 0 && result.Length > maxLength)
                {
                    result = result.Truncate(maxLength, "...");
                }
            }

            return result;
        }

        /// <summary>
        /// Find any email address of user
        /// </summary>
        public static string FindEmail(this User user)
        {
            if (user != null)
            {
                if (user.Email.HasValue())
                    return user.Email;

                if (user.BillingAddress != null && user.BillingAddress.Email.HasValue())
                    return user.BillingAddress.Email;

                if (user.ShippingAddress != null && user.ShippingAddress.Email.HasValue())
                    return user.ShippingAddress.Email;
            }
            return null;
        }



        //#region Shopping cart

        //public static int CountProductsInCart(this User user, ShoppingCartType cartType, int? storeId = null)
        //{
        //    int count = user.ShoppingCartItems
        //        .Filter(cartType, storeId)
        //        .Where(x => x.ParentItemId == null)
        //        .Sum(x => x.Quantity);

        //    return count;
        //}
        //public static List<OrganizedShoppingCartItem> GetCartItems(this User user, ShoppingCartType cartType, int? storeId = null, bool orderById = false)
        //{
        //    var rawItems = user.ShoppingCartItems.Filter(cartType, storeId);

        //    if (orderById)
        //        rawItems = rawItems.OrderByDescending(x => x.Id);

        //    var organizedItems = rawItems.ToList().Organize();

        //    return organizedItems.ToList();
        //}

        //#endregion

        //#region Gift cards

        ///// <summary>
        ///// Gets coupon codes
        ///// </summary>
        ///// <param name="user">User</param>
        ///// <returns>Coupon codes</returns>
        //public static string[] ParseAppliedGiftCardCouponCodes(this User user)
        //{
        //    var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        //    string existingGiftCartCouponCodes = user.GetAttribute<string>(SystemUserAttributeNames.GiftCardCouponCodes,
        //        genericAttributeService);

        //    var couponCodes = new List<string>();
        //    if (String.IsNullOrEmpty(existingGiftCartCouponCodes))
        //        return couponCodes.ToArray();

        //    try
        //    {
        //        var xmlDoc = new XmlDocument();
        //        xmlDoc.LoadXml(existingGiftCartCouponCodes);

        //        var nodeList1 = xmlDoc.SelectNodes(@"//GiftCardCouponCodes/CouponCode");
        //        foreach (XmlNode node1 in nodeList1)
        //        {
        //            if (node1.Attributes != null && node1.Attributes["Code"] != null)
        //            {
        //                string code = node1.Attributes["Code"].InnerText.Trim();
        //                couponCodes.Add(code);
        //            }
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        Debug.Write(exc.ToString());
        //    }
        //    return couponCodes.ToArray();
        //}

        ///// <summary>
        ///// Adds a coupon code
        ///// </summary>
        ///// <param name="user">User</param>
        ///// <param name="couponCode">Coupon code</param>
        ///// <returns>New coupon codes document</returns>
        //public static void ApplyGiftCardCouponCode(this User user, string couponCode)
        //{
        //    var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        //    string result = string.Empty;
        //    try
        //    {
        //        string existingGiftCartCouponCodes = user.GetAttribute<string>(SystemUserAttributeNames.GiftCardCouponCodes,
        //            genericAttributeService);

        //        couponCode = couponCode.Trim().ToLower();

        //        var xmlDoc = new XmlDocument();
        //        if (String.IsNullOrEmpty(existingGiftCartCouponCodes))
        //        {
        //            var element1 = xmlDoc.CreateElement("GiftCardCouponCodes");
        //            xmlDoc.AppendChild(element1);
        //        }
        //        else
        //        {
        //            xmlDoc.LoadXml(existingGiftCartCouponCodes);
        //        }
        //        var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//GiftCardCouponCodes");

        //        XmlElement gcElement = null;
        //        //find existing
        //        var nodeList1 = xmlDoc.SelectNodes(@"//GiftCardCouponCodes/CouponCode");
        //        foreach (XmlNode node1 in nodeList1)
        //        {
        //            if (node1.Attributes != null && node1.Attributes["Code"] != null)
        //            {
        //                string couponCodeAttribute = node1.Attributes["Code"].InnerText.Trim();
        //                if (couponCodeAttribute.ToLower() == couponCode.ToLower())
        //                {
        //                    gcElement = (XmlElement)node1;
        //                    break;
        //                }
        //            }
        //        }

        //        //create new one if not found
        //        if (gcElement == null)
        //        {
        //            gcElement = xmlDoc.CreateElement("CouponCode");
        //            gcElement.SetAttribute("Code", couponCode);
        //            rootElement.AppendChild(gcElement);
        //        }

        //        result = xmlDoc.OuterXml;
        //    }
        //    catch (Exception exc)
        //    {
        //        Debug.Write(exc.ToString());
        //    }

        //    //apply new value
        //    genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.GiftCardCouponCodes, result);
        //}

        ///// <summary>
        ///// Removes a coupon code
        ///// </summary>
        ///// <param name="user">User</param>
        ///// <param name="couponCode">Coupon code to remove</param>
        ///// <returns>New coupon codes document</returns>
        //public static void RemoveGiftCardCouponCode(this User user, string couponCode)
        //{
        //    //get applied coupon codes
        //    var existingCouponCodes = user.ParseAppliedGiftCardCouponCodes();

        //    //clear them
        //    var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        //    genericAttributeService.SaveAttribute<string>(user, SystemUserAttributeNames.GiftCardCouponCodes, null);

        //    //save again except removed one
        //    foreach (string existingCouponCode in existingCouponCodes)
        //        if (!existingCouponCode.Equals(couponCode, StringComparison.InvariantCultureIgnoreCase))
        //            user.ApplyGiftCardCouponCode(existingCouponCode);
        //}

        //#endregion
    }
}
