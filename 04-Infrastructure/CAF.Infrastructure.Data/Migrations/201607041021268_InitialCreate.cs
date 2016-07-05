namespace CAF.Infrastructure.Data.Migrations
{
    using CAF.Infrastructure.Core.Data;
    using CAF.Infrastructure.Data;
    using CAF.Infrastructure.Data.Setup;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    using System.Web.Hosting;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            if (DbMigrationContext.Current.SuppressInitialCreate<DefaultObjectContext>())
                return;
            CreateTable(
                "dbo.Affiliate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddressId = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.AddressId)
                .Index(t => t.AddressId);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Company = c.String(),
                        CountryId = c.Int(),
                        StateProvinceId = c.Int(),
                        City = c.String(),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        ZipPostalCode = c.String(),
                        PhoneNumber = c.String(),
                        FaxNumber = c.String(),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Country", t => t.CountryId)
                .ForeignKey("dbo.StateProvince", t => t.StateProvinceId)
                .Index(t => t.CountryId)
                .Index(t => t.StateProvinceId);
            
            CreateTable(
                "dbo.Country",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        AllowsBilling = c.Boolean(nullable: false),
                        AllowsShipping = c.Boolean(nullable: false),
                        TwoLetterIsoCode = c.String(maxLength: 2),
                        ThreeLetterIsoCode = c.String(maxLength: 3),
                        NumericIsoCode = c.Int(nullable: false),
                        SubjectToVat = c.Boolean(nullable: false),
                        Published = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StateProvince",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CountryId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Abbreviation = c.String(maxLength: 100),
                        Published = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Country", t => t.CountryId, cascadeDelete: true)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.GenericAttribute",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        KeyGroup = c.String(nullable: false, maxLength: 400),
                        Key = c.String(nullable: false, maxLength: 400),
                        Value = c.String(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SerialRule",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 200),
                        Name = c.String(maxLength: 1000),
                        Prefix = c.String(maxLength: 200),
                        Value = c.String(maxLength: 1000),
                        DefaultValue = c.String(maxLength: 1000),
                        RandNum = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        SerialRuleFormatId = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VisitRecord",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VisitReffer = c.String(maxLength: 500),
                        VisitRefferType = c.Int(nullable: false),
                        VisitRefferKeyWork = c.String(maxLength: 100),
                        VisitURL = c.String(maxLength: 500),
                        VisitTitle = c.String(maxLength: 100),
                        VisitTimeIn = c.DateTime(nullable: false),
                        VisitTimeOut = c.DateTime(),
                        VisitIP = c.String(maxLength: 50),
                        VisitProvince = c.String(maxLength: 30),
                        VisitCity = c.String(maxLength: 30),
                        VisitBrowerType = c.String(maxLength: 50),
                        VisitResolution = c.String(maxLength: 50),
                        VisitOS = c.String(maxLength: 50),
                        FromSource = c.String(maxLength: 50),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Setting",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        Value = c.String(nullable: false),
                        SiteId = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DeliveryTime",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ColorHexValue = c.String(nullable: false, maxLength: 50),
                        DisplayLocale = c.String(maxLength: 50),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MeasureDimension",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        SystemKeyword = c.String(nullable: false, maxLength: 100),
                        Ratio = c.Decimal(nullable: false, precision: 18, scale: 8),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MeasureWeight",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        SystemKeyword = c.String(nullable: false, maxLength: 100),
                        Ratio = c.Decimal(nullable: false, precision: 18, scale: 8),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuantityUnit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 50),
                        DisplayLocale = c.String(maxLength: 50),
                        DisplayOrder = c.Int(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Language",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        LanguageCulture = c.String(nullable: false, maxLength: 20),
                        UniqueSeoCode = c.String(maxLength: 2),
                        FlagImageFileName = c.String(maxLength: 50),
                        Rtl = c.Boolean(nullable: false),
                        LimitedToSites = c.Boolean(nullable: false),
                        Published = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LocaleStringResource",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LanguageId = c.Int(nullable: false),
                        ResourceName = c.String(nullable: false, maxLength: 200),
                        ResourceValue = c.String(nullable: false),
                        IsFromPlugin = c.Boolean(),
                        IsTouched = c.Boolean(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Language", t => t.LanguageId, cascadeDelete: true)
                .Index(t => t.LanguageId);
            
            CreateTable(
                "dbo.LocalizedProperty",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        LanguageId = c.Int(nullable: false),
                        LocaleKeyGroup = c.String(nullable: false, maxLength: 400),
                        LocaleKey = c.String(nullable: false, maxLength: 400),
                        LocaleValue = c.String(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Language", t => t.LanguageId, cascadeDelete: true)
                .Index(t => t.LanguageId);
            
            CreateTable(
                "dbo.ActivityLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActivityLogTypeId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        IpAddress = c.String(maxLength: 200),
                        Comment = c.String(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivityLogType", t => t.ActivityLogTypeId, cascadeDelete: true)
                .ForeignKey("dbo.UserCertificates", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ActivityLogTypeId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ActivityLogType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SystemKeyword = c.String(nullable: false, maxLength: 100),
                        Name = c.String(nullable: false, maxLength: 200),
                        Enabled = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserCertificates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SessionKey = c.String(),
                        AppId = c.String(),
                        UserName = c.String(),
                        UserCode = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        IsAdmin = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SQLProfilerLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Query = c.String(),
                        Parameters = c.String(),
                        CommandType = c.String(maxLength: 1000),
                        TotalSeconds = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Exception = c.String(),
                        InnerException = c.String(),
                        RequestId = c.Int(nullable: false),
                        FileName = c.String(maxLength: 100),
                        CreateDate = c.DateTime(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Log",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LogLevelId = c.Int(nullable: false),
                        ShortMessage = c.String(nullable: false),
                        FullMessage = c.String(),
                        IpAddress = c.String(maxLength: 200),
                        UserId = c.Int(),
                        PageUrl = c.String(),
                        ReferrerUrl = c.String(),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        UpdatedOnUtc = c.DateTime(),
                        Frequency = c.Int(nullable: false),
                        ContentHash = c.String(maxLength: 40),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserGuid = c.Guid(nullable: false),
                        UserName = c.String(maxLength: 1000),
                        Email = c.String(maxLength: 1000),
                        Password = c.String(),
                        PasswordFormatId = c.Int(nullable: false),
                        PasswordSalt = c.String(),
                        AdminComment = c.String(),
                        IsTaxExempt = c.Boolean(nullable: false),
                        AffiliateId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        IsSystemAccount = c.Boolean(nullable: false),
                        LastIpAddress = c.String(),
                        LastLoginDateUtc = c.DateTime(),
                        LastActivityDateUtc = c.DateTime(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        CreatedUserID = c.Long(),
                        ModifiedOnUtc = c.DateTime(),
                        ModifiedUserID = c.Long(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        BillingAddress_Id = c.Int(),
                        ShippingAddress_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.BillingAddress_Id)
                .ForeignKey("dbo.Addresses", t => t.ShippingAddress_Id)
                .Index(t => t.BillingAddress_Id)
                .Index(t => t.ShippingAddress_Id);
            
            CreateTable(
                "dbo.ExternalAuthenticationRecord",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Email = c.String(),
                        ExternalIdentifier = c.String(),
                        ExternalDisplayIdentifier = c.String(),
                        OAuthToken = c.String(),
                        OAuthAccessToken = c.String(),
                        ProviderSystemName = c.String(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Forums_Post",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TopicId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Text = c.String(nullable: false),
                        IPAddress = c.String(maxLength: 100),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        UpdatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Forums_Topic", t => t.TopicId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.TopicId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Forums_Topic",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ForumId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        TopicTypeId = c.Int(nullable: false),
                        Subject = c.String(nullable: false, maxLength: 450),
                        NumPosts = c.Int(nullable: false),
                        Views = c.Int(nullable: false),
                        LastPostId = c.Int(nullable: false),
                        LastPostUserId = c.Int(nullable: false),
                        LastPostTime = c.DateTime(),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        UpdatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Forums_Forum", t => t.ForumId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.ForumId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Forums_Forum",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ForumGroupId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 200),
                        Description = c.String(),
                        NumTopics = c.Int(nullable: false),
                        NumPosts = c.Int(nullable: false),
                        LastTopicId = c.Int(nullable: false),
                        LastPostId = c.Int(nullable: false),
                        LastPostUserId = c.Int(nullable: false),
                        LastPostTime = c.DateTime(),
                        DisplayOrder = c.Int(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        UpdatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Forums_Group", t => t.ForumGroupId, cascadeDelete: true)
                .Index(t => t.ForumGroupId);
            
            CreateTable(
                "dbo.Forums_Group",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        Description = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        UpdatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderNumber = c.String(),
                        OrderGuid = c.Guid(nullable: false),
                        SiteId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        BillingAddressId = c.Int(nullable: false),
                        ShippingAddressId = c.Int(),
                        OrderStatusId = c.Int(nullable: false),
                        PaymentStatusId = c.Int(nullable: false),
                        PaymentMethodSystemName = c.String(),
                        UserCurrencyCode = c.String(),
                        CurrencyRate = c.Decimal(nullable: false, precision: 18, scale: 8),
                        UserTaxDisplayTypeId = c.Int(nullable: false),
                        VatNumber = c.String(),
                        OrderSubtotalInclTax = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrderSubtotalExclTax = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrderSubTotalDiscountInclTax = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrderSubTotalDiscountExclTax = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrderShippingInclTax = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrderShippingExclTax = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrderShippingTaxRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PaymentMethodAdditionalFeeInclTax = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PaymentMethodAdditionalFeeExclTax = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PaymentMethodAdditionalFeeTaxRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TaxRates = c.String(),
                        OrderTax = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrderDiscount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrderTotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        RefundedAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        UpdatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.BillingAddressId)
                .ForeignKey("dbo.Addresses", t => t.ShippingAddressId)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.BillingAddressId)
                .Index(t => t.ShippingAddressId);
            
            CreateTable(
                "dbo.GiftCardUsageHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GiftCardId = c.Int(nullable: false),
                        UsedWithOrderId = c.Int(nullable: false),
                        UsedValue = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GiftCard", t => t.GiftCardId, cascadeDelete: true)
                .ForeignKey("dbo.Order", t => t.UsedWithOrderId, cascadeDelete: true)
                .Index(t => t.GiftCardId)
                .Index(t => t.UsedWithOrderId);
            
            CreateTable(
                "dbo.GiftCard",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PurchasedWithOrderItemId = c.Int(),
                        GiftCardTypeId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IsGiftCardActivated = c.Boolean(nullable: false),
                        GiftCardCouponCode = c.String(),
                        RecipientName = c.String(),
                        RecipientEmail = c.String(),
                        SenderName = c.String(),
                        SenderEmail = c.String(),
                        Message = c.String(),
                        IsRecipientNotified = c.Boolean(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderItem", t => t.PurchasedWithOrderItemId)
                .Index(t => t.PurchasedWithOrderItemId);
            
            CreateTable(
                "dbo.OrderItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderItemGuid = c.Guid(nullable: false),
                        OrderId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UnitPriceInclTax = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitPriceExclTax = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PriceInclTax = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PriceExclTax = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TaxRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        DiscountAmountInclTax = c.Decimal(nullable: false, precision: 18, scale: 4),
                        DiscountAmountExclTax = c.Decimal(nullable: false, precision: 18, scale: 4),
                        AttributeDescription = c.String(),
                        AttributesXml = c.String(),
                        DownloadCount = c.Int(nullable: false),
                        IsDownloadActivated = c.Boolean(nullable: false),
                        LicenseDownloadId = c.Int(),
                        ItemWeight = c.Decimal(precision: 18, scale: 4),
                        BundleData = c.String(),
                        ProductCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Product", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductTypeId = c.Int(nullable: false),
                        ParentGroupedProductId = c.Int(nullable: false),
                        VisibleIndividually = c.Boolean(nullable: false),
                        Name = c.String(nullable: false, maxLength: 400),
                        ShortDescription = c.String(),
                        FullDescription = c.String(),
                        AdminComment = c.String(),
                        ProductTemplateId = c.Int(nullable: false),
                        ShowOnHomePage = c.Boolean(nullable: false),
                        MetaKeywords = c.String(maxLength: 400),
                        MetaDescription = c.String(),
                        MetaTitle = c.String(maxLength: 400),
                        AllowCustomerReviews = c.Boolean(nullable: false),
                        ApprovedRatingSum = c.Int(nullable: false),
                        NotApprovedRatingSum = c.Int(nullable: false),
                        ApprovedTotalReviews = c.Int(nullable: false),
                        NotApprovedTotalReviews = c.Int(nullable: false),
                        SubjectToAcl = c.Boolean(nullable: false),
                        LimitedToSites = c.Boolean(nullable: false),
                        Sku = c.String(maxLength: 400),
                        ManufacturerPartNumber = c.String(maxLength: 400),
                        Gtin = c.String(maxLength: 400),
                        IsGiftCard = c.Boolean(nullable: false),
                        GiftCardTypeId = c.Int(nullable: false),
                        RequireOtherProducts = c.Boolean(nullable: false),
                        RequiredProductIds = c.String(maxLength: 1000),
                        AutomaticallyAddRequiredProducts = c.Boolean(nullable: false),
                        IsDownload = c.Boolean(nullable: false),
                        DownloadId = c.Int(nullable: false),
                        UnlimitedDownloads = c.Boolean(nullable: false),
                        MaxNumberOfDownloads = c.Int(nullable: false),
                        DownloadExpirationDays = c.Int(),
                        DownloadActivationTypeId = c.Int(nullable: false),
                        HasSampleDownload = c.Boolean(nullable: false),
                        SampleDownloadId = c.Int(),
                        HasUserAgreement = c.Boolean(nullable: false),
                        UserAgreementText = c.String(),
                        IsRecurring = c.Boolean(nullable: false),
                        RecurringCycleLength = c.Int(nullable: false),
                        RecurringCyclePeriodId = c.Int(nullable: false),
                        RecurringTotalCycles = c.Int(nullable: false),
                        IsShipEnabled = c.Boolean(nullable: false),
                        IsFreeShipping = c.Boolean(nullable: false),
                        AdditionalShippingCharge = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IsTaxExempt = c.Boolean(nullable: false),
                        IsEsd = c.Boolean(nullable: false),
                        TaxCategoryId = c.Int(nullable: false),
                        ManageInventoryMethodId = c.Int(nullable: false),
                        StockQuantity = c.Int(nullable: false),
                        DisplayStockAvailability = c.Boolean(nullable: false),
                        DisplayStockQuantity = c.Boolean(nullable: false),
                        MinStockQuantity = c.Int(nullable: false),
                        LowStockActivityId = c.Int(nullable: false),
                        NotifyAdminForQuantityBelow = c.Int(nullable: false),
                        BackorderModeId = c.Int(nullable: false),
                        AllowBackInStockSubscriptions = c.Boolean(nullable: false),
                        OrderMinimumQuantity = c.Int(nullable: false),
                        OrderMaximumQuantity = c.Int(nullable: false),
                        AllowedQuantities = c.String(maxLength: 1000),
                        DisableBuyButton = c.Boolean(nullable: false),
                        DisableWishlistButton = c.Boolean(nullable: false),
                        AvailableForPreOrder = c.Boolean(nullable: false),
                        CallForPrice = c.Boolean(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OldPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ProductCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SpecialPrice = c.Decimal(precision: 18, scale: 4),
                        SpecialPriceStartDateTimeUtc = c.DateTime(),
                        SpecialPriceEndDateTimeUtc = c.DateTime(),
                        CustomerEntersPrice = c.Boolean(nullable: false),
                        MinimumCustomerEnteredPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MaximumCustomerEnteredPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        HasTierPrices = c.Boolean(nullable: false),
                        LowestAttributeCombinationPrice = c.Decimal(precision: 18, scale: 4),
                        HasDiscountsApplied = c.Boolean(nullable: false),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Length = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Width = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Height = c.Decimal(nullable: false, precision: 18, scale: 4),
                        AvailableStartDateTimeUtc = c.DateTime(),
                        AvailableEndDateTimeUtc = c.DateTime(),
                        DisplayOrder = c.Int(nullable: false),
                        Published = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        UpdatedOnUtc = c.DateTime(nullable: false),
                        DeliveryTimeId = c.Int(),
                        QuantityUnitId = c.Int(),
                        BasePriceEnabled = c.Boolean(nullable: false),
                        BasePriceMeasureUnit = c.String(maxLength: 50),
                        BasePriceAmount = c.Decimal(precision: 18, scale: 4),
                        BasePriceBaseAmount = c.Int(),
                        BundleTitleText = c.String(maxLength: 400),
                        BundlePerItemShipping = c.Boolean(nullable: false),
                        BundlePerItemPricing = c.Boolean(nullable: false),
                        BundlePerItemShoppingCart = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DeliveryTime", t => t.DeliveryTimeId)
                .ForeignKey("dbo.QuantityUnit", t => t.QuantityUnitId)
                .ForeignKey("dbo.Download", t => t.SampleDownloadId)
                .Index(t => t.SampleDownloadId)
                .Index(t => t.DeliveryTimeId)
                .Index(t => t.QuantityUnitId);
            
            CreateTable(
                "dbo.Download",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DownloadGuid = c.Guid(nullable: false),
                        UseDownloadUrl = c.Boolean(nullable: false),
                        DownloadUrl = c.String(),
                        DownloadBinary = c.Binary(),
                        ContentType = c.String(),
                        Filename = c.String(),
                        Extension = c.String(),
                        IsNew = c.Boolean(nullable: false),
                        IsTransient = c.Boolean(nullable: false),
                        UpdatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.UpdatedOnUtc, t.IsTransient }, name: "IX_UpdatedOn_IsTransient");
            
            CreateTable(
                "dbo.OrderNote",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        Note = c.String(nullable: false),
                        DisplayToCustomer = c.Boolean(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.ShoppingCartItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteId = c.Int(nullable: false),
                        ParentItemId = c.Int(),
                        BundleItemId = c.Int(),
                        ShoppingCartTypeId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        AttributesXml = c.String(),
                        UserEnteredPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Quantity = c.Int(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        UpdatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Product", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.UserContent",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        IpAddress = c.String(maxLength: 200),
                        IsApproved = c.Boolean(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        CreatedUserID = c.Long(),
                        ModifiedOnUtc = c.DateTime(),
                        ModifiedUserID = c.Long(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Article",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ArticleGuid = c.Guid(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 100),
                        IsPasswordProtected = c.Boolean(nullable: false),
                        Password = c.String(),
                        LinkUrl = c.String(maxLength: 50),
                        ImgUrl = c.String(maxLength: 50),
                        PictureId = c.Int(),
                        MetaKeywords = c.String(maxLength: 255),
                        MetaDescription = c.String(maxLength: 500),
                        MetaTitle = c.String(maxLength: 50),
                        ShortContent = c.String(maxLength: 255),
                        FullContent = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        Click = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        GroupidsView = c.String(maxLength: 255),
                        VoteId = c.Int(nullable: false),
                        IsTop = c.Boolean(nullable: false),
                        IsRed = c.Boolean(nullable: false),
                        IsHot = c.Boolean(nullable: false),
                        IsSlide = c.Boolean(nullable: false),
                        IsSys = c.Boolean(nullable: false),
                        Author = c.String(maxLength: 50),
                        IsDownload = c.Boolean(nullable: false),
                        DownloadId = c.Int(nullable: false),
                        UnlimitedDownloads = c.Boolean(nullable: false),
                        MaxNumberOfDownloads = c.Int(nullable: false),
                        DownloadCount = c.Int(nullable: false),
                        AllowComments = c.Boolean(nullable: false),
                        AllowUserReviews = c.Boolean(nullable: false),
                        ApprovedRatingSum = c.Int(nullable: false),
                        NotApprovedRatingSum = c.Int(nullable: false),
                        ApprovedTotalReviews = c.Int(nullable: false),
                        NotApprovedTotalReviews = c.Int(nullable: false),
                        ApprovedCommentCount = c.Int(nullable: false),
                        NotApprovedCommentCount = c.Int(nullable: false),
                        ModelTemplateId = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        LimitedToSites = c.Boolean(nullable: false),
                        SubjectToAcl = c.Boolean(nullable: false),
                        StartDateUtc = c.DateTime(),
                        EndDateUtc = c.DateTime(),
                        Rating = c.Int(nullable: false),
                        HelpfulYesTotal = c.Int(nullable: false),
                        HelpfulNoTotal = c.Int(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        CreatedUserID = c.Long(),
                        ModifiedOnUtc = c.DateTime(),
                        ModifiedUserID = c.Long(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ArticleCategory", t => t.CategoryId)
                .ForeignKey("dbo.Picture", t => t.PictureId)
                .Index(t => t.CategoryId)
                .Index(t => t.PictureId);
            
            CreateTable(
                "dbo.Article_Picture_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ArticleId = c.Int(nullable: false),
                        PictureId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Article", t => t.ArticleId, cascadeDelete: true)
                .ForeignKey("dbo.Picture", t => t.PictureId, cascadeDelete: true)
                .Index(t => t.ArticleId)
                .Index(t => t.PictureId);
            
            CreateTable(
                "dbo.Picture",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PictureBinary = c.Binary(),
                        MimeType = c.String(nullable: false, maxLength: 40),
                        SeoFilename = c.String(maxLength: 300),
                        IsNew = c.Boolean(nullable: false),
                        IsTransient = c.Boolean(nullable: false),
                        UpdatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.UpdatedOnUtc, t.IsTransient }, name: "IX_UpdatedOn_IsTransient");
            
            CreateTable(
                "dbo.ArticleAttach",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ArticleId = c.Int(nullable: false),
                        FileName = c.String(maxLength: 50),
                        FilePath = c.String(maxLength: 50),
                        FileSize = c.Int(nullable: false),
                        FileExt = c.String(maxLength: 10),
                        DownNum = c.Int(nullable: false),
                        Point = c.Int(nullable: false),
                        AddTime = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Article", t => t.ArticleId)
                .Index(t => t.ArticleId);
            
            CreateTable(
                "dbo.ArticleCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 400),
                        Alias = c.String(maxLength: 100),
                        FullName = c.String(maxLength: 400),
                        ParentCategoryId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        LinkUrl = c.String(maxLength: 50),
                        PictureId = c.Int(),
                        Description = c.String(),
                        BottomDescription = c.String(),
                        MetaTitle = c.String(maxLength: 50),
                        MetaKeywords = c.String(maxLength: 255),
                        MetaDescription = c.String(maxLength: 500),
                        Published = c.Boolean(nullable: false),
                        ShowOnHomePage = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        SubjectToAcl = c.Boolean(nullable: false),
                        LimitedToSites = c.Boolean(nullable: false),
                        ModelTemplateId = c.Int(nullable: false),
                        DetailModelTemplateId = c.Int(nullable: false),
                        ChannelId = c.Int(nullable: false),
                        DefaultViewMode = c.String(),
                        PageSize = c.Int(nullable: false),
                        AllowUsersToSelectPageSize = c.Boolean(nullable: false),
                        PageSizeOptions = c.String(maxLength: 200),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        CreatedUserID = c.Long(),
                        ModifiedOnUtc = c.DateTime(),
                        ModifiedUserID = c.Long(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Channel", t => t.ChannelId, cascadeDelete: true)
                .ForeignKey("dbo.Picture", t => t.PictureId)
                .Index(t => t.PictureId)
                .Index(t => t.ChannelId);
            
            CreateTable(
                "dbo.ArticleCategoryExtend",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        Name = c.String(maxLength: 50),
                        Title = c.String(maxLength: 50),
                        ControlType = c.String(maxLength: 50),
                        DataType = c.String(maxLength: 50),
                        DataLength = c.Int(nullable: false),
                        DataPlace = c.Int(nullable: false),
                        ItemOption = c.String(maxLength: 50),
                        DefaultValue = c.String(maxLength: 50),
                        IsRequired = c.Boolean(nullable: false),
                        IsPassword = c.Boolean(nullable: false),
                        IsHtml = c.Boolean(nullable: false),
                        EditorType = c.Int(nullable: false),
                        ValidTipMsg = c.String(maxLength: 50),
                        ValidErrorMsg = c.String(maxLength: 50),
                        ValidPattern = c.String(maxLength: 50),
                        SortId = c.Int(nullable: false),
                        IsSys = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ArticleCategory", t => t.CategoryId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Channel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChannelGuid = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        Title = c.String(maxLength: 100),
                        DisplayOrder = c.Int(nullable: false),
                        ModelTemplateId = c.Int(nullable: false),
                        DetailModelTemplateId = c.Int(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        CreatedUserID = c.Long(),
                        ModifiedOnUtc = c.DateTime(),
                        ModifiedUserID = c.Long(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExtendedAttribute",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 400),
                        Title = c.String(nullable: false, maxLength: 400),
                        TextPrompt = c.String(),
                        IsRequired = c.Boolean(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        AttributeControlTypeId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExtendedAttributeValue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExtendedAttributeId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 400),
                        IsPreSelected = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ExtendedAttribute", t => t.ExtendedAttributeId, cascadeDelete: true)
                .Index(t => t.ExtendedAttributeId);
            
            CreateTable(
                "dbo.ArticleExtend",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Value = c.String(maxLength: 50),
                        Type = c.String(maxLength: 500),
                        SortId = c.Int(nullable: false),
                        ArticleId = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Article", t => t.ArticleId)
                .Index(t => t.ArticleId);
            
            CreateTable(
                "dbo.ArticleTag",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 400),
                        IsSys = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PollAnswer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PollId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        NumberOfVotes = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Poll", t => t.PollId, cascadeDelete: true)
                .Index(t => t.PollId);
            
            CreateTable(
                "dbo.Poll",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LanguageId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        SystemKeyword = c.String(),
                        Published = c.Boolean(nullable: false),
                        ShowOnHomePage = c.Boolean(nullable: false),
                        AllowGuestsToVote = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        StartDateUtc = c.DateTime(),
                        EndDateUtc = c.DateTime(),
                        LimitedToSites = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Language", t => t.LanguageId, cascadeDelete: true)
                .Index(t => t.LanguageId);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        FreeShipping = c.Boolean(nullable: false),
                        TaxExempt = c.Boolean(nullable: false),
                        TaxDisplayType = c.Int(),
                        Active = c.Boolean(nullable: false),
                        IsSystemRole = c.Boolean(nullable: false),
                        SystemName = c.String(maxLength: 255),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PermissionRecord",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 255),
                        Category = c.String(nullable: false, maxLength: 255),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Campaign",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Subject = c.String(nullable: false),
                        Body = c.String(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmailAccount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 255),
                        DisplayName = c.String(maxLength: 255),
                        Host = c.String(nullable: false, maxLength: 255),
                        Port = c.Int(nullable: false),
                        Username = c.String(nullable: false, maxLength: 255),
                        Password = c.String(nullable: false, maxLength: 255),
                        EnableSsl = c.Boolean(nullable: false),
                        UseDefaultCredentials = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MessageTemplate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        BccEmailAddresses = c.String(maxLength: 200),
                        Subject = c.String(maxLength: 1000),
                        Body = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        EmailAccountId = c.Int(nullable: false),
                        LimitedToSites = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NewsLetterSubscription",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NewsLetterSubscriptionGuid = c.Guid(nullable: false),
                        Email = c.String(nullable: false, maxLength: 255),
                        Active = c.Boolean(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        SiteId = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QueuedEmail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Priority = c.Int(nullable: false),
                        From = c.String(nullable: false, maxLength: 500),
                        FromName = c.String(maxLength: 500),
                        To = c.String(nullable: false, maxLength: 500),
                        ToName = c.String(maxLength: 500),
                        ReplyTo = c.String(maxLength: 500),
                        ReplyToName = c.String(maxLength: 500),
                        CC = c.String(maxLength: 500),
                        Bcc = c.String(maxLength: 500),
                        Subject = c.String(maxLength: 1000),
                        Body = c.String(),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        SentTries = c.Int(nullable: false),
                        SentOnUtc = c.DateTime(),
                        EmailAccountId = c.Int(nullable: false),
                        SendManually = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailAccount", t => t.EmailAccountId, cascadeDelete: true)
                .Index(t => t.EmailAccountId);
            
            CreateTable(
                "dbo.QueuedEmailAttachments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QueuedEmailId = c.Int(nullable: false),
                        StorageLocation = c.Int(nullable: false),
                        Path = c.String(),
                        FileId = c.Int(),
                        Data = c.Binary(),
                        Name = c.String(),
                        MimeType = c.String(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Download", t => t.FileId)
                .ForeignKey("dbo.QueuedEmail", t => t.QueuedEmailId, cascadeDelete: true)
                .Index(t => t.QueuedEmailId)
                .Index(t => t.FileId);
            
            CreateTable(
                "dbo.AclRecord",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        EntityName = c.String(nullable: false, maxLength: 400),
                        UserRoleId = c.Int(nullable: false),
                        IsIdle = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UrlRecord",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        EntityName = c.String(nullable: false, maxLength: 400),
                        Slug = c.String(nullable: false, maxLength: 400),
                        IsActive = c.Boolean(nullable: false),
                        LanguageId = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Site",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteKey = c.String(maxLength: 200),
                        Code = c.String(maxLength: 200),
                        Name = c.String(maxLength: 1000),
                        Email = c.String(maxLength: 1000),
                        Tel = c.String(maxLength: 200),
                        Manager = c.String(maxLength: 200),
                        Url = c.String(nullable: false, maxLength: 400),
                        Hosts = c.String(maxLength: 1000),
                        LogoPictureId = c.Int(nullable: false),
                        SslEnabled = c.Boolean(nullable: false),
                        SecureUrl = c.String(),
                        ContentDeliveryNetwork = c.String(),
                        Icon = c.String(maxLength: 200),
                        Enabled = c.Boolean(nullable: false),
                        AllowEdit = c.Boolean(nullable: false),
                        AllowDelete = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Description = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        HtmlBodyId = c.String(),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        CreatedUserID = c.Long(),
                        ModifiedOnUtc = c.DateTime(),
                        ModifiedUserID = c.Long(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteMapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        EntityName = c.String(nullable: false, maxLength: 400),
                        SiteId = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ScheduleTask",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 500),
                        Alias = c.String(maxLength: 500),
                        CronExpression = c.String(maxLength: 1000),
                        Type = c.String(nullable: false, maxLength: 800),
                        Enabled = c.Boolean(nullable: false),
                        StopOnError = c.Boolean(nullable: false),
                        NextRunUtc = c.DateTime(),
                        LastStartUtc = c.DateTime(),
                        LastEndUtc = c.DateTime(),
                        LastSuccessUtc = c.DateTime(),
                        LastError = c.String(maxLength: 1000),
                        IsHidden = c.Boolean(nullable: false),
                        ProgressPercent = c.Int(),
                        ProgressMessage = c.String(maxLength: 1000),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Type)
                .Index(t => new { t.NextRunUtc, t.Enabled }, name: "IX_NextRun_Enabled")
                .Index(t => new { t.LastStartUtc, t.LastEndUtc }, name: "IX_LastStart_LastEnd");
            
            CreateTable(
                "dbo.TaxCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 400),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ThemeVariable",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Theme = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        Value = c.String(maxLength: 2000),
                        SiteId = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ArticleAttribute",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        KeyGroup = c.String(nullable: false, maxLength: 400),
                        Key = c.String(nullable: false, maxLength: 400),
                        Value = c.String(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ArticleCategoryMapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        EntityName = c.String(nullable: false, maxLength: 400),
                        ArticleCategoryId = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FeedbackMap",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        Content = c.String(maxLength: 500),
                        UserName = c.String(maxLength: 50),
                        UserTel = c.String(maxLength: 50),
                        UserQQ = c.String(maxLength: 50),
                        UserEmail = c.String(maxLength: 50),
                        AddTime = c.DateTime(nullable: false),
                        ReplyContent = c.String(maxLength: 500),
                        ReplyTime = c.DateTime(nullable: false),
                        IsLock = c.Boolean(nullable: false),
                        IPAddress = c.String(maxLength: 50),
                        IsPass = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Link",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Intro = c.String(maxLength: 255),
                        LinkUrl = c.String(maxLength: 255),
                        LogoUrl = c.String(maxLength: 255),
                        SortId = c.Int(nullable: false),
                        IsHome = c.Boolean(nullable: false),
                        PictureId = c.Int(),
                        LanguageId = c.Int(nullable: false),
                        LimitedToSites = c.Boolean(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        CreatedUserID = c.Long(),
                        ModifiedOnUtc = c.DateTime(),
                        ModifiedUserID = c.Long(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Picture", t => t.PictureId)
                .Index(t => t.PictureId);
            
            CreateTable(
                "dbo.ModelTemplate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 400),
                        ViewPath = c.String(nullable: false, maxLength: 400),
                        DisplayOrder = c.Int(nullable: false),
                        TemplageTypeId = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RelatedArticle",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ArticleId1 = c.Int(nullable: false),
                        ArticleId2 = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Client",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 400),
                        Description = c.String(),
                        MetaKeywords = c.String(maxLength: 400),
                        MetaDescription = c.String(),
                        MetaTitle = c.String(maxLength: 400),
                        PictureId = c.Int(),
                        LimitedToSites = c.Boolean(nullable: false),
                        Published = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        CreatedUserID = c.Long(),
                        ModifiedOnUtc = c.DateTime(),
                        ModifiedUserID = c.Long(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Picture", t => t.PictureId)
                .Index(t => t.PictureId);
            
            CreateTable(
                "dbo.Forums_Subscription",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubscriptionGuid = c.Guid(nullable: false),
                        UserId = c.Int(nullable: false),
                        ForumId = c.Int(nullable: false),
                        TopicId = c.Int(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Forums_PrivateMessage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteId = c.Int(nullable: false),
                        FromUserId = c.Int(nullable: false),
                        ToUserId = c.Int(nullable: false),
                        Subject = c.String(nullable: false, maxLength: 450),
                        Text = c.String(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        IsDeletedByAuthor = c.Boolean(nullable: false),
                        IsDeletedByRecipient = c.Boolean(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.FromUserId)
                .ForeignKey("dbo.User", t => t.ToUserId)
                .Index(t => t.FromUserId)
                .Index(t => t.ToUserId);
            
            CreateTable(
                "dbo.RegionalContent",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SystemName = c.String(nullable: false, maxLength: 40),
                        Name = c.String(maxLength: 100),
                        DisplayOrder = c.Int(nullable: false),
                        Body = c.String(),
                        LanguageId = c.Int(nullable: false),
                        LimitedToSites = c.Boolean(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        CreatedUserID = c.Long(),
                        ModifiedOnUtc = c.DateTime(),
                        ModifiedUserID = c.Long(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Topic",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SystemName = c.String(),
                        IncludeInSitemap = c.Boolean(nullable: false),
                        IsPasswordProtected = c.Boolean(nullable: false),
                        Password = c.String(),
                        Title = c.String(),
                        Body = c.String(),
                        MetaKeywords = c.String(),
                        MetaDescription = c.String(),
                        MetaTitle = c.String(),
                        LimitedToSites = c.Boolean(nullable: false),
                        RenderAsWidget = c.Boolean(nullable: false),
                        WidgetZone = c.String(),
                        WidgetShowTitle = c.Boolean(nullable: false),
                        WidgetBordered = c.Boolean(nullable: false),
                        Priority = c.Int(nullable: false),
                        TitleTag = c.String(),
                        TopicTemplateId = c.Int(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        CreatedUserID = c.Long(),
                        ModifiedOnUtc = c.DateTime(),
                        ModifiedUserID = c.Long(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserAddresses",
                c => new
                    {
                        User_Id = c.Int(nullable: false),
                        Address_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Address_Id })
                .ForeignKey("dbo.User", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.Addresses", t => t.Address_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Address_Id);
            
            CreateTable(
                "dbo.Channel_ExtendedAttribute_Mapping",
                c => new
                    {
                        Channel_Id = c.Int(nullable: false),
                        ExtendedAttribute_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Channel_Id, t.ExtendedAttribute_Id })
                .ForeignKey("dbo.Channel", t => t.Channel_Id, cascadeDelete: true)
                .ForeignKey("dbo.ExtendedAttribute", t => t.ExtendedAttribute_Id, cascadeDelete: true)
                .Index(t => t.Channel_Id)
                .Index(t => t.ExtendedAttribute_Id);
            
            CreateTable(
                "dbo.Article_ArticleTag_Mapping",
                c => new
                    {
                        Article_Id = c.Int(nullable: false),
                        ArticleTag_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Article_Id, t.ArticleTag_Id })
                .ForeignKey("dbo.Article", t => t.Article_Id, cascadeDelete: true)
                .ForeignKey("dbo.ArticleTag", t => t.ArticleTag_Id, cascadeDelete: true)
                .Index(t => t.Article_Id)
                .Index(t => t.ArticleTag_Id);
            
            CreateTable(
                "dbo.PermissionRecord_Role_Mapping",
                c => new
                    {
                        PermissionRecord_Id = c.Int(nullable: false),
                        UserRole_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PermissionRecord_Id, t.UserRole_Id })
                .ForeignKey("dbo.PermissionRecord", t => t.PermissionRecord_Id, cascadeDelete: true)
                .ForeignKey("dbo.UserRole", t => t.UserRole_Id, cascadeDelete: true)
                .Index(t => t.PermissionRecord_Id)
                .Index(t => t.UserRole_Id);
            
            CreateTable(
                "dbo.User_UserRole_Mapping",
                c => new
                    {
                        User_Id = c.Int(nullable: false),
                        UserRole_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.UserRole_Id })
                .ForeignKey("dbo.User", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.UserRole", t => t.UserRole_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.UserRole_Id);
            
            CreateTable(
                "dbo.ArticleComment",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ArticleId = c.Int(nullable: false),
                        ParentId = c.Int(nullable: false),
                        CommentTitle = c.String(),
                        CommentText = c.String(),
                        AddTime = c.DateTime(nullable: false),
                        IsReply = c.Boolean(nullable: false),
                        ReplyContent = c.String(),
                        ReplyTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserContent", t => t.Id)
                .ForeignKey("dbo.Article", t => t.ArticleId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.ArticleId);
            
            CreateTable(
                "dbo.ArticleReviewHelpfulness",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ArticleReviewId = c.Int(nullable: false),
                        WasHelpful = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserContent", t => t.Id)
                .ForeignKey("dbo.ArticleReview", t => t.ArticleReviewId)
                .Index(t => t.Id)
                .Index(t => t.ArticleReviewId);
            
            CreateTable(
                "dbo.ArticleReview",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ArticleId = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                        HelpfulYesTotal = c.Int(nullable: false),
                        HelpfulNoTotal = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserContent", t => t.Id)
                .ForeignKey("dbo.Article", t => t.ArticleId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.ArticleId);
            
            CreateTable(
                "dbo.PollVotingRecord",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        PollAnswerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserContent", t => t.Id)
                .ForeignKey("dbo.PollAnswer", t => t.PollAnswerId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.PollAnswerId);
            #region Custom

            this.SqlFileOrResource("Indexes.sql");
            if (HostingEnvironment.IsHosted && DataSettings.Current.IsSqlServer)
            {
                // do not execute in unit tests
                this.SqlFileOrResource("Indexes.SqlServer.sql");
                this.SqlFileOrResource("StoredProcedures.sql");
            }

            #endregion
        }
        
        public override void Down()
        {
            #region Custom

            this.SqlFileOrResource("Indexes.Inverse.sql");
            if (HostingEnvironment.IsHosted && DataSettings.Current.IsSqlServer)
            {
                // do not execute in unit tests
                this.SqlFileOrResource("Indexes.SqlServer.Inverse.sql");
                this.SqlFileOrResource("StoredProcedures.Inverse.sql");
            }

            #endregion
            DropForeignKey("dbo.PollVotingRecord", "PollAnswerId", "dbo.PollAnswer");
            DropForeignKey("dbo.PollVotingRecord", "Id", "dbo.UserContent");
            DropForeignKey("dbo.ArticleReview", "ArticleId", "dbo.Article");
            DropForeignKey("dbo.ArticleReview", "Id", "dbo.UserContent");
            DropForeignKey("dbo.ArticleReviewHelpfulness", "ArticleReviewId", "dbo.ArticleReview");
            DropForeignKey("dbo.ArticleReviewHelpfulness", "Id", "dbo.UserContent");
            DropForeignKey("dbo.ArticleComment", "ArticleId", "dbo.Article");
            DropForeignKey("dbo.ArticleComment", "Id", "dbo.UserContent");
            DropForeignKey("dbo.Forums_PrivateMessage", "ToUserId", "dbo.User");
            DropForeignKey("dbo.Forums_PrivateMessage", "FromUserId", "dbo.User");
            DropForeignKey("dbo.Forums_Subscription", "UserId", "dbo.User");
            DropForeignKey("dbo.Client", "PictureId", "dbo.Picture");
            DropForeignKey("dbo.Link", "PictureId", "dbo.Picture");
            DropForeignKey("dbo.QueuedEmail", "EmailAccountId", "dbo.EmailAccount");
            DropForeignKey("dbo.QueuedEmailAttachments", "QueuedEmailId", "dbo.QueuedEmail");
            DropForeignKey("dbo.QueuedEmailAttachments", "FileId", "dbo.Download");
            DropForeignKey("dbo.Log", "UserId", "dbo.User");
            DropForeignKey("dbo.User_UserRole_Mapping", "UserRole_Id", "dbo.UserRole");
            DropForeignKey("dbo.User_UserRole_Mapping", "User_Id", "dbo.User");
            DropForeignKey("dbo.PermissionRecord_Role_Mapping", "UserRole_Id", "dbo.UserRole");
            DropForeignKey("dbo.PermissionRecord_Role_Mapping", "PermissionRecord_Id", "dbo.PermissionRecord");
            DropForeignKey("dbo.PollAnswer", "PollId", "dbo.Poll");
            DropForeignKey("dbo.Poll", "LanguageId", "dbo.Language");
            DropForeignKey("dbo.Article", "PictureId", "dbo.Picture");
            DropForeignKey("dbo.Article_ArticleTag_Mapping", "ArticleTag_Id", "dbo.ArticleTag");
            DropForeignKey("dbo.Article_ArticleTag_Mapping", "Article_Id", "dbo.Article");
            DropForeignKey("dbo.ArticleExtend", "ArticleId", "dbo.Article");
            DropForeignKey("dbo.Article", "CategoryId", "dbo.ArticleCategory");
            DropForeignKey("dbo.ArticleCategory", "PictureId", "dbo.Picture");
            DropForeignKey("dbo.ArticleCategory", "ChannelId", "dbo.Channel");
            DropForeignKey("dbo.Channel_ExtendedAttribute_Mapping", "ExtendedAttribute_Id", "dbo.ExtendedAttribute");
            DropForeignKey("dbo.Channel_ExtendedAttribute_Mapping", "Channel_Id", "dbo.Channel");
            DropForeignKey("dbo.ExtendedAttributeValue", "ExtendedAttributeId", "dbo.ExtendedAttribute");
            DropForeignKey("dbo.ArticleCategoryExtend", "CategoryId", "dbo.ArticleCategory");
            DropForeignKey("dbo.ArticleAttach", "ArticleId", "dbo.Article");
            DropForeignKey("dbo.Article_Picture_Mapping", "PictureId", "dbo.Picture");
            DropForeignKey("dbo.Article_Picture_Mapping", "ArticleId", "dbo.Article");
            DropForeignKey("dbo.UserContent", "UserId", "dbo.User");
            DropForeignKey("dbo.ShoppingCartItem", "UserId", "dbo.User");
            DropForeignKey("dbo.ShoppingCartItem", "ProductId", "dbo.Product");
            DropForeignKey("dbo.User", "ShippingAddress_Id", "dbo.Addresses");
            DropForeignKey("dbo.Order", "UserId", "dbo.User");
            DropForeignKey("dbo.Order", "ShippingAddressId", "dbo.Addresses");
            DropForeignKey("dbo.OrderNote", "OrderId", "dbo.Order");
            DropForeignKey("dbo.GiftCardUsageHistory", "UsedWithOrderId", "dbo.Order");
            DropForeignKey("dbo.GiftCardUsageHistory", "GiftCardId", "dbo.GiftCard");
            DropForeignKey("dbo.GiftCard", "PurchasedWithOrderItemId", "dbo.OrderItem");
            DropForeignKey("dbo.OrderItem", "ProductId", "dbo.Product");
            DropForeignKey("dbo.Product", "SampleDownloadId", "dbo.Download");
            DropForeignKey("dbo.Product", "QuantityUnitId", "dbo.QuantityUnit");
            DropForeignKey("dbo.Product", "DeliveryTimeId", "dbo.DeliveryTime");
            DropForeignKey("dbo.OrderItem", "OrderId", "dbo.Order");
            DropForeignKey("dbo.Order", "BillingAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Forums_Post", "UserId", "dbo.User");
            DropForeignKey("dbo.Forums_Post", "TopicId", "dbo.Forums_Topic");
            DropForeignKey("dbo.Forums_Topic", "UserId", "dbo.User");
            DropForeignKey("dbo.Forums_Topic", "ForumId", "dbo.Forums_Forum");
            DropForeignKey("dbo.Forums_Forum", "ForumGroupId", "dbo.Forums_Group");
            DropForeignKey("dbo.ExternalAuthenticationRecord", "UserId", "dbo.User");
            DropForeignKey("dbo.User", "BillingAddress_Id", "dbo.Addresses");
            DropForeignKey("dbo.UserAddresses", "Address_Id", "dbo.Addresses");
            DropForeignKey("dbo.UserAddresses", "User_Id", "dbo.User");
            DropForeignKey("dbo.ActivityLog", "UserId", "dbo.UserCertificates");
            DropForeignKey("dbo.ActivityLog", "ActivityLogTypeId", "dbo.ActivityLogType");
            DropForeignKey("dbo.LocalizedProperty", "LanguageId", "dbo.Language");
            DropForeignKey("dbo.LocaleStringResource", "LanguageId", "dbo.Language");
            DropForeignKey("dbo.Affiliate", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Addresses", "StateProvinceId", "dbo.StateProvince");
            DropForeignKey("dbo.Addresses", "CountryId", "dbo.Country");
            DropForeignKey("dbo.StateProvince", "CountryId", "dbo.Country");
            DropIndex("dbo.PollVotingRecord", new[] { "PollAnswerId" });
            DropIndex("dbo.PollVotingRecord", new[] { "Id" });
            DropIndex("dbo.ArticleReview", new[] { "ArticleId" });
            DropIndex("dbo.ArticleReview", new[] { "Id" });
            DropIndex("dbo.ArticleReviewHelpfulness", new[] { "ArticleReviewId" });
            DropIndex("dbo.ArticleReviewHelpfulness", new[] { "Id" });
            DropIndex("dbo.ArticleComment", new[] { "ArticleId" });
            DropIndex("dbo.ArticleComment", new[] { "Id" });
            DropIndex("dbo.User_UserRole_Mapping", new[] { "UserRole_Id" });
            DropIndex("dbo.User_UserRole_Mapping", new[] { "User_Id" });
            DropIndex("dbo.PermissionRecord_Role_Mapping", new[] { "UserRole_Id" });
            DropIndex("dbo.PermissionRecord_Role_Mapping", new[] { "PermissionRecord_Id" });
            DropIndex("dbo.Article_ArticleTag_Mapping", new[] { "ArticleTag_Id" });
            DropIndex("dbo.Article_ArticleTag_Mapping", new[] { "Article_Id" });
            DropIndex("dbo.Channel_ExtendedAttribute_Mapping", new[] { "ExtendedAttribute_Id" });
            DropIndex("dbo.Channel_ExtendedAttribute_Mapping", new[] { "Channel_Id" });
            DropIndex("dbo.UserAddresses", new[] { "Address_Id" });
            DropIndex("dbo.UserAddresses", new[] { "User_Id" });
            DropIndex("dbo.Forums_PrivateMessage", new[] { "ToUserId" });
            DropIndex("dbo.Forums_PrivateMessage", new[] { "FromUserId" });
            DropIndex("dbo.Forums_Subscription", new[] { "UserId" });
            DropIndex("dbo.Client", new[] { "PictureId" });
            DropIndex("dbo.Link", new[] { "PictureId" });
            DropIndex("dbo.ScheduleTask", "IX_LastStart_LastEnd");
            DropIndex("dbo.ScheduleTask", "IX_NextRun_Enabled");
            DropIndex("dbo.ScheduleTask", new[] { "Type" });
            DropIndex("dbo.QueuedEmailAttachments", new[] { "FileId" });
            DropIndex("dbo.QueuedEmailAttachments", new[] { "QueuedEmailId" });
            DropIndex("dbo.QueuedEmail", new[] { "EmailAccountId" });
            DropIndex("dbo.Poll", new[] { "LanguageId" });
            DropIndex("dbo.PollAnswer", new[] { "PollId" });
            DropIndex("dbo.ArticleExtend", new[] { "ArticleId" });
            DropIndex("dbo.ExtendedAttributeValue", new[] { "ExtendedAttributeId" });
            DropIndex("dbo.ArticleCategoryExtend", new[] { "CategoryId" });
            DropIndex("dbo.ArticleCategory", new[] { "ChannelId" });
            DropIndex("dbo.ArticleCategory", new[] { "PictureId" });
            DropIndex("dbo.ArticleAttach", new[] { "ArticleId" });
            DropIndex("dbo.Picture", "IX_UpdatedOn_IsTransient");
            DropIndex("dbo.Article_Picture_Mapping", new[] { "PictureId" });
            DropIndex("dbo.Article_Picture_Mapping", new[] { "ArticleId" });
            DropIndex("dbo.Article", new[] { "PictureId" });
            DropIndex("dbo.Article", new[] { "CategoryId" });
            DropIndex("dbo.UserContent", new[] { "UserId" });
            DropIndex("dbo.ShoppingCartItem", new[] { "ProductId" });
            DropIndex("dbo.ShoppingCartItem", new[] { "UserId" });
            DropIndex("dbo.OrderNote", new[] { "OrderId" });
            DropIndex("dbo.Download", "IX_UpdatedOn_IsTransient");
            DropIndex("dbo.Product", new[] { "QuantityUnitId" });
            DropIndex("dbo.Product", new[] { "DeliveryTimeId" });
            DropIndex("dbo.Product", new[] { "SampleDownloadId" });
            DropIndex("dbo.OrderItem", new[] { "ProductId" });
            DropIndex("dbo.OrderItem", new[] { "OrderId" });
            DropIndex("dbo.GiftCard", new[] { "PurchasedWithOrderItemId" });
            DropIndex("dbo.GiftCardUsageHistory", new[] { "UsedWithOrderId" });
            DropIndex("dbo.GiftCardUsageHistory", new[] { "GiftCardId" });
            DropIndex("dbo.Order", new[] { "ShippingAddressId" });
            DropIndex("dbo.Order", new[] { "BillingAddressId" });
            DropIndex("dbo.Order", new[] { "UserId" });
            DropIndex("dbo.Forums_Forum", new[] { "ForumGroupId" });
            DropIndex("dbo.Forums_Topic", new[] { "UserId" });
            DropIndex("dbo.Forums_Topic", new[] { "ForumId" });
            DropIndex("dbo.Forums_Post", new[] { "UserId" });
            DropIndex("dbo.Forums_Post", new[] { "TopicId" });
            DropIndex("dbo.ExternalAuthenticationRecord", new[] { "UserId" });
            DropIndex("dbo.User", new[] { "ShippingAddress_Id" });
            DropIndex("dbo.User", new[] { "BillingAddress_Id" });
            DropIndex("dbo.Log", new[] { "UserId" });
            DropIndex("dbo.ActivityLog", new[] { "UserId" });
            DropIndex("dbo.ActivityLog", new[] { "ActivityLogTypeId" });
            DropIndex("dbo.LocalizedProperty", new[] { "LanguageId" });
            DropIndex("dbo.LocaleStringResource", new[] { "LanguageId" });
            DropIndex("dbo.StateProvince", new[] { "CountryId" });
            DropIndex("dbo.Addresses", new[] { "StateProvinceId" });
            DropIndex("dbo.Addresses", new[] { "CountryId" });
            DropIndex("dbo.Affiliate", new[] { "AddressId" });
            DropTable("dbo.PollVotingRecord");
            DropTable("dbo.ArticleReview");
            DropTable("dbo.ArticleReviewHelpfulness");
            DropTable("dbo.ArticleComment");
            DropTable("dbo.User_UserRole_Mapping");
            DropTable("dbo.PermissionRecord_Role_Mapping");
            DropTable("dbo.Article_ArticleTag_Mapping");
            DropTable("dbo.Channel_ExtendedAttribute_Mapping");
            DropTable("dbo.UserAddresses");
            DropTable("dbo.Topic");
            DropTable("dbo.RegionalContent");
            DropTable("dbo.Forums_PrivateMessage");
            DropTable("dbo.Forums_Subscription");
            DropTable("dbo.Client");
            DropTable("dbo.RelatedArticle");
            DropTable("dbo.ModelTemplate");
            DropTable("dbo.Link");
            DropTable("dbo.FeedbackMap");
            DropTable("dbo.ArticleCategoryMapping");
            DropTable("dbo.ArticleAttribute");
            DropTable("dbo.ThemeVariable");
            DropTable("dbo.TaxCategory");
            DropTable("dbo.ScheduleTask");
            DropTable("dbo.SiteMapping");
            DropTable("dbo.Site");
            DropTable("dbo.UrlRecord");
            DropTable("dbo.AclRecord");
            DropTable("dbo.QueuedEmailAttachments");
            DropTable("dbo.QueuedEmail");
            DropTable("dbo.NewsLetterSubscription");
            DropTable("dbo.MessageTemplate");
            DropTable("dbo.EmailAccount");
            DropTable("dbo.Campaign");
            DropTable("dbo.PermissionRecord");
            DropTable("dbo.UserRole");
            DropTable("dbo.Poll");
            DropTable("dbo.PollAnswer");
            DropTable("dbo.ArticleTag");
            DropTable("dbo.ArticleExtend");
            DropTable("dbo.ExtendedAttributeValue");
            DropTable("dbo.ExtendedAttribute");
            DropTable("dbo.Channel");
            DropTable("dbo.ArticleCategoryExtend");
            DropTable("dbo.ArticleCategory");
            DropTable("dbo.ArticleAttach");
            DropTable("dbo.Picture");
            DropTable("dbo.Article_Picture_Mapping");
            DropTable("dbo.Article");
            DropTable("dbo.UserContent");
            DropTable("dbo.ShoppingCartItem");
            DropTable("dbo.OrderNote");
            DropTable("dbo.Download");
            DropTable("dbo.Product");
            DropTable("dbo.OrderItem");
            DropTable("dbo.GiftCard");
            DropTable("dbo.GiftCardUsageHistory");
            DropTable("dbo.Order");
            DropTable("dbo.Forums_Group");
            DropTable("dbo.Forums_Forum");
            DropTable("dbo.Forums_Topic");
            DropTable("dbo.Forums_Post");
            DropTable("dbo.ExternalAuthenticationRecord");
            DropTable("dbo.User");
            DropTable("dbo.Log");
            DropTable("dbo.SQLProfilerLog");
            DropTable("dbo.UserCertificates");
            DropTable("dbo.ActivityLogType");
            DropTable("dbo.ActivityLog");
            DropTable("dbo.LocalizedProperty");
            DropTable("dbo.LocaleStringResource");
            DropTable("dbo.Language");
            DropTable("dbo.QuantityUnit");
            DropTable("dbo.MeasureWeight");
            DropTable("dbo.MeasureDimension");
            DropTable("dbo.DeliveryTime");
            DropTable("dbo.Setting");
            DropTable("dbo.VisitRecord");
            DropTable("dbo.SerialRule");
            DropTable("dbo.GenericAttribute");
            DropTable("dbo.StateProvince");
            DropTable("dbo.Country");
            DropTable("dbo.Addresses");
            DropTable("dbo.Affiliate");
        }
    }
}
