using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.WebSite.Application.Seedwork;
using CAF.WebSite.Application.Services.Catalog;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Domain.Seedwork.Shop.Catalog;
using CAF.WebSite.Domain.Seedwork.Shop.Orders;
using CAF.WebSite.Domain.Seedwork.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace CAF.WebSite.Application.Services.Orders
{
    /// <summary>
    /// Shopping cart service
    /// </summary>
    public partial class ShoppingCartService : IShoppingCartService
    {
        #region Fields

        private readonly IRepository<ShoppingCartItem> _sciRepository;
        private readonly IWorkContext _workContext;
		private readonly ISiteContext _siteContext;
        private readonly ICurrencyService _currencyService;
        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeParser _productAttributeParser;
		private readonly IProductAttributeService _productAttributeService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IUserService _userService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
		private readonly ISiteMappingService _siteMappingService;
		private readonly IGenericAttributeService _genericAttributeService;
		private readonly IDownloadService _downloadService;
		private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="sciRepository">Shopping cart repository</param>
        /// <param name="workContext">Work context</param>
		/// <param name="siteContext">Site context</param>
        /// <param name="currencyService">Currency service</param>
        /// <param name="productService">Product settings</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <param name="checkoutAttributeService">Checkout attribute service</param>
        /// <param name="checkoutAttributeParser">Checkout attribute parser</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <param name="userService">User service</param>
        /// <param name="shoppingCartSettings">Shopping cart settings</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="permissionService">Permission service</param>
        /// <param name="aclService">ACL service</param>
		/// <param name="siteMappingService">Site mapping service</param>
		/// <param name="genericAttributeService">Generic attribute service</param>
        public ShoppingCartService(IRepository<ShoppingCartItem> sciRepository,
			IWorkContext workContext, ISiteContext siteContext, 
			ICurrencyService currencyService,
            IProductService productService, ILocalizationService localizationService,
            IProductAttributeParser productAttributeParser,
			IProductAttributeService productAttributeService,
            ICheckoutAttributeService checkoutAttributeService,
            ICheckoutAttributeParser checkoutAttributeParser,
            IPriceFormatter priceFormatter,
            IUserService userService,
            ShoppingCartSettings shoppingCartSettings,
            IEventPublisher eventPublisher,
            IPermissionService permissionService, 
            IAclService aclService,
			ISiteMappingService siteMappingService,
			IGenericAttributeService genericAttributeService,
			IDownloadService downloadService,
			CatalogSettings catalogSettings)
        {
            this._sciRepository = sciRepository;
            this._workContext = workContext;
			this._siteContext = siteContext;
            this._currencyService = currencyService;
            this._productService = productService;
            this._localizationService = localizationService;
            this._productAttributeParser = productAttributeParser;
			this._productAttributeService = productAttributeService;
            this._checkoutAttributeService = checkoutAttributeService;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._priceFormatter = priceFormatter;
            this._userService = userService;
            this._shoppingCartSettings = shoppingCartSettings;
            this._eventPublisher = eventPublisher;
            this._permissionService = permissionService;
            this._aclService = aclService;
			this._siteMappingService = siteMappingService;
			this._genericAttributeService = genericAttributeService;
			this._downloadService = downloadService;
			this._catalogSettings = catalogSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current user</param>
		/// <param name="deleteChildCartItems">A value indicating whether to delete child cart items</param>
        public virtual void DeleteShoppingCartItem(ShoppingCartItem shoppingCartItem, bool resetCheckoutData = true, 
            bool ensureOnlyActiveCheckoutAttributes = false, bool deleteChildCartItems = true)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException("shoppingCartItem");

            var user = shoppingCartItem.User;
			var siteId = shoppingCartItem.SiteId;
			int cartItemId = shoppingCartItem.Id;

            //reset checkout data
            if (resetCheckoutData)
            {
				_userService.ResetCheckoutData(shoppingCartItem.User, shoppingCartItem.SiteId);
            }

            //delete item
            _sciRepository.Delete(shoppingCartItem);

            //validate checkout attributes
            if (ensureOnlyActiveCheckoutAttributes &&
                //only for shopping cart items (ignore wishlist)
                shoppingCartItem.ShoppingCartType == ShoppingCartType.ShoppingCart)
            {
				var cart = user.GetCartItems(ShoppingCartType.ShoppingCart, siteId);

				var checkoutAttributesXml = user.GetAttribute<string>(SystemUserAttributeNames.CheckoutAttributes, _genericAttributeService);
				checkoutAttributesXml = _checkoutAttributeParser.EnsureOnlyActiveAttributes(checkoutAttributesXml, cart);
				_genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.CheckoutAttributes, checkoutAttributesXml);
            }

            //event notification
            _eventPublisher.EntityDeleted(shoppingCartItem);

			// delete child items
			if (deleteChildCartItems)
			{
				var childCartItems = _sciRepository.Table
					.Where(x => x.UserId == user.Id && x.ParentItemId != null && x.ParentItemId.Value == cartItemId && x.Id != cartItemId)
					.ToList();

				foreach (var cartItem in childCartItems)
					DeleteShoppingCartItem(cartItem, resetCheckoutData, ensureOnlyActiveCheckoutAttributes, false);
			}
        }

		public virtual void DeleteShoppingCartItem(int shoppingCartItemId, bool resetCheckoutData = true,
			bool ensureOnlyActiveCheckoutAttributes = false, bool deleteChildCartItems = true)
		{
			if (shoppingCartItemId != 0)
			{
				var shoppingCartItem = _sciRepository.GetById(shoppingCartItemId);

				DeleteShoppingCartItem(shoppingCartItem, resetCheckoutData, ensureOnlyActiveCheckoutAttributes, deleteChildCartItems);
			}
		}

        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        /// <returns>Number of deleted items</returns>
        public virtual int DeleteExpiredShoppingCartItems(DateTime olderThanUtc)
        {
            var query =
				from sci in _sciRepository.Table
				where sci.UpdatedOnUtc < olderThanUtc && sci.ParentItemId == null
				select sci;

            var cartItems = query.ToList();

			foreach (var parentItem in cartItems)
			{
				var childItems = _sciRepository.Table
					.Where(x => x.ParentItemId != null && x.ParentItemId.Value == parentItem.Id && x.Id != parentItem.Id).ToList();

				foreach (var childItem in childItems)
					_sciRepository.Delete(childItem);

				_sciRepository.Delete(parentItem);
			}

            return cartItems.Count;
        }
        
        /// <summary>
		/// Validates required products (products which require other variant to be added to the cart)
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="product">Product</param>
		/// <param name="siteId">Site identifier</param>
		/// <param name="automaticallyAddRequiredProductsIfEnabled">Automatically add required products if enabled</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetRequiredProductWarnings(User user,
			ShoppingCartType shoppingCartType, Product product,
			int siteId, bool automaticallyAddRequiredProductsIfEnabled)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (product == null)
                throw new ArgumentNullException("product");

            var cart = user.GetCartItems(shoppingCartType, siteId);
            var warnings = new List<string>();

            if (product.RequireOtherProducts)
            {
                var requiredProducts = new List<Product>();
                foreach (var id in product.ParseRequiredProductIds())
                {
					var rp = _productService.GetProductById(id);
                    if (rp != null)
                        requiredProducts.Add(rp);
                }
                
                foreach (var rp in requiredProducts)
                {
                    //ensure that product is in the cart
                    bool alreadyInTheCart = false;
                    foreach (var sci in cart)
                    {
                        if (sci.Item.ProductId == rp.Id)
                        {
                            alreadyInTheCart = true;
                            break;
                        }
                    }
                    //not in the cart
                    if (!alreadyInTheCart)
                    {
                        if (product.AutomaticallyAddRequiredProducts)
                        {
                            //add to cart (if possible)
                            if (automaticallyAddRequiredProductsIfEnabled)
                            {
                                //pass 'false' for 'automaticallyAddRequiredProducsIfEnabled' to prevent circular references
								var addToCartWarnings = AddToCart(user, rp, shoppingCartType, siteId, "", decimal.Zero, 1, false, null);

                                if (addToCartWarnings.Count > 0)
                                {
                                    //a product wasn't atomatically added for some reasons

                                    //don't display specific errors from 'addToCartWarnings' variable
                                    //display only generic error
									warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.RequiredProductWarning"), rp.GetLocalized(x => x.Name)));
                                }
                            }
                            else
                            {
								warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.RequiredProductWarning"), rp.GetLocalized(x => x.Name)));
                            }
                        }
                        else
                        {
							warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.RequiredProductWarning"), rp.GetLocalized(x => x.Name)));
                        }
                    }
                }
            }

            return warnings;
        }
        
        /// <summary>
		/// Validates a product for standard properties
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="product">Product</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="userEnteredPrice">User entered price</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetStandardWarnings(User user, ShoppingCartType shoppingCartType,
            Product product, string selectedAttributes, decimal userEnteredPrice, int quantity)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();

            //deleted?
            if (product.Deleted)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.ProductDeleted"));
                return warnings;
            }

			// check if the product type is available for order
			if (product.ProductType == ProductType.GroupedProduct)
			{
				warnings.Add(_localizationService.GetResource("ShoppingCart.ProductNotAvailableForOrder"));
			}

			// validate bundle
			if (product.ProductType == ProductType.BundledProduct)
			{
				if (product.BundlePerItemPricing && userEnteredPrice != decimal.Zero)
					warnings.Add(_localizationService.GetResource("ShoppingCart.Bundle.NoUserEnteredPrice"));
			}

            //published?
            if (!product.Published)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.ProductUnpublished"));
            }
            
            //ACL
            if (!_aclService.Authorize(product, user))
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.ProductUnpublished"));
            }

			//Site mapping
			if (!_siteMappingService.Authorize(product, _siteContext.CurrentSite.Id))
			{
				warnings.Add(_localizationService.GetResource("ShoppingCart.ProductUnpublished"));
			}

            //disabled "add to cart" button
            if (shoppingCartType == ShoppingCartType.ShoppingCart && product.DisableBuyButton)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.BuyingDisabled"));
            }

            //disabled "add to wishlist" button
            if (shoppingCartType == ShoppingCartType.Wishlist && product.DisableWishlistButton)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.WishlistDisabled"));
            }

            //call for price
            if (shoppingCartType == ShoppingCartType.ShoppingCart && product.CallForPrice)
            {
                warnings.Add(_localizationService.GetResource("Products.CallForPrice"));
            }

            //user entered price
            if (product.UserEntersPrice)
            {
                if (userEnteredPrice < product.MinimumUserEnteredPrice ||
                    userEnteredPrice > product.MaximumUserEnteredPrice)
                {
                    decimal minimumUserEnteredPrice = _currencyService.ConvertFromPrimarySiteCurrency(product.MinimumUserEnteredPrice, _workContext.WorkingCurrency);
                    decimal maximumUserEnteredPrice = _currencyService.ConvertFromPrimarySiteCurrency(product.MaximumUserEnteredPrice, _workContext.WorkingCurrency);
                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.UserEnteredPrice.RangeError"),
                        _priceFormatter.FormatPrice(minimumUserEnteredPrice, true, false),
                        _priceFormatter.FormatPrice(maximumUserEnteredPrice, true, false)));
                }
            }

            //quantity validation
            var hasQtyWarnings = false;
            if (quantity < product.OrderMinimumQuantity)
            {
                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MinimumQuantity"), product.OrderMinimumQuantity));
                hasQtyWarnings = true;
            }
            if (quantity > product.OrderMaximumQuantity)
            {
                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumQuantity"), product.OrderMaximumQuantity));
                hasQtyWarnings = true;
            }
            var allowedQuantities = product.ParseAllowedQuatities();
            if (allowedQuantities.Length > 0 && !allowedQuantities.Contains(quantity))
            {
                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.AllowedQuantities"), string.Join(", ", allowedQuantities)));
            }

            var validateOutOfStock = shoppingCartType == ShoppingCartType.ShoppingCart || !_shoppingCartSettings.AllowOutOfStockItemsToBeAddedToWishlist;
            if (validateOutOfStock && !hasQtyWarnings)
            {
                switch (product.ManageInventoryMethod)
                {
                    case ManageInventoryMethod.DontManageStock:
                        {
                        }
                        break;
                    case ManageInventoryMethod.ManageStock:
                        {
                            if ((BackorderMode)product.BackorderMode == BackorderMode.NoBackorders)
                            {
                                if (product.StockQuantity < quantity)
                                {
                                    int maximumQuantityCanBeAdded = product.StockQuantity;
                                    if (maximumQuantityCanBeAdded <= 0)
                                        warnings.Add(_localizationService.GetResource("ShoppingCart.OutOfStock"));
                                    else
                                        warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.QuantityExceedsStock"), maximumQuantityCanBeAdded));
                                }
                            }
                        }
                        break;
                    case ManageInventoryMethod.ManageStockByAttributes:
                        {
                            var combination = product.ProductVariantAttributeCombinations
                                .FirstOrDefault(x => _productAttributeParser.AreProductAttributesEqual(x.AttributesXml, selectedAttributes));

                            if (combination != null)
                            {
								if (!combination.AllowOutOfStockOrders && combination.StockQuantity < quantity)
								{
									int maximumQuantityCanBeAdded = combination.StockQuantity;

									if (maximumQuantityCanBeAdded <= 0)
										warnings.Add(_localizationService.GetResource("ShoppingCart.OutOfStock"));
									else
										warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.QuantityExceedsStock"), maximumQuantityCanBeAdded));
								}
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            //availability dates
            bool availableStartDateError = false;
            if (product.AvailableStartDateTimeUtc.HasValue)
            {
                DateTime now = DateTime.UtcNow;
                DateTime availableStartDateTime = DateTime.SpecifyKind(product.AvailableStartDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableStartDateTime.CompareTo(now) > 0)
                {
                    warnings.Add(_localizationService.GetResource("ShoppingCart.NotAvailable"));
                    availableStartDateError = true;
                }
            }
            if (product.AvailableEndDateTimeUtc.HasValue && !availableStartDateError)
            {
                DateTime now = DateTime.UtcNow;
                DateTime availableEndDateTime = DateTime.SpecifyKind(product.AvailableEndDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableEndDateTime.CompareTo(now) < 0)
                {
                    warnings.Add(_localizationService.GetResource("ShoppingCart.NotAvailable"));
                }
            }
            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item attributes
        /// </summary>
		/// <param name="user">The user</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="product">Product</param>
        /// <param name="selectedAttributes">Selected attributes</param>
		/// <param name="quantity">Quantity</param>
		/// <param name="bundleItem">Product bundle item</param>
        /// <returns>Warnings</returns>
		public virtual IList<string> GetShoppingCartItemAttributeWarnings(User user, ShoppingCartType shoppingCartType,
			Product product, string selectedAttributes, int quantity = 1, ProductBundleItem bundleItem = null)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();

			if (product.ProductType == ProductType.BundledProduct)
				return warnings;	// user cannot select anything cause bundles have no attributes

			if (bundleItem != null && !bundleItem.BundleProduct.BundlePerItemPricing)
				return warnings;	// user cannot select anything... selectedAttribute is always empty

            //selected attributes
            var pva1Collection = _productAttributeParser.ParseProductVariantAttributes(selectedAttributes);
            foreach (var pva1 in pva1Collection)
            {
                var pv1 = pva1.Product;

				if (pv1 == null || pv1.Id != product.Id)
				{
					warnings.Add(_localizationService.GetResource("ShoppingCart.AttributeError"));
					return warnings;
				}
            }

            //existing product attributes
            var pva2Collection = product.ProductVariantAttributes;
            foreach (var pva2 in pva2Collection)
            {
                if (pva2.IsRequired)
                {
                    bool found = false;
                    //selected product attributes
                    foreach (var pva1 in pva1Collection)
                    {
                        if (pva1.Id == pva2.Id)
                        {
                            var pvaValuesStr = _productAttributeParser.ParseValues(selectedAttributes, pva1.Id);
                            foreach (string str1 in pvaValuesStr)
                            {
                                if (!String.IsNullOrEmpty(str1.Trim()))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }

					if (!found && bundleItem != null && bundleItem.FilterAttributes && !bundleItem.AttributeFilters.Any(x => x.AttributeId == pva2.ProductAttributeId))
					{
						found = true;	// attribute is filtered out on bundle item level... it cannot be selected by user
					}

                    if (!found)
                    {
						warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.SelectAttribute"),
							pva2.TextPrompt.HasValue() ? pva2.TextPrompt : pva2.ProductAttribute.GetLocalized(a => a.Name)));
                    }
                }
            }

			// check if there is a selected attribute combination and if it is active
			if (warnings.Count == 0 && selectedAttributes.HasValue())
			{
				var combination = product
					.ProductVariantAttributeCombinations
					.FirstOrDefault(x => _productAttributeParser.AreProductAttributesEqual(x.AttributesXml, selectedAttributes));

				if (combination != null && !combination.IsActive)
				{
					warnings.Add(_localizationService.GetResource("ShoppingCart.NotAvailable"));
				}
			}

			if (warnings.Count == 0)
			{
				var pvaValues = _productAttributeParser.ParseProductVariantAttributeValues(selectedAttributes);
				foreach (var pvaValue in pvaValues)
				{
					if (pvaValue.ValueType ==  ProductVariantAttributeValueType.ProductLinkage)
					{
						var linkedProduct = _productService.GetProductById(pvaValue.LinkedProductId);
						if (linkedProduct != null)
						{
							var linkageWarnings = GetShoppingCartItemWarnings(user, shoppingCartType, linkedProduct, _siteContext.CurrentSite.Id,
								"", decimal.Zero, quantity * pvaValue.Quantity, false, true, true, true, true);

							foreach (var linkageWarning in linkageWarnings)
							{
								string msg = _localizationService.GetResource("ShoppingCart.ProductLinkageAttributeWarning").FormatWith(
									pvaValue.ProductVariantAttribute.ProductAttribute.GetLocalized(a => a.Name),
									pvaValue.GetLocalized(a => a.Name),
									linkageWarning);

								warnings.Add(msg);
							}
						}
						else
						{
							string msg = _localizationService.GetResource("ShoppingCart.ProductLinkageProductNotLoading").FormatWith(pvaValue.LinkedProductId);
							warnings.Add(msg);
						}
					}
				}
			}

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item (gift card)
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="product">Product</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartItemGiftCardWarnings(ShoppingCartType shoppingCartType,
            Product product, string selectedAttributes)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();

            //gift cards
            if (product.IsGiftCard)
            {
                string giftCardRecipientName = string.Empty;
                string giftCardRecipientEmail = string.Empty;
                string giftCardSenderName = string.Empty;
                string giftCardSenderEmail = string.Empty;
                string giftCardMessage = string.Empty;

                _productAttributeParser.GetGiftCardAttribute(selectedAttributes,
                    out giftCardRecipientName, out giftCardRecipientEmail,
                    out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                if (String.IsNullOrEmpty(giftCardRecipientName))
                    warnings.Add(_localizationService.GetResource("ShoppingCart.RecipientNameError"));

                if (product.GiftCardType == GiftCardType.Virtual)
                {
                    //validate for virtual gift cards only
					if (String.IsNullOrEmpty(giftCardRecipientEmail) || !giftCardRecipientEmail.IsEmail())
                        warnings.Add(_localizationService.GetResource("ShoppingCart.RecipientEmailError"));
                }

                if (String.IsNullOrEmpty(giftCardSenderName))
                    warnings.Add(_localizationService.GetResource("ShoppingCart.SenderNameError"));

                if (product.GiftCardType == GiftCardType.Virtual)
                {
                    //validate for virtual gift cards only
					if (String.IsNullOrEmpty(giftCardSenderEmail) || !giftCardSenderEmail.IsEmail())
                        warnings.Add(_localizationService.GetResource("ShoppingCart.SenderEmailError"));
                }
            }

            return warnings;
        }

		/// <summary>
		/// Validates bundle items
		/// </summary>
		/// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="bundleItem">Product bundle item</param>
		/// <returns>Warnings</returns>
		public virtual IList<string> GetBundleItemWarnings(ProductBundleItem bundleItem)
		{
			var warnings = new List<string>();

			if (bundleItem != null)
			{
				string name = bundleItem.GetLocalizedName();

				if (!bundleItem.Published)
					warnings.Add(_localizationService.GetResource("ShoppingCart.Bundle.BundleItemUnpublished").FormatWith(name));

				if (bundleItem.ProductId == 0 || bundleItem.BundleProductId == 0 || bundleItem.Product == null || bundleItem.BundleProduct == null)
					warnings.Add(_localizationService.GetResource("ShoppingCart.Bundle.MissingProduct").FormatWith(name));

				if (bundleItem.Quantity <= 0)
					warnings.Add(_localizationService.GetResource("ShoppingCart.Bundle.Quantity").FormatWith(name));

				if (bundleItem.Product.IsDownload || bundleItem.Product.IsRecurring)
					warnings.Add(_localizationService.GetResource("ShoppingCart.Bundle.ProductResrictions").FormatWith(name));
			}

			return warnings;
		}
		public virtual IList<string> GetBundleItemWarnings(IList<OrganizedShoppingCartItem> cartItems)
		{
			var warnings = new List<string>();

			if (cartItems != null)
			{
				foreach (var sci in cartItems.Where(x => x.Item.BundleItem != null))
				{
					warnings.AddRange(GetBundleItemWarnings(sci.Item.BundleItem));
				}
			}
			return warnings;
		}
        
        /// <summary>
        /// Validates shopping cart item
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="product">Product</param>
		/// <param name="siteId">Site identifier</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="userEnteredPrice">User entered price</param>
        /// <param name="quantity">Quantity</param>
		/// <param name="automaticallyAddRequiredProductsIfEnabled">Automatically add required products if enabled</param>
        /// <param name="getStandardWarnings">A value indicating whether we should validate a product for standard properties</param>
        /// <param name="getAttributesWarnings">A value indicating whether we should validate product attributes</param>
        /// <param name="getGiftCardWarnings">A value indicating whether we should validate gift card properties</param>
        /// <param name="getRequiredProductWarnings">A value indicating whether we should validate required products (products which require other products to be added to the cart)</param>
		/// <param name="getBundleWarnings">A value indicating whether we should validate bundle and bundle items</param>
		/// <param name="bundleItem">Product bundle item if bundles should be validated</param>
		/// <param name="childItems">Child cart items to validate bundle items</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartItemWarnings(User user, ShoppingCartType shoppingCartType,
			Product product, int siteId, 
			string selectedAttributes, decimal userEnteredPrice,
			int quantity, bool automaticallyAddRequiredProductsIfEnabled,
            bool getStandardWarnings = true, bool getAttributesWarnings = true, 
            bool getGiftCardWarnings = true, bool getRequiredProductWarnings = true,
			bool getBundleWarnings = true, ProductBundleItem bundleItem = null, IList<OrganizedShoppingCartItem> childItems = null)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();
            
            //standard properties
            if (getStandardWarnings)
                warnings.AddRange(GetStandardWarnings(user, shoppingCartType, product, selectedAttributes, userEnteredPrice, quantity));

            //selected attributes
            if (getAttributesWarnings)
                warnings.AddRange(GetShoppingCartItemAttributeWarnings(user, shoppingCartType, product, selectedAttributes, quantity, bundleItem));

            //gift cards
            if (getGiftCardWarnings)
                warnings.AddRange(GetShoppingCartItemGiftCardWarnings(shoppingCartType, product, selectedAttributes));

            //required products
            if (getRequiredProductWarnings)
				warnings.AddRange(GetRequiredProductWarnings(user, shoppingCartType, product, siteId, automaticallyAddRequiredProductsIfEnabled));

			// bundle and bundle item warnings
			if (getBundleWarnings)
			{
				if (bundleItem != null)
					warnings.AddRange(GetBundleItemWarnings(bundleItem));

				if (childItems != null)
					warnings.AddRange(GetBundleItemWarnings(childItems));
			}
            
            return warnings;
        }

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="checkoutAttributes">Checkout attributes</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
        /// <returns>Warnings</returns>
		public virtual IList<string> GetShoppingCartWarnings(IList<OrganizedShoppingCartItem> shoppingCart, 
            string checkoutAttributes, bool validateCheckoutAttributes)
        {
            var warnings = new List<string>();

            bool hasStandartProducts = false;
            bool hasRecurringProducts = false;

            foreach (var sci in shoppingCart)
            {
                var product = sci.Item.Product;
                if (product == null)
                {
                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.CannotLoadProduct"), sci.Item.ProductId));
                    return warnings;
                }

                if (product.IsRecurring)
                    hasRecurringProducts = true;
                else
                    hasStandartProducts = true;
            }

            //don't mix standard and recurring products
            if (hasStandartProducts && hasRecurringProducts)
                warnings.Add(_localizationService.GetResource("ShoppingCart.CannotMixStandardAndAutoshipProducts"));

            //recurring cart validation
            if (hasRecurringProducts)
            {
                int cycleLength = 0;
                RecurringProductCyclePeriod cyclePeriod =  RecurringProductCyclePeriod.Days;
                int totalCycles = 0;
                string cyclesError = shoppingCart.GetRecurringCycleInfo(_localizationService, out cycleLength, out cyclePeriod, out totalCycles);
                if (!string.IsNullOrEmpty(cyclesError))
                {
                    warnings.Add(cyclesError);
                    return warnings;
                }
            }

            //validate checkout attributes
            if (validateCheckoutAttributes)
            {
                //selected attributes
                var ca1Collection = _checkoutAttributeParser.ParseCheckoutAttributes(checkoutAttributes);

                //existing checkout attributes
                var ca2Collection = _checkoutAttributeService.GetAllCheckoutAttributes();
                if (!shoppingCart.RequiresShipping())
                {
                    //remove attributes which require shippable products
                    ca2Collection = ca2Collection.RemoveShippableAttributes();
                }
                foreach (var ca2 in ca2Collection)
                {
                    if (ca2.IsRequired)
                    {
                        bool found = false;
                        //selected checkout attributes
                        foreach (var ca1 in ca1Collection)
                        {
                            if (ca1.Id == ca2.Id)
                            {
                                var caValuesStr = _checkoutAttributeParser.ParseValues(checkoutAttributes, ca1.Id);
                                foreach (string str1 in caValuesStr)
                                    if (!String.IsNullOrEmpty(str1.Trim()))
                                    {
                                        found = true;
                                        break;
                                    }
                            }
                        }

                        //if not found
                        if (!found)
                        {
                            if (!string.IsNullOrEmpty(ca2.GetLocalized(a => a.TextPrompt)))
                                warnings.Add(ca2.GetLocalized(a => a.TextPrompt));
                            else
                                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.SelectAttribute"), ca2.GetLocalized(a => a.Name)));
                        }
                    }
                }
            }

            return warnings;
        }

        /// <summary>
        /// Finds a shopping cart item in the cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="product">Product</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="userEnteredPrice">Price entered by a user</param>
        /// <returns>Found shopping cart item</returns>
		public virtual OrganizedShoppingCartItem FindShoppingCartItemInTheCart(IList<OrganizedShoppingCartItem> shoppingCart,
            ShoppingCartType shoppingCartType,
            Product product,
            string selectedAttributes = "",
            decimal userEnteredPrice = decimal.Zero)
        {
            if (shoppingCart == null)
                throw new ArgumentNullException("shoppingCart");

            if (product == null)
                throw new ArgumentNullException("product");

			if (product.ProductType == ProductType.BundledProduct && product.BundlePerItemPricing)
				return null;		// too complex

            foreach (var sci in shoppingCart.Where(a => a.Item.ShoppingCartType == shoppingCartType && a.Item.ParentItemId == null))
            {
                if (sci.Item.ProductId == product.Id && sci.Item.Product.ProductTypeId == product.ProductTypeId)
                {
                    //attributes
                    bool attributesEqual = _productAttributeParser.AreProductAttributesEqual(sci.Item.AttributesXml, selectedAttributes);

                    //gift cards
                    bool giftCardInfoSame = true;
                    if (sci.Item.Product.IsGiftCard)
                    {
                        string giftCardRecipientName1 = string.Empty;
                        string giftCardRecipientEmail1 = string.Empty;
                        string giftCardSenderName1 = string.Empty;
                        string giftCardSenderEmail1 = string.Empty;
                        string giftCardMessage1 = string.Empty;

                        _productAttributeParser.GetGiftCardAttribute(selectedAttributes,
                            out giftCardRecipientName1, out giftCardRecipientEmail1,
                            out giftCardSenderName1, out giftCardSenderEmail1, out giftCardMessage1);

                        string giftCardRecipientName2 = string.Empty;
                        string giftCardRecipientEmail2 = string.Empty;
                        string giftCardSenderName2 = string.Empty;
                        string giftCardSenderEmail2 = string.Empty;
                        string giftCardMessage2 = string.Empty;

                        _productAttributeParser.GetGiftCardAttribute(sci.Item.AttributesXml,
                            out giftCardRecipientName2, out giftCardRecipientEmail2,
                            out giftCardSenderName2, out giftCardSenderEmail2, out giftCardMessage2);


                        if (giftCardRecipientName1.ToLowerInvariant() != giftCardRecipientName2.ToLowerInvariant() ||
                            giftCardSenderName1.ToLowerInvariant() != giftCardSenderName2.ToLowerInvariant())
                            giftCardInfoSame = false;
                    }

                    //price is the same (for products which require users to enter a price)
                    bool userEnteredPricesEqual = true;
                    if (sci.Item.Product.UserEntersPrice)
                        userEnteredPricesEqual = Math.Round(sci.Item.UserEnteredPrice, 2) == Math.Round(userEnteredPrice, 2);

                    //found?
                    if (attributesEqual && giftCardInfoSame && userEnteredPricesEqual)
                        return sci;
                }
            }

            return null;
        }

		/// <summary>
		/// Add product to cart
		/// </summary>
		/// <param name="user">The user</param>
		/// <param name="product">The product</param>
		/// <param name="cartType">Cart type</param>
		/// <param name="siteId">Site identifier</param>
		/// <param name="selectedAttributes">Selected attributes</param>
		/// <param name="userEnteredPrice">Price entered by user</param>
		/// <param name="quantity">Quantity</param>
		/// <param name="automaticallyAddRequiredProductsIfEnabled">Whether to add required products</param>
		/// <param name="ctx">Add to cart context</param>
		/// <returns>List with warnings</returns>
		public virtual List<string> AddToCart(User user, Product product, ShoppingCartType cartType, int siteId, string selectedAttributes,
			decimal userEnteredPrice, int quantity, bool automaticallyAddRequiredProductsIfEnabled,	AddToCartContext ctx = null)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			if (product == null)
				throw new ArgumentNullException("product");

			var warnings = new List<string>();
			var bundleItem = (ctx == null ? null : ctx.BundleItem);

			if (ctx != null && bundleItem != null && ctx.Warnings.Count > 0)
				return ctx.Warnings;	// warnings while adding bundle items to cart -> no need for further processing

			if (cartType == ShoppingCartType.ShoppingCart && !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart, user))
			{
				warnings.Add("Shopping cart is disabled");
				return warnings;
			}
			if (cartType == ShoppingCartType.Wishlist && !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist, user))
			{
				warnings.Add("Wishlist is disabled");
				return warnings;
			}

			if (quantity <= 0)
			{
				warnings.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));
				return warnings;
			}

			//if (parentItemId.HasValue && (parentItemId.Value == 0 || bundleItem == null || bundleItem.Id == 0))
			//{
			//	warnings.Add(_localizationService.GetResource("ShoppingCart.Bundle.BundleItemNotFound").FormatWith(bundleItem.GetLocalizedName()));
			//	return warnings;
			//}

			//reset checkout info
			_userService.ResetCheckoutData(user, siteId);

			var cart = user.GetCartItems(cartType, siteId);
			OrganizedShoppingCartItem existingCartItem = null;

			if (bundleItem == null)
			{
				existingCartItem = FindShoppingCartItemInTheCart(cart, cartType, product, selectedAttributes, userEnteredPrice);
			}

			if (existingCartItem != null)
			{
				//update existing shopping cart item
				int newQuantity = existingCartItem.Item.Quantity + quantity;

				warnings.AddRange(
					GetShoppingCartItemWarnings(user, cartType, product, siteId, selectedAttributes, userEnteredPrice, newQuantity,
						automaticallyAddRequiredProductsIfEnabled, bundleItem: bundleItem)
				);

				if (warnings.Count == 0)
				{
					existingCartItem.Item.AttributesXml = selectedAttributes;
					existingCartItem.Item.Quantity = newQuantity;
					existingCartItem.Item.UpdatedOnUtc = DateTime.UtcNow;
					_userService.UpdateUser(user);

					//event notification
					_eventPublisher.EntityUpdated(existingCartItem.Item);
				}
			}
			else
			{
				//new shopping cart item
				warnings.AddRange(
					GetShoppingCartItemWarnings(user, cartType, product, siteId, selectedAttributes, userEnteredPrice, quantity,
						automaticallyAddRequiredProductsIfEnabled, bundleItem: bundleItem)
				);

				if (warnings.Count == 0)
				{
					//maximum items validation
					if (cartType == ShoppingCartType.ShoppingCart && cart.Count >= _shoppingCartSettings.MaximumShoppingCartItems)
					{
						warnings.Add(_localizationService.GetResource("ShoppingCart.MaximumShoppingCartItems"));
						return warnings;
					}
					else if (cartType == ShoppingCartType.Wishlist && cart.Count >= _shoppingCartSettings.MaximumWishlistItems)
					{
						warnings.Add(_localizationService.GetResource("ShoppingCart.MaximumWishlistItems"));
						return warnings;
					}

					var now = DateTime.UtcNow;
					var cartItem = new ShoppingCartItem()
					{
						ShoppingCartType = cartType,
						SiteId = siteId,
						Product = product,
						AttributesXml = selectedAttributes,
						UserEnteredPrice = userEnteredPrice,
						Quantity = quantity,
						CreatedOnUtc = now,
						UpdatedOnUtc = now,
						ParentItemId = null	//parentItemId
					};

					if (bundleItem != null)
						cartItem.BundleItemId = bundleItem.Id;


					if (ctx == null)
					{
						user.ShoppingCartItems.Add(cartItem);
						_userService.UpdateUser(user);
						_eventPublisher.EntityInserted(cartItem);
					}
					else
					{
						if (bundleItem == null)
						{
							Debug.Assert(ctx.Item == null, "Add to cart item already specified");
							ctx.Item = cartItem;
						}
						else
						{
							ctx.ChildItems.Add(cartItem);
						}
					}
				}
			}

			return warnings;
		}

		/// <summary>
		/// Add product to cart
		/// </summary>
		/// <param name="ctx">Add to cart context</param>
		public virtual void AddToCart(AddToCartContext ctx)
		{
			var user = ctx.User ?? _workContext.CurrentUser;
			int siteId = ctx.SiteId ?? _siteContext.CurrentSite.Id;
			var cart = user.GetCartItems(ctx.CartType, siteId);

			_userService.ResetCheckoutData(user, siteId);

			if (ctx.AttributeForm != null)
			{
				var attributes = _productAttributeService.GetProductVariantAttributesByProductId(ctx.Product.Id);

				ctx.Attributes = ctx.AttributeForm.CreateSelectedAttributesXml(ctx.Product.Id, attributes, _productAttributeParser, _localizationService,
					_downloadService, _catalogSettings, null, ctx.Warnings, true, ctx.BundleItemId);

				if (ctx.Product.ProductType == ProductType.BundledProduct && ctx.Attributes.HasValue())
					ctx.Warnings.Add(_localizationService.GetResource("ShoppingCart.Bundle.NoAttributes"));

				if (ctx.Product.IsGiftCard)
					ctx.Attributes = ctx.AttributeForm.AddGiftCardAttribute(ctx.Attributes, ctx.Product.Id, _productAttributeParser, ctx.BundleItemId);
			}

			ctx.Warnings.AddRange(
				AddToCart(_workContext.CurrentUser, ctx.Product, ctx.CartType, siteId,	ctx.Attributes, ctx.UserEnteredPrice, ctx.Quantity, ctx.AddRequiredProducts, ctx)
			);

			if (ctx.Product.ProductType == ProductType.BundledProduct && ctx.Warnings.Count <= 0 && ctx.BundleItem == null)
			{
				foreach (var bundleItem in _productService.GetBundleItems(ctx.Product.Id).Select(x => x.Item))
				{
					AddToCart(new AddToCartContext
					{
						BundleItem = bundleItem,
						Warnings = ctx.Warnings,
						Item = ctx.Item,
						ChildItems = ctx.ChildItems,
						Product = bundleItem.Product,
						User = user,
						AttributeForm = ctx.AttributeForm,
						CartType = ctx.CartType,
						Quantity = bundleItem.Quantity,
						AddRequiredProducts = ctx.AddRequiredProducts,
						SiteId = siteId
					});

					if (ctx.Warnings.Count > 0)
					{
						ctx.ChildItems.Clear();
						break;
					}
				}
			}

			if (ctx.BundleItem == null)
			{
				AddToCartStoring(ctx);
			}
		}

		/// <summary>
		/// Sites the shopping card items in the database
		/// </summary>
		/// <param name="ctx">Add to cart context</param>
		public virtual void AddToCartStoring(AddToCartContext ctx)
		{
			if (ctx.Warnings.Count == 0 && ctx.Item != null)
			{
				var user = ctx.User ?? _workContext.CurrentUser;

				user.ShoppingCartItems.Add(ctx.Item);
				_userService.UpdateUser(user);
				_eventPublisher.EntityInserted(ctx.Item);

				foreach (var childItem in ctx.ChildItems)
				{
					childItem.ParentItemId = ctx.Item.Id;

					user.ShoppingCartItems.Add(childItem);
					_userService.UpdateUser(user);
					_eventPublisher.EntityInserted(childItem);
				}
			}
		}

        /// <summary>
        /// Updates the shopping cart item
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartItemId">Shopping cart item identifier</param>
        /// <param name="newQuantity">New shopping cart item quantity</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> UpdateShoppingCartItem(User user, int shoppingCartItemId, int newQuantity, bool resetCheckoutData)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var warnings = new List<string>();

			var shoppingCartItem = user.ShoppingCartItems.FirstOrDefault(sci => sci.Id == shoppingCartItemId && sci.ParentItemId == null);
            if (shoppingCartItem != null)
            {
                if (resetCheckoutData)
                {
                    //reset checkout data
					_userService.ResetCheckoutData(user, shoppingCartItem.SiteId);
                }
                if (newQuantity > 0)
                {
                    //check warnings
                    warnings.AddRange(GetShoppingCartItemWarnings(user, shoppingCartItem.ShoppingCartType, shoppingCartItem.Product, shoppingCartItem.SiteId,
						shoppingCartItem.AttributesXml, shoppingCartItem.UserEnteredPrice, newQuantity, false));

                    if (warnings.Count == 0)
                    {
                        //if everything is OK, then update a shopping cart item
                        shoppingCartItem.Quantity = newQuantity;
                        shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;
                        _userService.UpdateUser(user);

                        //event notification
                        _eventPublisher.EntityUpdated(shoppingCartItem);
                    }
                }
                else
                {
                    //delete a shopping cart item
                    DeleteShoppingCartItem(shoppingCartItem, resetCheckoutData, true);
                }
            }

            return warnings;
        }
        
        /// <summary>
        /// Migrate shopping cart
        /// </summary>
        /// <param name="fromUser">From user</param>
        /// <param name="toUser">To user</param>
        public virtual void MigrateShoppingCart(User fromUser, User toUser)
        {
            if (fromUser == null)
                throw new ArgumentNullException("fromUser");

            if (toUser == null)
                throw new ArgumentNullException("toUser");

            if (fromUser.Id == toUser.Id)
                return;

			int siteId = 0;
			var cartItems = fromUser.ShoppingCartItems.ToList().Organize().ToList();

			if (cartItems.Count <= 0)
				return;

			foreach (var cartItem in cartItems)
			{
				if (siteId == 0)
					siteId = cartItem.Item.SiteId;

				Copy(cartItem, toUser, cartItem.Item.ShoppingCartType, cartItem.Item.SiteId, false);
			}

			_eventPublisher.PublishMigrateShoppingCart(fromUser, toUser, siteId);

			foreach (var cartItem in cartItems)
			{
				DeleteShoppingCartItem(cartItem.Item);
			}
        }

		/// <summary>
		/// Copies a shopping cart item.
		/// </summary>
		/// <param name="sci">Shopping cart item</param>
		/// <param name="user">The user</param>
		/// <param name="cartType">Shopping cart type</param>
		/// <param name="siteId">Site Id</param>
		/// <param name="addRequiredProductsIfEnabled">Add required products if enabled</param>
		/// <returns>List with add-to-cart warnings.</returns>
		public virtual IList<string> Copy(OrganizedShoppingCartItem sci, User user, ShoppingCartType cartType, int siteId, bool addRequiredProductsIfEnabled)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			if (sci == null)
				throw new ArgumentNullException("item");

			var addToCartContext = new AddToCartContext
			{
				User = user
			};

			addToCartContext.Warnings = AddToCart(user, sci.Item.Product, cartType, siteId, sci.Item.AttributesXml, sci.Item.UserEnteredPrice,
				sci.Item.Quantity, addRequiredProductsIfEnabled, addToCartContext);

			if (addToCartContext.Warnings.Count == 0 && sci.ChildItems != null)
			{
				foreach (var childItem in sci.ChildItems)
				{
					addToCartContext.BundleItem = childItem.Item.BundleItem;

					addToCartContext.Warnings = AddToCart(user, childItem.Item.Product, cartType, siteId, childItem.Item.AttributesXml, childItem.Item.UserEnteredPrice,
						childItem.Item.Quantity, false, addToCartContext);
				}
			}

			AddToCartStoring(addToCartContext);

			return addToCartContext.Warnings;
		}

        #endregion
    }
}
