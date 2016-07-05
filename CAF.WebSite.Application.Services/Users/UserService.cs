
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using CAF.Infrastructure.Core.Domain.Common.Common;
using CAF.WebSite.Application.Services.Common;
using System.Globalization;
//using CAF.Infrastructure.Core.Domain.Shop.Catalog;
using CAF.Infrastructure.Core.Localization;
//using CAF.Infrastructure.Core.Domain.Shop.Orders;
using CAF.WebSite.Application.Services.Localization;

namespace CAF.WebSite.Application.Services.Users
{
    public partial class UserService : IUserService
    {

        #region Constants

        private const string UserROLES_ALL_KEY = "caf.Userrole.all-{0}";
        private const string UserROLES_BY_SYSTEMNAME_KEY = "caf.Userrole.systemname-{0}";
        private const string UserROLES_PATTERN_KEY = "caf.Userrole.";
        #endregion

        #region Fields
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
      //  private readonly RewardPointsSettings _rewardPointsSettings;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="userRepository"></param>
        /// <param name="userRoleRepository"></param>
        /// <param name="eventPublisher"></param>
        public UserService(ICacheManager cacheManager,
            IRepository<User> userRepository, IRepository<UserRole> userRoleRepository,
            IRepository<GenericAttribute> gaRepository,
            IGenericAttributeService genericAttributeService,
            IEventPublisher eventPublisher
            //RewardPointsSettings rewardPointsSettings
            )
        {
            this._cacheManager = cacheManager;
            this._userRepository = userRepository;
            this._gaRepository = gaRepository;
            this._genericAttributeService = genericAttributeService;
            this._userRoleRepository = userRoleRepository;
            this._eventPublisher = eventPublisher;
           // this._rewardPointsSettings = rewardPointsSettings;

            T = NullLocalizer.Instance;
        }

        #endregion
        #region Properties

        public Localizer T { get; set; }

        #endregion
        #region Methods

        #region Users
        /// <summary>
        /// Gets all users
        /// </summary>
        /// <param name="registrationFrom">User registration from; null to load all users</param>
        /// <param name="registrationTo">User registration to; null to load all users</param>
        /// <param name="userRoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users; </param>
        /// <param name="email">Email; null to load all users</param>
        /// <param name="username">UserName; null to load all users</param>
        /// <param name="firstName">First name; null to load all users</param>
        /// <param name="lastName">Last name; null to load all users</param>
        /// <param name="dayOfBirth">Day of birth; 0 to load all users</param>
        /// <param name="monthOfBirth">Month of birth; 0 to load all users</param>
        /// <param name="company">Company; null to load all users</param>
        /// <param name="phone">Phone; null to load all users</param>
        /// <param name="zipPostalCode">Phone; null to load all users</param>
        /// <param name="loadOnlyWithShoppingCart">Value indicating whther to load users only with shopping cart</param>
        /// <param name="sct">Value indicating what shopping cart type to filter; userd when 'loadOnlyWithShoppingCart' param is 'true'</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>User collection</returns>
        public virtual IPagedList<User> GetAllUsers(DateTime? registrationFrom,
            DateTime? registrationTo, int[] userRoleIds, string email, string username,
            string firstName, string lastName, int dayOfBirth, int monthOfBirth,
            string company, string phone, string zipPostalCode,
            bool loadOnlyWithShoppingCart,
            //ShoppingCartType? sct,
            int pageIndex, int pageSize)
        {
            var query = _userRepository.Table;
            if (registrationFrom.HasValue)
                query = query.Where(c => registrationFrom.Value <= c.CreatedOnUtc);
            if (registrationTo.HasValue)
                query = query.Where(c => registrationTo.Value >= c.CreatedOnUtc);
            query = query.Where(c => !c.Deleted);
            if (userRoleIds != null && userRoleIds.Length > 0)
                query = query.Where(c => c.UserRoles.Select(cr => cr.Id).Intersect(userRoleIds).Count() > 0);
            if (!String.IsNullOrWhiteSpace(email))
                query = query.Where(c => c.Email.Contains(email));
            if (!String.IsNullOrWhiteSpace(username))
                query = query.Where(c => c.UserName.Contains(username));
            if (!String.IsNullOrWhiteSpace(firstName))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "User" &&
                        z.Attribute.Key == SystemUserAttributeNames.FirstName &&
                        z.Attribute.Value.Contains(firstName)))
                    .Select(z => z.User);
            }
            if (!String.IsNullOrWhiteSpace(lastName))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "User" &&
                        z.Attribute.Key == SystemUserAttributeNames.LastName &&
                        z.Attribute.Value.Contains(lastName)))
                    .Select(z => z.User);
            }
            //date of birth is stored as a string into database.
            //we also know that date of birth is stored in the following format YYYY-MM-DD (for example, 1983-02-18).
            //so let's search it as a string
            if (dayOfBirth > 0 && monthOfBirth > 0)
            {
                //both are specified
                string dateOfBirthStr = monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-" + dayOfBirth.ToString("00", CultureInfo.InvariantCulture);
                //EndsWith is not supported by SQL Server Compact
                //so let's use the following workaround http://social.msdn.microsoft.com/Forums/is/sqlce/thread/0f810be1-2132-4c59-b9ae-8f7013c0cc00

                //we also cannot use Length function in SQL Server Compact (not supported in this context)
                //z.Attribute.Value.Length - dateOfBirthStr.Length = 5
                //dateOfBirthStr.Length = 5
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "User" &&
                        z.Attribute.Key == SystemUserAttributeNames.DateOfBirth &&
                        z.Attribute.Value.Substring(5, 5) == dateOfBirthStr))
                    .Select(z => z.User);
            }
            else if (dayOfBirth > 0)
            {
                //only day is specified
                string dateOfBirthStr = dayOfBirth.ToString("00", CultureInfo.InvariantCulture);
                //EndsWith is not supported by SQL Server Compact
                //so let's use the following workaround http://social.msdn.microsoft.com/Forums/is/sqlce/thread/0f810be1-2132-4c59-b9ae-8f7013c0cc00

                //we also cannot use Length function in SQL Server Compact (not supported in this context)
                //z.Attribute.Value.Length - dateOfBirthStr.Length = 8
                //dateOfBirthStr.Length = 2
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "User" &&
                        z.Attribute.Key == SystemUserAttributeNames.DateOfBirth &&
                        z.Attribute.Value.Substring(8, 2) == dateOfBirthStr))
                    .Select(z => z.User);
            }
            else if (monthOfBirth > 0)
            {
                //only month is specified
                string dateOfBirthStr = "-" + monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-";
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "User" &&
                        z.Attribute.Key == SystemUserAttributeNames.DateOfBirth &&
                        z.Attribute.Value.Contains(dateOfBirthStr)))
                    .Select(z => z.User);
            }
            //search by company
            if (!String.IsNullOrWhiteSpace(company))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "User" &&
                        z.Attribute.Key == SystemUserAttributeNames.Company &&
                        z.Attribute.Value.Contains(company)))
                    .Select(z => z.User);
            }
            //search by phone
            if (!String.IsNullOrWhiteSpace(phone))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "User" &&
                        z.Attribute.Key == SystemUserAttributeNames.Phone &&
                        z.Attribute.Value.Contains(phone)))
                    .Select(z => z.User);
            }
            //search by zip
            if (!String.IsNullOrWhiteSpace(zipPostalCode))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "User" &&
                        z.Attribute.Key == SystemUserAttributeNames.ZipPostalCode &&
                        z.Attribute.Value.Contains(zipPostalCode)))
                    .Select(z => z.User);
            }

            //if (loadOnlyWithShoppingCart)
            //{
            //    int? sctId = null;
            //    if (sct.HasValue)
            //        sctId = (int)sct.Value;

            //    query = sct.HasValue ?
            //        query.Where(c => c.ShoppingCartItems.Where(x => x.ShoppingCartTypeId == sctId).Count() > 0) :
            //        query.Where(c => c.ShoppingCartItems.Count() > 0);
            //}

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            var users = new PagedList<User>(query, pageIndex, pageSize);
            return users;
        }
        /// <summary>
        /// Gets all users by affiliate identifier
        /// </summary>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Users</returns>
        public virtual IPagedList<User> GetAllUsers(int affiliateId, int pageIndex, int pageSize)
        {
            var query = _userRepository.Table;
            query = query.Where(c => !c.Deleted);
            query = query.Where(c => c.AffiliateId == affiliateId);
            query = query.OrderByDescending(c => c.CreatedOnUtc);

            var users = new PagedList<User>(query, pageIndex, pageSize);
            return users;
        }

        /// <summary>
        /// Gets all users by user format (including deleted ones)
        /// </summary>
        /// <param name="passwordFormat">Password format</param>
        /// <returns>Users</returns>
        public virtual IList<User> GetAllUsersByPasswordFormat(PasswordFormat passwordFormat)
        {
            int passwordFormatId = (int)passwordFormat;

            var query = _userRepository.Table;
            query = query.Where(c => c.PasswordFormatId == passwordFormatId);
            query = query.OrderByDescending(c => c.CreatedOnUtc);
            var users = query.ToList();
            return users;
        }

        /// <summary>
        /// Gets online users
        /// </summary>
        /// <param name="lastActivityFromUtc">User last activity date (from)</param>
        /// <param name="userRoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users; </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>User collection</returns>
        public virtual IPagedList<User> GetOnlineUsers(DateTime lastActivityFromUtc,
            int[] userRoleIds, int pageIndex, int pageSize)
        {
            var query = _userRepository.Table;
            query = query.Where(c => lastActivityFromUtc <= c.LastActivityDateUtc);
            query = query.Where(c => !c.Deleted);
            if (userRoleIds != null && userRoleIds.Length > 0)
                query = query.Where(c => c.UserRoles.Select(cr => cr.Id).Intersect(userRoleIds).Count() > 0);

            query = query.OrderByDescending(c => c.LastActivityDateUtc);
            var users = new PagedList<User>(query, pageIndex, pageSize);
            return users;
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="user">User</param>
        public virtual void DeleteUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (user.IsSystemAccount)
                throw new WorkException(string.Format("System user account ({0}) could not be deleted", user.UserName));

            user.Deleted = true;
            UpdateUser(user);
        }

        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>A user</returns>
        public virtual User GetUserById(int userId)
        {
            if (userId == 0)
                return null;

            var user = _userRepository.GetById(userId);
            return user;
        }

        /// <summary>
        /// Get users by identifiers
        /// </summary>
        /// <param name="userIds">User identifiers</param>
        /// <returns>Users</returns>
        public virtual IList<User> GetUsersByIds(int[] userIds)
        {
            if (userIds == null || userIds.Length == 0)
                return new List<User>();

            var query = from c in _userRepository.Table
                        where userIds.Contains(c.Id)
                        select c;
            var users = query.ToList();
            //sort by passed identifiers
            var sortedUsers = new List<User>();
            foreach (int id in userIds)
            {
                var user = users.Find(x => x.Id == id);
                if (user != null)
                    sortedUsers.Add(user);
            }
            return sortedUsers;
        }

        /// <summary>
        /// Gets a user by GUID
        /// </summary>
        /// <param name="userGuid">User GUID</param>
        /// <returns>A user</returns>
        public virtual User GetUserByGuid(Guid userGuid)
        {
            if (userGuid == Guid.Empty)
                return null;

            var query = from c in _userRepository.Table
                        where c.UserGuid == userGuid
                        orderby c.Id
                        select c;
            var user = query.FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>User</returns>
        public virtual User GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var query = from c in _userRepository.Table
                        orderby c.Id
                        where c.Email == email
                        select c;
            var user = query.FirstOrDefault();
            return user;
        }

      

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">UserName</param>
        /// <returns>User</returns>
        public virtual User GetUserByUserName(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var query = from c in _userRepository.Table
                        orderby c.Id
                        where c.UserName == username
                        select c;

            var user = query.FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Insert a guest user
        /// </summary>
        /// <returns>User</returns>
        public virtual User InsertGuestUser()
        {
            var user = new User()
            {
                UserGuid = Guid.NewGuid(),
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                ModifiedOnUtc=DateTime .UtcNow
            };

            //add to 'Guests' role
            var guestRole = GetUserRoleBySystemName(SystemUserRoleNames.Guests);
            if (guestRole == null)
                throw new WorkException("'Guests' role could not be loaded");
            user.UserRoles.Add(guestRole);

            _userRepository.Insert(user);

            return user;
        }

        /// <summary>
        /// Insert a user
        /// </summary>
        /// <param name="user">User</param>
        public virtual void InsertUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _userRepository.Insert(user);

            //event notification
            _eventPublisher.EntityInserted(user);
        }

        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="user">User</param>
        public virtual void UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _userRepository.Update(user);

            //event notification
            _eventPublisher.EntityUpdated(user);
        }

        /// <summary>
        /// Reset data required for checkout
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="siteId">Store identifier</param>
        /// <param name="clearCouponCodes">A value indicating whether to clear coupon code</param>
        /// <param name="clearExtendedAttributes">A value indicating whether to clear selected checkout attributes</param>
        /// <param name="clearRewardPoints">A value indicating whether to clear "Use reward points" flag</param>
        /// <param name="clearShippingMethod">A value indicating whether to clear selected shipping method</param>
        /// <param name="clearPaymentMethod">A value indicating whether to clear selected payment method</param>
        public virtual void ResetCheckoutData(User user, int siteId,
            bool clearCouponCodes = false, bool clearExtendedAttributes = false,
            bool clearRewardPoints = true, bool clearShippingMethod = true,
            bool clearPaymentMethod = true)
        {
            if (user == null)
                throw new ArgumentNullException();

            //clear entered coupon codes
            //if (clearCouponCodes)
            //{
            //    _genericAttributeService.SaveAttribute<ShippingOption>(user, SystemUserAttributeNames.DiscountCouponCode, null);
            //    _genericAttributeService.SaveAttribute<ShippingOption>(user, SystemUserAttributeNames.GiftCardCouponCodes, null);
            //}

            ////clear checkout attributes
            //if (clearExtendedAttributes)
            //{
            //    _genericAttributeService.SaveAttribute<ShippingOption>(user, SystemUserAttributeNames.ExtendedAttributes, null);
            //}

            //clear reward points flag
            if (clearRewardPoints)
            {
                _genericAttributeService.SaveAttribute<bool>(user, SystemUserAttributeNames.UseRewardPointsDuringCheckout, false, siteId);
            }

            ////clear selected shipping method
            //if (clearShippingMethod)
            //{
            //    _genericAttributeService.SaveAttribute<ShippingOption>(user, SystemUserAttributeNames.SelectedShippingOption, null, siteId);
            //    _genericAttributeService.SaveAttribute<ShippingOption>(user, SystemUserAttributeNames.OfferedShippingOptions, null, siteId);
            //}

            //clear selected payment method
            if (clearPaymentMethod)
            {
                _genericAttributeService.SaveAttribute<string>(user, SystemUserAttributeNames.SelectedPaymentMethod, null, siteId);
            }

            UpdateUser(user);
        }

        /// <summary>
        /// Delete guest user records
        /// </summary>
        /// <param name="registrationFrom">User registration from; null to load all users</param>
        /// <param name="registrationTo">User registration to; null to load all users</param>
        /// <param name="onlyWithoutShoppingCart">A value indicating whether to delete users only without shopping cart</param>
        /// <returns>Number of deleted users</returns>
        public virtual int DeleteGuestUsers(DateTime? registrationFrom, DateTime? registrationTo, bool onlyWithoutShoppingCart, int maxItemsToDelete = 5000)
        {
            var ctx = _userRepository.Context;

            using (var scope = new DbContextScope(ctx: ctx, autoDetectChanges: false, proxyCreation: true, validateOnSave: false, forceNoTracking: true))
            {
                var guestRole = GetUserRoleBySystemName(SystemUserRoleNames.Guests);
                if (guestRole == null)
                    throw new WorkException("'Guests' role could not be loaded");

                var query = _userRepository.Table;

                if (registrationFrom.HasValue)
                    query = query.Where(c => registrationFrom.Value <= c.CreatedOnUtc);
                if (registrationTo.HasValue)
                    query = query.Where(c => registrationTo.Value >= c.CreatedOnUtc);
                query = query.Where(c => c.UserRoles.Select(cr => cr.Id).Contains(guestRole.Id));

                //if (onlyWithoutShoppingCart)
                //    query = query.Where(c => !c.ShoppingCartItems.Any());

                //// no orders
                //query = JoinWith<Order>(query, x => x.UserId);

                // no user content
                query = JoinWith<UserContent>(query, x => x.UserId);

                //// no forum posts
                //query = JoinWith<ForumPost>(query, x => x.UserId);

                //// no forum topics
                //query = JoinWith<ForumTopic>(query, x => x.UserId);

                //// no blog comments
                //query = JoinWith<BlogComment>(query, x => x.UserId);

                //// no news comments
                //query = JoinWith<NewsComment>(query, x => x.UserId);

                //// no product reviews
                //query = JoinWith<ProductReview>(query, x => x.UserId);

                //// no product review helpfulness
                //query = JoinWith<ProductReviewHelpfulness>(query, x => x.UserId);

                //// no poll voting
                //query = JoinWith<PollVotingRecord>(query, x => x.UserId);

                //don't delete system accounts
                query = query.Where(c => !c.IsSystemAccount);

                // only distinct items
                query = from c in query
                        group c by c.Id
                            into cGroup
                            orderby cGroup.Key
                            select cGroup.FirstOrDefault();
                query = query.OrderBy(c => c.Id);

                var users = query.Take(maxItemsToDelete).ToList();

                var crAutoCommit = _userRepository.AutoCommitEnabled;
                var gaAutoCommit = _gaRepository.AutoCommitEnabled;
                _userRepository.AutoCommitEnabled = false;
                _gaRepository.AutoCommitEnabled = false;

                int numberOfDeletedUsers = 0;
                foreach (var c in users)
                {
                    try
                    {
                        // delete attributes (using GenericAttributeService would incorporate caching... which is bad in long running processes)
                        var gaQuery = from ga in _gaRepository.Table
                                      where ga.EntityId == c.Id &&
                                      ga.KeyGroup == "User"
                                      select ga;
                        var attributes = gaQuery.ToList();

                        foreach (var attribute in attributes)
                        {
                            _gaRepository.Delete(attribute);
                        }

                        // delete user
                        _userRepository.Delete(c);
                        numberOfDeletedUsers++;

                        if (numberOfDeletedUsers % 1000 == 0)
                        {
                            // save changes all 1000th item
                            try
                            {
                                scope.Commit();
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                // save the rest
                scope.Commit();

                _userRepository.AutoCommitEnabled = crAutoCommit;
                _gaRepository.AutoCommitEnabled = gaAutoCommit;

                return numberOfDeletedUsers;
            }
        }

        private IQueryable<User> JoinWith<T>(IQueryable<User> query, Expression<Func<T, int>> userIdSelector) where T : BaseEntity
        {
            var inner = _userRepository.Context.Set<T>().AsNoTracking();

            /* 
             * Lamda join created with LinqPad. ORIGINAL:
                 from c in users
                    join inner in ctx.Set<TInner>().AsNoTracking() on c.Id equals inner.UserId into c_inner
                    from inner in c_inner.DefaultIfEmpty()
                    where !c_inner.Any()
                    select c;
            */
            query = query
                .GroupJoin(
                    inner,
                    c => c.Id,
                    userIdSelector,
                    (c, i) => new { User = c, Inner = i })
                .SelectMany(
                    x => x.Inner.DefaultIfEmpty(),
                    (a, b) => new { a, b }
                )
                .Where(x => !(x.a.Inner.Any()))
                .Select(x => x.a.User);

            return query;
        }


        #endregion

        #region User roles

        /// <summary>
        /// Delete a user role
        /// </summary>
        /// <param name="userRole">User role</param>
        public virtual void DeleteUserRole(UserRole userRole)
        {
            if (userRole == null)
                throw new ArgumentNullException("userRole");

            if (userRole.IsSystemRole)
                throw new WorkException("System role could not be deleted");

            _userRoleRepository.Delete(userRole);

            _cacheManager.RemoveByPattern(UserROLES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(userRole);
        }

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="userRoleId">User role identifier</param>
        /// <returns>User role</returns>
        public virtual UserRole GetUserRoleById(int userRoleId)
        {
            if (userRoleId == 0)
                return null;

            return _userRoleRepository.GetById(userRoleId);
        }

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="systemName">User role system name</param>
        /// <returns>User role</returns>
        public virtual UserRole GetUserRoleBySystemName(string systemName)
        {
            if (String.IsNullOrWhiteSpace(systemName))
                return null;

            string key = string.Format(UserROLES_BY_SYSTEMNAME_KEY, systemName);
            return _cacheManager.Get(key, () =>
            {
                var query = from cr in _userRoleRepository.Table
                            orderby cr.Id
                            where cr.SystemName == systemName
                            select cr;
                var userRole = query.FirstOrDefault();
                return userRole;
            });
        }

        /// <summary>
        /// Gets all user roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>User role collection</returns>
        public virtual IList<UserRole> GetAllUserRoles(bool showHidden = false)
        {
            string key = string.Format(UserROLES_ALL_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from cr in _userRoleRepository.Table
                            orderby cr.Name
                            where (showHidden || cr.Active)
                            select cr;
                var userRoles = query.ToList();
                return userRoles;
            });
        }

        /// <summary>
        /// Inserts a user role
        /// </summary>
        /// <param name="userRole">User role</param>
        public virtual void InsertUserRole(UserRole userRole)
        {
            if (userRole == null)
                throw new ArgumentNullException("userRole");

            _userRoleRepository.Insert(userRole);

            _cacheManager.RemoveByPattern(UserROLES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(userRole);
        }

        /// <summary>
        /// Updates the user role
        /// </summary>
        /// <param name="userRole">User role</param>
        public virtual void UpdateUserRole(UserRole userRole)
        {
            if (userRole == null)
                throw new ArgumentNullException("userRole");

            _userRoleRepository.Update(userRole);

            _cacheManager.RemoveByPattern(UserROLES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(userRole);
        }

        #endregion

        #region Reward points

        /// <summary>
        /// Add or remove reward points for a product review
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="product">The product</param>
        /// <param name="add">Whether to add or remove points</param>
        //public virtual void RewardPointsForProductReview(User user, Product product, bool add)
        //{
        //    if (_rewardPointsSettings.Enabled && _rewardPointsSettings.PointsForProductReview > 0)
        //    {
        //        string message = T(add ? "RewardPoints.Message.EarnedForProductReview" : "RewardPoints.Message.ReducedForProductReview", product.GetLocalized(x => x.Name));

        //        user.AddRewardPointsHistoryEntry(_rewardPointsSettings.PointsForProductReview * (add ? 1 : -1), message);

        //        UpdateUser(user);
        //    }
        //}

        #endregion Reward points
        #endregion

    }
}
