using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Utilities;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.Infrastructure.Core.Domain.Tax;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Email;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.Infrastructure.Core.Domain.Cms.Topic;
using CAF.Infrastructure.Core.Domain.Seo;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Cms.Payments;
using CAF.Infrastructure.Core.Domain.Cms.Forums;
using CAF.Infrastructure.Core.Domain.Cms;
using CAF.Infrastructure.Core.Domain.Themes;
using CAF.Infrastructure.Core.Domain.Logging;
using CAF.Infrastructure.Core.Domain.Tasks;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using CAF.Infrastructure.Core.Domain.Cms.Polls;

namespace CAF.Infrastructure.Data.Setup
{
    public abstract class InvariantSeedData
    {
        private DefaultObjectContext _ctx;
        private string _sampleImagesPath;
        private string _sampleDownloadsPath;

        protected InvariantSeedData()
        {
        }

        public void Initialize(DefaultObjectContext context)
        {
            this._ctx = context;

            this._sampleImagesPath = CommonHelper.MapPath("~/content/samples/");
            this._sampleDownloadsPath = CommonHelper.MapPath("~/content/samples/");
        }

        #region Mandatory data creators

        public IList<Picture> Pictures()
        {
            var entities = new List<Picture> 
			{ 
				    CreatePicture(File.ReadAllBytes(_sampleImagesPath + "company_logo.png"), "image/png", GetSeName("company-logo")),
			        CreatePicture(File.ReadAllBytes(_sampleImagesPath + "clouds.png"), "image/png", GetSeName("slider-bg")),
                    CreatePicture(File.ReadAllBytes(_sampleImagesPath + "slide-1.png"), "image/png", GetSeName("slide-1")),
				    CreatePicture(File.ReadAllBytes(_sampleImagesPath + "slide-2.png"), "image/png", GetSeName("slide-2")),
				    CreatePicture(File.ReadAllBytes(_sampleImagesPath + "slide-3.png"), "image/png", GetSeName("slide-3")),
            };
            this.Alter(entities);
            return entities;
        }

        public IList<Site> Sites()
        {
            var seName = GetSeName("company-logo");
            var imgCompanyLogo = _ctx.Set<Picture>().Where(x => x.SeoFilename == seName).FirstOrDefault();



            var entities = new List<Site>()
			{
				new Site()
				{
					Name = "Your store name",
					Url = "http://www.yourSite.com/",
					Hosts = "yourstore.com,www.yourstore.com",
					SslEnabled = false,
					DisplayOrder = 1,
					LogoPictureId = imgCompanyLogo.Id,
                    CreatedOnUtc=DateTime .Now,
                    ModifiedOnUtc=DateTime .Now
				}
			};
            this.Alter(entities);
            return entities;
        }

        public IList<MeasureDimension> MeasureDimensions()
        {
            var entities = new List<MeasureDimension>()
			{
				new MeasureDimension()
				{
					Name = "millimetre",
					SystemKeyword = "mm",
					Ratio = 25.4M,
					DisplayOrder = 1,
				},
				new MeasureDimension()
				{
					Name = "centimetre",
					SystemKeyword = "cm",
					Ratio = 0.254M,
					DisplayOrder = 2,
				},
				new MeasureDimension()
				{
					Name = "meter",
					SystemKeyword = "m",
					Ratio = 0.0254M,
					DisplayOrder = 3,
				},
				new MeasureDimension()
				{
					Name = "in",
					SystemKeyword = "inch",
					Ratio = 1M,
					DisplayOrder = 4,
				},
				new MeasureDimension()
				{
					Name = "feet",
					SystemKeyword = "ft",
					Ratio = 0.08333333M,
					DisplayOrder = 5,
				}
			};

            this.Alter(entities);
            return entities;
        }

        public IList<MeasureWeight> MeasureWeights()
        {
            var entities = new List<MeasureWeight>()
			{
				new MeasureWeight()
				{
					Name = "ounce", // Ounce, Unze
					SystemKeyword = "oz",
					Ratio = 16M,
					DisplayOrder = 5,
				},
				new MeasureWeight()
				{
					Name = "lb", // Pound
					SystemKeyword = "lb",
					Ratio = 1M,
					DisplayOrder = 6,
				},

				new MeasureWeight()
				{
					Name = "kg",
					SystemKeyword = "kg",
					Ratio = 0.45359237M,
					DisplayOrder = 1,
				},
				new MeasureWeight()
				{
					Name = "gram",
					SystemKeyword = "g",
					Ratio = 453.59237M,
					DisplayOrder = 2,
				},
				new MeasureWeight()
				{
					Name = "liter",
					SystemKeyword = "l",
					Ratio = 0.45359237M,
					DisplayOrder = 3,
				},
				new MeasureWeight()
				{
					Name = "milliliter",
					SystemKeyword = "ml",
					Ratio = 0.45359237M,
					DisplayOrder = 4,
				}
			};

            this.Alter(entities);
            return entities;
        }

        protected virtual string TaxNameBooks
        {
            get { return "Books"; }
        }

        protected virtual string TaxNameDigitalGoods
        {
            get { return "Downloadable Products"; }
        }

        protected virtual string TaxNameJewelry
        {
            get { return "Jewelry"; }
        }

        protected virtual string TaxNameApparel
        {
            get { return "Apparel & Shoes"; }
        }

        protected virtual string TaxNameFood
        {
            get { return "Food"; }
        }

        protected virtual string TaxNameElectronics
        {
            get { return "Electronics & Software"; }
        }

        protected virtual string TaxNameTaxFree
        {
            get { return "Tax free"; }
        }

        public virtual decimal[] FixedTaxRates
        {
            get { return new decimal[] { 0, 0, 0, 0, 0 }; }
        }

        public IList<TaxCategory> TaxCategories()
        {
            var entities = new List<TaxCategory>
			{
				new TaxCategory
				{
					Name = this.TaxNameBooks,
					DisplayOrder = 1,
				},
				new TaxCategory
				{
					Name = this.TaxNameElectronics,
					DisplayOrder = 5,
				},
				new TaxCategory
				{
					Name = this.TaxNameDigitalGoods,
					DisplayOrder = 10,
				},
				new TaxCategory
				{
					Name = this.TaxNameJewelry,
					DisplayOrder = 15,
				},
				new TaxCategory
				{
					Name = this.TaxNameApparel,
					DisplayOrder = 20,
				},
			};

            this.Alter(entities);
            return entities;
        }

        //public IList<Currency> Currencies()
        //{
        //    var entities = new List<Currency>()
        //    {
        //        CreateCurrency("en-US", published: true, rate: 1M, order: 0),
        //        CreateCurrency("en-GB", published: true, rate: 0.61M, order: 5),
        //        CreateCurrency("en-AU", published: true, rate: 0.94M, order: 10),
        //        CreateCurrency("en-CA", published: true, rate: 0.98M, order: 15),
        //        CreateCurrency("de-DE", rate: 0.79M, order: 20/*, formatting: string.Format("0.00 {0}", "\u20ac")*/),
        //        CreateCurrency("de-CH", rate: 0.93M, order: 25, formatting: "CHF #,##0.00"),
        //        CreateCurrency("zh-CN", rate: 6.48M, order: 30),
        //        CreateCurrency("zh-HK", rate: 7.75M, order: 35),
        //        CreateCurrency("ja-JP", rate: 80.07M, order: 40),
        //        CreateCurrency("ru-RU", rate: 27.7M, order: 45),
        //        CreateCurrency("tr-TR", rate: 1.78M, order: 50),
        //        CreateCurrency("sv-SE", rate: 6.19M, order: 55)
        //    };

        //    this.Alter(entities);
        //    return entities;
        //}

        public IList<Country> Countries()
        {
            var cUsa = new Country
            {
                Name = "United States",
                AllowsBilling = true,
                AllowsShipping = true,
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 840,
                SubjectToVat = false,
                DisplayOrder = 1,
                Published = true,
            };

            #region US Regions

            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "AA (Armed Forces Americas)",
                Abbreviation = "AA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "AE (Armed Forces Europe)",
                Abbreviation = "AE",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Alabama",
                Abbreviation = "AL",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Alaska",
                Abbreviation = "AK",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "American Samoa",
                Abbreviation = "AS",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "AP (Armed Forces Pacific)",
                Abbreviation = "AP",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Arizona",
                Abbreviation = "AZ",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Arkansas",
                Abbreviation = "AR",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "California",
                Abbreviation = "CA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Colorado",
                Abbreviation = "CO",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Connecticut",
                Abbreviation = "CT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Delaware",
                Abbreviation = "DE",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "District of Columbia",
                Abbreviation = "DC",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Federated States of Micronesia",
                Abbreviation = "FM",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Florida",
                Abbreviation = "FL",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Georgia",
                Abbreviation = "GA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Guam",
                Abbreviation = "GU",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Hawaii",
                Abbreviation = "HI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Idaho",
                Abbreviation = "ID",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Illinois",
                Abbreviation = "IL",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Indiana",
                Abbreviation = "IN",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Iowa",
                Abbreviation = "IA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Kansas",
                Abbreviation = "KS",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Kentucky",
                Abbreviation = "KY",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Louisiana",
                Abbreviation = "LA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Maine",
                Abbreviation = "ME",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Marshall Islands",
                Abbreviation = "MH",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Maryland",
                Abbreviation = "MD",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Massachusetts",
                Abbreviation = "MA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Michigan",
                Abbreviation = "MI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Minnesota",
                Abbreviation = "MN",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Mississippi",
                Abbreviation = "MS",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Missouri",
                Abbreviation = "MO",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Montana",
                Abbreviation = "MT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Nebraska",
                Abbreviation = "NE",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Nevada",
                Abbreviation = "NV",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "New Hampshire",
                Abbreviation = "NH",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "New Jersey",
                Abbreviation = "NJ",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "New Mexico",
                Abbreviation = "NM",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "New York",
                Abbreviation = "NY",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "North Carolina",
                Abbreviation = "NC",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "North Dakota",
                Abbreviation = "ND",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Northern Mariana Islands",
                Abbreviation = "MP",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Ohio",
                Abbreviation = "OH",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Oklahoma",
                Abbreviation = "OK",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Oregon",
                Abbreviation = "OR",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Palau",
                Abbreviation = "PW",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Pennsylvania",
                Abbreviation = "PA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Puerto Rico",
                Abbreviation = "PR",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Rhode Island",
                Abbreviation = "RI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "South Carolina",
                Abbreviation = "SC",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "South Dakota",
                Abbreviation = "SD",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Tennessee",
                Abbreviation = "TN",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Texas",
                Abbreviation = "TX",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Utah",
                Abbreviation = "UT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Vermont",
                Abbreviation = "VT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Virgin Islands",
                Abbreviation = "VI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Virginia",
                Abbreviation = "VA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Washington",
                Abbreviation = "WA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "West Virginia",
                Abbreviation = "WV",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Wisconsin",
                Abbreviation = "WI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Wyoming",
                Abbreviation = "WY",
                Published = true,
                DisplayOrder = 1,
            });

            #endregion

            var cCanada = new Country
            {
                Name = "Canada",
                AllowsBilling = true,
                AllowsShipping = true,
                TwoLetterIsoCode = "CA",
                ThreeLetterIsoCode = "CAN",
                NumericIsoCode = 124,
                SubjectToVat = false,
                DisplayOrder = 2,
                Published = true,
            };

            #region CA Regions

            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Alberta",
                Abbreviation = "AB",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "British Columbia",
                Abbreviation = "BC",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Manitoba",
                Abbreviation = "MB",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "New Brunswick",
                Abbreviation = "NB",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Newfoundland and Labrador",
                Abbreviation = "NL",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Northwest Territories",
                Abbreviation = "NT",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Nova Scotia",
                Abbreviation = "NS",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Nunavut",
                Abbreviation = "NU",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Ontario",
                Abbreviation = "ON",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Prince Edward Island",
                Abbreviation = "PE",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Quebec",
                Abbreviation = "QC",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Saskatchewan",
                Abbreviation = "SK",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Yukon Territory",
                Abbreviation = "YU",
                Published = true,
                DisplayOrder = 1,
            });
            #endregion

            var entities = new List<Country>()
			{
				new Country()
				{
					Name = "Germany",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "DE",
					ThreeLetterIsoCode = "DEU",
					NumericIsoCode = 276,
					SubjectToVat = true,
					DisplayOrder = -10,
					Published = true
				},

				new Country
				{
					Name = "Austria",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "AT",
					ThreeLetterIsoCode = "AUT",
					NumericIsoCode = 40,
					SubjectToVat = true,
					DisplayOrder = -5,
					Published = true
				},
				new Country
				{
					Name = "Switzerland",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "CH",
					ThreeLetterIsoCode = "CHE",
					NumericIsoCode = 756,
					SubjectToVat = false,
					DisplayOrder = -1,
					Published = true
				},
			    cUsa,
				cCanada,

				//other countries
				new Country
				{
					Name = "Argentina",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "AR",
					ThreeLetterIsoCode = "ARG",
					NumericIsoCode = 32,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Armenia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "AM",
					ThreeLetterIsoCode = "ARM",
					NumericIsoCode = 51,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Aruba",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "AW",
					ThreeLetterIsoCode = "ABW",
					NumericIsoCode = 533,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Australia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "AU",
					ThreeLetterIsoCode = "AUS",
					NumericIsoCode = 36,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Azerbaijan",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "AZ",
					ThreeLetterIsoCode = "AZE",
					NumericIsoCode = 31,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Bahamas",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BS",
					ThreeLetterIsoCode = "BHS",
					NumericIsoCode = 44,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Bangladesh",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BD",
					ThreeLetterIsoCode = "BGD",
					NumericIsoCode = 50,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Belarus",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BY",
					ThreeLetterIsoCode = "BLR",
					NumericIsoCode = 112,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Belgium",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BE",
					ThreeLetterIsoCode = "BEL",
					NumericIsoCode = 56,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Belize",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BZ",
					ThreeLetterIsoCode = "BLZ",
					NumericIsoCode = 84,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Bermuda",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BM",
					ThreeLetterIsoCode = "BMU",
					NumericIsoCode = 60,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Bolivia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BO",
					ThreeLetterIsoCode = "BOL",
					NumericIsoCode = 68,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Bosnia and Herzegowina",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BA",
					ThreeLetterIsoCode = "BIH",
					NumericIsoCode = 70,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Brazil",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BR",
					ThreeLetterIsoCode = "BRA",
					NumericIsoCode = 76,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Bulgaria",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BG",
					ThreeLetterIsoCode = "BGR",
					NumericIsoCode = 100,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Cayman Islands",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "KY",
					ThreeLetterIsoCode = "CYM",
					NumericIsoCode = 136,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Chile",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "CL",
					ThreeLetterIsoCode = "CHL",
					NumericIsoCode = 152,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "China",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "CN",
					ThreeLetterIsoCode = "CHN",
					NumericIsoCode = 156,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Colombia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "CO",
					ThreeLetterIsoCode = "COL",
					NumericIsoCode = 170,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Costa Rica",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "CR",
					ThreeLetterIsoCode = "CRI",
					NumericIsoCode = 188,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Croatia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "HR",
					ThreeLetterIsoCode = "HRV",
					NumericIsoCode = 191,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Cuba",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "CU",
					ThreeLetterIsoCode = "CUB",
					NumericIsoCode = 192,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Cyprus",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "CY",
					ThreeLetterIsoCode = "CYP",
					NumericIsoCode = 196,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Czech Republic",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "CZ",
					ThreeLetterIsoCode = "CZE",
					NumericIsoCode = 203,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Denmark",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "DK",
					ThreeLetterIsoCode = "DNK",
					NumericIsoCode = 208,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Dominican Republic",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "DO",
					ThreeLetterIsoCode = "DOM",
					NumericIsoCode = 214,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Ecuador",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "EC",
					ThreeLetterIsoCode = "ECU",
					NumericIsoCode = 218,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Egypt",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "EG",
					ThreeLetterIsoCode = "EGY",
					NumericIsoCode = 818,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Finland",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "FI",
					ThreeLetterIsoCode = "FIN",
					NumericIsoCode = 246,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "France",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "FR",
					ThreeLetterIsoCode = "FRA",
					NumericIsoCode = 250,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Georgia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GE",
					ThreeLetterIsoCode = "GEO",
					NumericIsoCode = 268,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Gibraltar",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GI",
					ThreeLetterIsoCode = "GIB",
					NumericIsoCode = 292,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Greece",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GR",
					ThreeLetterIsoCode = "GRC",
					NumericIsoCode = 300,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Guatemala",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GT",
					ThreeLetterIsoCode = "GTM",
					NumericIsoCode = 320,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Hong Kong",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "HK",
					ThreeLetterIsoCode = "HKG",
					NumericIsoCode = 344,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Hungary",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "HU",
					ThreeLetterIsoCode = "HUN",
					NumericIsoCode = 348,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "India",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "IN",
					ThreeLetterIsoCode = "IND",
					NumericIsoCode = 356,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Indonesia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "ID",
					ThreeLetterIsoCode = "IDN",
					NumericIsoCode = 360,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Ireland",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "IE",
					ThreeLetterIsoCode = "IRL",
					NumericIsoCode = 372,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Israel",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "IL",
					ThreeLetterIsoCode = "ISR",
					NumericIsoCode = 376,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Italy",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "IT",
					ThreeLetterIsoCode = "ITA",
					NumericIsoCode = 380,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Jamaica",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "JM",
					ThreeLetterIsoCode = "JAM",
					NumericIsoCode = 388,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Japan",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "JP",
					ThreeLetterIsoCode = "JPN",
					NumericIsoCode = 392,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Jordan",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "JO",
					ThreeLetterIsoCode = "JOR",
					NumericIsoCode = 400,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Kazakhstan",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "KZ",
					ThreeLetterIsoCode = "KAZ",
					NumericIsoCode = 398,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Korea, Democratic People's Republic of",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "KP",
					ThreeLetterIsoCode = "PRK",
					NumericIsoCode = 408,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Kuwait",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "KW",
					ThreeLetterIsoCode = "KWT",
					NumericIsoCode = 414,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Malaysia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MY",
					ThreeLetterIsoCode = "MYS",
					NumericIsoCode = 458,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Mexico",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MX",
					ThreeLetterIsoCode = "MEX",
					NumericIsoCode = 484,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Netherlands",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "NL",
					ThreeLetterIsoCode = "NLD",
					NumericIsoCode = 528,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "New Zealand",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "NZ",
					ThreeLetterIsoCode = "NZL",
					NumericIsoCode = 554,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Norway",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "NO",
					ThreeLetterIsoCode = "NOR",
					NumericIsoCode = 578,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Pakistan",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "PK",
					ThreeLetterIsoCode = "PAK",
					NumericIsoCode = 586,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Paraguay",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "PY",
					ThreeLetterIsoCode = "PRY",
					NumericIsoCode = 600,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Peru",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "PE",
					ThreeLetterIsoCode = "PER",
					NumericIsoCode = 604,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Philippines",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "PH",
					ThreeLetterIsoCode = "PHL",
					NumericIsoCode = 608,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Poland",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "PL",
					ThreeLetterIsoCode = "POL",
					NumericIsoCode = 616,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Portugal",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "PT",
					ThreeLetterIsoCode = "PRT",
					NumericIsoCode = 620,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Puerto Rico",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "PR",
					ThreeLetterIsoCode = "PRI",
					NumericIsoCode = 630,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Qatar",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "QA",
					ThreeLetterIsoCode = "QAT",
					NumericIsoCode = 634,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Romania",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "RO",
					ThreeLetterIsoCode = "ROM",
					NumericIsoCode = 642,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Russia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "RU",
					ThreeLetterIsoCode = "RUS",
					NumericIsoCode = 643,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Saudi Arabia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SA",
					ThreeLetterIsoCode = "SAU",
					NumericIsoCode = 682,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Singapore",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SG",
					ThreeLetterIsoCode = "SGP",
					NumericIsoCode = 702,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Slovakia (Slovak Republic)",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SK",
					ThreeLetterIsoCode = "SVK",
					NumericIsoCode = 703,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Slovenia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SI",
					ThreeLetterIsoCode = "SVN",
					NumericIsoCode = 705,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "South Africa",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "ZA",
					ThreeLetterIsoCode = "ZAF",
					NumericIsoCode = 710,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Spain",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "ES",
					ThreeLetterIsoCode = "ESP",
					NumericIsoCode = 724,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Sweden",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SE",
					ThreeLetterIsoCode = "SWE",
					NumericIsoCode = 752,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Taiwan",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "TW",
					ThreeLetterIsoCode = "TWN",
					NumericIsoCode = 158,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Thailand",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "TH",
					ThreeLetterIsoCode = "THA",
					NumericIsoCode = 764,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Turkey",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "TR",
					ThreeLetterIsoCode = "TUR",
					NumericIsoCode = 792,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Ukraine",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "UA",
					ThreeLetterIsoCode = "UKR",
					NumericIsoCode = 804,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "United Arab Emirates",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "AE",
					ThreeLetterIsoCode = "ARE",
					NumericIsoCode = 784,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "United Kingdom",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GB",
					ThreeLetterIsoCode = "GBR",
					NumericIsoCode = 826,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "United States minor outlying islands",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "UM",
					ThreeLetterIsoCode = "UMI",
					NumericIsoCode = 581,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Uruguay",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "UY",
					ThreeLetterIsoCode = "URY",
					NumericIsoCode = 858,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Uzbekistan",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "UZ",
					ThreeLetterIsoCode = "UZB",
					NumericIsoCode = 860,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Venezuela",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "VE",
					ThreeLetterIsoCode = "VEN",
					NumericIsoCode = 862,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Serbia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "RS",
					ThreeLetterIsoCode = "SRB",
					NumericIsoCode = 688,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Afghanistan",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "AF",
					ThreeLetterIsoCode = "AFG",
					NumericIsoCode = 4,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Albania",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "AL",
					ThreeLetterIsoCode = "ALB",
					NumericIsoCode = 8,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Algeria",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "DZ",
					ThreeLetterIsoCode = "DZA",
					NumericIsoCode = 12,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "American Samoa",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "AS",
					ThreeLetterIsoCode = "ASM",
					NumericIsoCode = 16,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Andorra",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "AD",
					ThreeLetterIsoCode = "AND",
					NumericIsoCode = 20,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Angola",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "AO",
					ThreeLetterIsoCode = "AGO",
					NumericIsoCode = 24,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Anguilla",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "AI",
					ThreeLetterIsoCode = "AIA",
					NumericIsoCode = 660,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Antarctica",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "AQ",
					ThreeLetterIsoCode = "ATA",
					NumericIsoCode = 10,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Antigua and Barbuda",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "AG",
					ThreeLetterIsoCode = "ATG",
					NumericIsoCode = 28,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Bahrain",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BH",
					ThreeLetterIsoCode = "BHR",
					NumericIsoCode = 48,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Barbados",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BB",
					ThreeLetterIsoCode = "BRB",
					NumericIsoCode = 52,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Benin",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BJ",
					ThreeLetterIsoCode = "BEN",
					NumericIsoCode = 204,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Bhutan",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BT",
					ThreeLetterIsoCode = "BTN",
					NumericIsoCode = 64,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Botswana",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BW",
					ThreeLetterIsoCode = "BWA",
					NumericIsoCode = 72,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Bouvet Island",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BV",
					ThreeLetterIsoCode = "BVT",
					NumericIsoCode = 74,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "British Indian Ocean Territory",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "IO",
					ThreeLetterIsoCode = "IOT",
					NumericIsoCode = 86,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Brunei Darussalam",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BN",
					ThreeLetterIsoCode = "BRN",
					NumericIsoCode = 96,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Burkina Faso",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BF",
					ThreeLetterIsoCode = "BFA",
					NumericIsoCode = 854,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Burundi",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "BI",
					ThreeLetterIsoCode = "BDI",
					NumericIsoCode = 108,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Cambodia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "KH",
					ThreeLetterIsoCode = "KHM",
					NumericIsoCode = 116,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Cameroon",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "CM",
					ThreeLetterIsoCode = "CMR",
					NumericIsoCode = 120,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Cape Verde",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "CV",
					ThreeLetterIsoCode = "CPV",
					NumericIsoCode = 132,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Central African Republic",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "CF",
					ThreeLetterIsoCode = "CAF",
					NumericIsoCode = 140,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Chad",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "TD",
					ThreeLetterIsoCode = "TCD",
					NumericIsoCode = 148,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Christmas Island",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "CX",
					ThreeLetterIsoCode = "CXR",
					NumericIsoCode = 162,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Cocos (Keeling) Islands",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "CC",
					ThreeLetterIsoCode = "CCK",
					NumericIsoCode = 166,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Comoros",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "KM",
					ThreeLetterIsoCode = "COM",
					NumericIsoCode = 174,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Congo",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "CG",
					ThreeLetterIsoCode = "COG",
					NumericIsoCode = 178,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Cook Islands",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "CK",
					ThreeLetterIsoCode = "COK",
					NumericIsoCode = 184,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Cote D'Ivoire",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "CI",
					ThreeLetterIsoCode = "CIV",
					NumericIsoCode = 384,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Djibouti",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "DJ",
					ThreeLetterIsoCode = "DJI",
					NumericIsoCode = 262,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Dominica",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "DM",
					ThreeLetterIsoCode = "DMA",
					NumericIsoCode = 212,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "El Salvador",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SV",
					ThreeLetterIsoCode = "SLV",
					NumericIsoCode = 222,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Equatorial Guinea",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GQ",
					ThreeLetterIsoCode = "GNQ",
					NumericIsoCode = 226,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Eritrea",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "ER",
					ThreeLetterIsoCode = "ERI",
					NumericIsoCode = 232,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Estonia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "EE",
					ThreeLetterIsoCode = "EST",
					NumericIsoCode = 233,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Ethiopia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "ET",
					ThreeLetterIsoCode = "ETH",
					NumericIsoCode = 231,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Falkland Islands (Malvinas)",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "FK",
					ThreeLetterIsoCode = "FLK",
					NumericIsoCode = 238,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Faroe Islands",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "FO",
					ThreeLetterIsoCode = "FRO",
					NumericIsoCode = 234,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Fiji",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "FJ",
					ThreeLetterIsoCode = "FJI",
					NumericIsoCode = 242,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "French Guiana",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GF",
					ThreeLetterIsoCode = "GUF",
					NumericIsoCode = 254,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "French Polynesia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "PF",
					ThreeLetterIsoCode = "PYF",
					NumericIsoCode = 258,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "French Southern Territories",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "TF",
					ThreeLetterIsoCode = "ATF",
					NumericIsoCode = 260,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Gabon",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GA",
					ThreeLetterIsoCode = "GAB",
					NumericIsoCode = 266,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Gambia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GM",
					ThreeLetterIsoCode = "GMB",
					NumericIsoCode = 270,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Ghana",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GH",
					ThreeLetterIsoCode = "GHA",
					NumericIsoCode = 288,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Greenland",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GL",
					ThreeLetterIsoCode = "GRL",
					NumericIsoCode = 304,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Grenada",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GD",
					ThreeLetterIsoCode = "GRD",
					NumericIsoCode = 308,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Guadeloupe",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GP",
					ThreeLetterIsoCode = "GLP",
					NumericIsoCode = 312,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Guam",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GU",
					ThreeLetterIsoCode = "GUM",
					NumericIsoCode = 316,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Guinea",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GN",
					ThreeLetterIsoCode = "GIN",
					NumericIsoCode = 324,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Guinea-bissau",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GW",
					ThreeLetterIsoCode = "GNB",
					NumericIsoCode = 624,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Guyana",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GY",
					ThreeLetterIsoCode = "GUY",
					NumericIsoCode = 328,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Haiti",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "HT",
					ThreeLetterIsoCode = "HTI",
					NumericIsoCode = 332,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Heard and Mc Donald Islands",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "HM",
					ThreeLetterIsoCode = "HMD",
					NumericIsoCode = 334,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Honduras",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "HN",
					ThreeLetterIsoCode = "HND",
					NumericIsoCode = 340,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Iceland",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "IS",
					ThreeLetterIsoCode = "ISL",
					NumericIsoCode = 352,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Iran (Islamic Republic of)",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "IR",
					ThreeLetterIsoCode = "IRN",
					NumericIsoCode = 364,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Iraq",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "IQ",
					ThreeLetterIsoCode = "IRQ",
					NumericIsoCode = 368,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Kenya",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "KE",
					ThreeLetterIsoCode = "KEN",
					NumericIsoCode = 404,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Kiribati",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "KI",
					ThreeLetterIsoCode = "KIR",
					NumericIsoCode = 296,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Korea",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "KR",
					ThreeLetterIsoCode = "KOR",
					NumericIsoCode = 410,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Kyrgyzstan",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "KG",
					ThreeLetterIsoCode = "KGZ",
					NumericIsoCode = 417,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Lao People's Democratic Republic",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "LA",
					ThreeLetterIsoCode = "LAO",
					NumericIsoCode = 418,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Latvia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "LV",
					ThreeLetterIsoCode = "LVA",
					NumericIsoCode = 428,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Lebanon",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "LB",
					ThreeLetterIsoCode = "LBN",
					NumericIsoCode = 422,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Lesotho",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "LS",
					ThreeLetterIsoCode = "LSO",
					NumericIsoCode = 426,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Liberia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "LR",
					ThreeLetterIsoCode = "LBR",
					NumericIsoCode = 430,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Libyan Arab Jamahiriya",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "LY",
					ThreeLetterIsoCode = "LBY",
					NumericIsoCode = 434,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Liechtenstein",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "LI",
					ThreeLetterIsoCode = "LIE",
					NumericIsoCode = 438,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Lithuania",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "LT",
					ThreeLetterIsoCode = "LTU",
					NumericIsoCode = 440,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Luxembourg",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "LU",
					ThreeLetterIsoCode = "LUX",
					NumericIsoCode = 442,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Macau",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MO",
					ThreeLetterIsoCode = "MAC",
					NumericIsoCode = 446,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Macedonia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MK",
					ThreeLetterIsoCode = "MKD",
					NumericIsoCode = 807,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Madagascar",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MG",
					ThreeLetterIsoCode = "MDG",
					NumericIsoCode = 450,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Malawi",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MW",
					ThreeLetterIsoCode = "MWI",
					NumericIsoCode = 454,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Maldives",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MV",
					ThreeLetterIsoCode = "MDV",
					NumericIsoCode = 462,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Mali",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "ML",
					ThreeLetterIsoCode = "MLI",
					NumericIsoCode = 466,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Malta",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MT",
					ThreeLetterIsoCode = "MLT",
					NumericIsoCode = 470,
					SubjectToVat = true,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Marshall Islands",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MH",
					ThreeLetterIsoCode = "MHL",
					NumericIsoCode = 584,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Martinique",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MQ",
					ThreeLetterIsoCode = "MTQ",
					NumericIsoCode = 474,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Mauritania",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MR",
					ThreeLetterIsoCode = "MRT",
					NumericIsoCode = 478,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Mauritius",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MU",
					ThreeLetterIsoCode = "MUS",
					NumericIsoCode = 480,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Mayotte",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "YT",
					ThreeLetterIsoCode = "MYT",
					NumericIsoCode = 175,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Micronesia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "FM",
					ThreeLetterIsoCode = "FSM",
					NumericIsoCode = 583,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Moldova",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MD",
					ThreeLetterIsoCode = "MDA",
					NumericIsoCode = 498,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Monaco",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MC",
					ThreeLetterIsoCode = "MCO",
					NumericIsoCode = 492,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Mongolia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MN",
					ThreeLetterIsoCode = "MNG",
					NumericIsoCode = 496,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Montenegro",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "ME",
					ThreeLetterIsoCode = "MNE",
					NumericIsoCode = 499,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Montserrat",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MS",
					ThreeLetterIsoCode = "MSR",
					NumericIsoCode = 500,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Morocco",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MA",
					ThreeLetterIsoCode = "MAR",
					NumericIsoCode = 504,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Mozambique",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MZ",
					ThreeLetterIsoCode = "MOZ",
					NumericIsoCode = 508,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Myanmar",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MM",
					ThreeLetterIsoCode = "MMR",
					NumericIsoCode = 104,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Namibia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "NA",
					ThreeLetterIsoCode = "NAM",
					NumericIsoCode = 516,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Nauru",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "NR",
					ThreeLetterIsoCode = "NRU",
					NumericIsoCode = 520,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Nepal",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "NP",
					ThreeLetterIsoCode = "NPL",
					NumericIsoCode = 524,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Netherlands Antilles",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "AN",
					ThreeLetterIsoCode = "ANT",
					NumericIsoCode = 530,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "New Caledonia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "NC",
					ThreeLetterIsoCode = "NCL",
					NumericIsoCode = 540,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Nicaragua",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "NI",
					ThreeLetterIsoCode = "NIC",
					NumericIsoCode = 558,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Niger",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "NE",
					ThreeLetterIsoCode = "NER",
					NumericIsoCode = 562,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Nigeria",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "NG",
					ThreeLetterIsoCode = "NGA",
					NumericIsoCode = 566,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Niue",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "NU",
					ThreeLetterIsoCode = "NIU",
					NumericIsoCode = 570,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Norfolk Island",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "NF",
					ThreeLetterIsoCode = "NFK",
					NumericIsoCode = 574,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Northern Mariana Islands",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "MP",
					ThreeLetterIsoCode = "MNP",
					NumericIsoCode = 580,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Oman",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "OM",
					ThreeLetterIsoCode = "OMN",
					NumericIsoCode = 512,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Palau",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "PW",
					ThreeLetterIsoCode = "PLW",
					NumericIsoCode = 585,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Panama",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "PA",
					ThreeLetterIsoCode = "PAN",
					NumericIsoCode = 591,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Papua New Guinea",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "PG",
					ThreeLetterIsoCode = "PNG",
					NumericIsoCode = 598,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Pitcairn",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "PN",
					ThreeLetterIsoCode = "PCN",
					NumericIsoCode = 612,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Reunion",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "RE",
					ThreeLetterIsoCode = "REU",
					NumericIsoCode = 638,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Rwanda",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "RW",
					ThreeLetterIsoCode = "RWA",
					NumericIsoCode = 646,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Saint Kitts and Nevis",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "KN",
					ThreeLetterIsoCode = "KNA",
					NumericIsoCode = 659,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Saint Lucia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "LC",
					ThreeLetterIsoCode = "LCA",
					NumericIsoCode = 662,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Saint Vincent and the Grenadines",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "VC",
					ThreeLetterIsoCode = "VCT",
					NumericIsoCode = 670,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Samoa",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "WS",
					ThreeLetterIsoCode = "WSM",
					NumericIsoCode = 882,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "San Marino",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SM",
					ThreeLetterIsoCode = "SMR",
					NumericIsoCode = 674,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Sao Tome and Principe",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "ST",
					ThreeLetterIsoCode = "STP",
					NumericIsoCode = 678,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Senegal",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SN",
					ThreeLetterIsoCode = "SEN",
					NumericIsoCode = 686,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Seychelles",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SC",
					ThreeLetterIsoCode = "SYC",
					NumericIsoCode = 690,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Sierra Leone",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SL",
					ThreeLetterIsoCode = "SLE",
					NumericIsoCode = 694,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Solomon Islands",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SB",
					ThreeLetterIsoCode = "SLB",
					NumericIsoCode = 90,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Somalia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SO",
					ThreeLetterIsoCode = "SOM",
					NumericIsoCode = 706,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "South Georgia & South Sandwich Islands",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "GS",
					ThreeLetterIsoCode = "SGS",
					NumericIsoCode = 239,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Sri Lanka",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "LK",
					ThreeLetterIsoCode = "LKA",
					NumericIsoCode = 144,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "St. Helena",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SH",
					ThreeLetterIsoCode = "SHN",
					NumericIsoCode = 654,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "St. Pierre and Miquelon",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "PM",
					ThreeLetterIsoCode = "SPM",
					NumericIsoCode = 666,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Sudan",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SD",
					ThreeLetterIsoCode = "SDN",
					NumericIsoCode = 736,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Suriname",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SR",
					ThreeLetterIsoCode = "SUR",
					NumericIsoCode = 740,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Svalbard and Jan Mayen Islands",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SJ",
					ThreeLetterIsoCode = "SJM",
					NumericIsoCode = 744,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Swaziland",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SZ",
					ThreeLetterIsoCode = "SWZ",
					NumericIsoCode = 748,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Syrian Arab Republic",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "SY",
					ThreeLetterIsoCode = "SYR",
					NumericIsoCode = 760,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Tajikistan",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "TJ",
					ThreeLetterIsoCode = "TJK",
					NumericIsoCode = 762,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Tanzania",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "TZ",
					ThreeLetterIsoCode = "TZA",
					NumericIsoCode = 834,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Togo",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "TG",
					ThreeLetterIsoCode = "TGO",
					NumericIsoCode = 768,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Tokelau",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "TK",
					ThreeLetterIsoCode = "TKL",
					NumericIsoCode = 772,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Tonga",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "TO",
					ThreeLetterIsoCode = "TON",
					NumericIsoCode = 776,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Trinidad and Tobago",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "TT",
					ThreeLetterIsoCode = "TTO",
					NumericIsoCode = 780,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Tunisia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "TN",
					ThreeLetterIsoCode = "TUN",
					NumericIsoCode = 788,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Turkmenistan",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "TM",
					ThreeLetterIsoCode = "TKM",
					NumericIsoCode = 795,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Turks and Caicos Islands",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "TC",
					ThreeLetterIsoCode = "TCA",
					NumericIsoCode = 796,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Tuvalu",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "TV",
					ThreeLetterIsoCode = "TUV",
					NumericIsoCode = 798,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Uganda",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "UG",
					ThreeLetterIsoCode = "UGA",
					NumericIsoCode = 800,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Vanuatu",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "VU",
					ThreeLetterIsoCode = "VUT",
					NumericIsoCode = 548,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Vatican City State (Holy See)",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "VA",
					ThreeLetterIsoCode = "VAT",
					NumericIsoCode = 336,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Viet Nam",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "VN",
					ThreeLetterIsoCode = "VNM",
					NumericIsoCode = 704,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Virgin Islands (British)",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "VG",
					ThreeLetterIsoCode = "VGB",
					NumericIsoCode = 92,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Virgin Islands (U.S.)",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "VI",
					ThreeLetterIsoCode = "VIR",
					NumericIsoCode = 850,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Wallis and Futuna Islands",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "WF",
					ThreeLetterIsoCode = "WLF",
					NumericIsoCode = 876,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Western Sahara",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "EH",
					ThreeLetterIsoCode = "ESH",
					NumericIsoCode = 732,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Yemen",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "YE",
					ThreeLetterIsoCode = "YEM",
					NumericIsoCode = 887,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Zambia",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "ZM",
					ThreeLetterIsoCode = "ZMB",
					NumericIsoCode = 894,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
				new Country
				{
					Name = "Zimbabwe",
					AllowsBilling = true,
					AllowsShipping = true,
					TwoLetterIsoCode = "ZW",
					ThreeLetterIsoCode = "ZWE",
					NumericIsoCode = 716,
					SubjectToVat = false,
					DisplayOrder = 100,
					Published = true
				},
			};
            this.Alter(entities);
            return entities;
        }

        public IList<UserRole> UserRoles()
        {
            var crAdministrators = new UserRole
            {
                Name = "Administrators",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemUserRoleNames.Administrators,
            };
            var crForumModerators = new UserRole
            {
                Name = "Forum Moderators",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemUserRoleNames.ForumModerators,
            };
            var crRegistered = new UserRole
            {
                Name = "Registered",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemUserRoleNames.Registered,
            };
            var crGuests = new UserRole
            {
                Name = "Guests",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemUserRoleNames.Guests,
            };
            var entities = new List<UserRole>
			{
				crAdministrators,
				crForumModerators,
				crRegistered,
				crGuests
			};
            this.Alter(entities);
            return entities;
        }

        public Address AdminAddress()
        {
            var cCountry = _ctx.Set<Country>()
                .Where(x => x.ThreeLetterIsoCode == "USA")
                .FirstOrDefault();

            var entity = new Address()
            {
                FirstName = "疯狂蚂蚁",
                LastName = "疯狂蚂蚁",
                PhoneNumber = "12345678",
                Email = "admin@demo.com",
                FaxNumber = "",
                Company = "疯狂蚂蚁网络工作室",
                Address1 = "1234 Main Road",
                Address2 = "",
                City = "广东广州",
                StateProvince = cCountry.StateProvinces.FirstOrDefault(),
                Country = cCountry,
                ZipPostalCode = "510520",
                CreatedOnUtc = DateTime.UtcNow,
            };
            this.Alter(entity);
            return entity;
        }

        public User SearchEngineUser()
        {
            var entity = new User
            {
                Email = "builtin@search-engine-record.com",
                UserGuid = Guid.NewGuid(),
                PasswordFormat = PasswordFormat.Clear,
                AdminComment = "内置系统客人记录用于来自搜索引擎的请求.",
                Active = true,
                IsSystemAccount = true,
                UserName = SystemUserNames.SearchEngine,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                ModifiedOnUtc = DateTime.UtcNow
            };

            this.Alter(entity);
            return entity;
        }

        public User BackgroundTaskUser()
        {
            var entity = new User
            {
                Email = "builtin@background-task-record.com",
                UserGuid = Guid.NewGuid(),
                PasswordFormat = PasswordFormat.Clear,
                AdminComment = "内置系统记录用于后台任务.",
                Active = true,
                IsSystemAccount = true,
                UserName = SystemUserNames.BackgroundTask,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                ModifiedOnUtc = DateTime.UtcNow
            };

            this.Alter(entity);
            return entity;
        }

        public User PdfConverterUser()
        {
            var entity = new User
            {
                Email = "builtin@pdf-converter-record.com",
                UserGuid = Guid.NewGuid(),
                PasswordFormat = PasswordFormat.Clear,
                AdminComment = "内置系统记录用于PDF转换器.",
                Active = true,
                IsSystemAccount = true,
                UserName = SystemUserNames.PdfConverter,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                ModifiedOnUtc = DateTime.UtcNow
            };

            this.Alter(entity);
            return entity;
        }

        public IList<EmailAccount> EmailAccounts()
        {
            var entities = new List<EmailAccount>()
			{
				new EmailAccount
				{
					Email = "admin@mail.com",
					DisplayName = "管理员",
					Host = "smtp.mail.com",
					Port = 25,
					Username = "123",
					Password = "123",
					EnableSsl = false,
					UseDefaultCredentials = false
				}
			};
            this.Alter(entities);
            return entities;
        }

        public IList<MessageTemplate> MessageTemplates()
        {
            var eaGeneral = this.EmailAccounts().FirstOrDefault(x => x.Email != null);

            string cssString = @"<style type=""text/css"">address, blockquote, center, del, dir, div, dl, fieldset, form, h1, h2, h3, h4, h5, h6, hr, ins, isindex, menu, noframes, noscript, ol, p, pre, table{ margin:0px; } body, td, p{ font-size: 13px;                        font-family: 'Segoe UI', Tahoma, Arial, Helvetica, sans-serif; line-height: 18px; color: #163764; } body{ background:#efefef; } p{ margin-top: 0px; margin-bottom: 10px; } img{ border:0px; } th{ font-weight:bold; color: #ffffff; padding: 5px 0 5px 0; } ul{ list-style-type: square; } li{ line-height: normal; margin-bottom: 5px; } .template-body { width:720px; padding: 10px; border: 1px solid #ccc; } .attr-caption { font-weight: bold; text-align:right; } .attr-value { text-align:right; min-width:158px; width:160px; }</style>";
            string templateHeader = cssString + "<center><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" align=\"center\" bgcolor=\"#ffffff\" class=\"template-body\"><tbody><tr><td>";
            string templateFooter = "</td></tr></tbody></table></center>";

            var entities = new List<MessageTemplate>()
			{
				new MessageTemplate
					{
						Name = "Article.ArticleComment",
						Subject = "%Site.Name%. New blog comment.",
						Body = templateHeader + "<h2><a href=\"%Site.URL%\">%Site.Name%</a></h2><p><br /><br />A new blog comment has been created for blog post \"%ArticleComment.Title%\".</p>" + templateFooter,
						IsActive = true,
						EmailAccountId = eaGeneral.Id,
					},
				 
				new MessageTemplate
					{
						Name = "User.EmailValidationMessage",
						Subject = "%Site.Name%. Email validation",
						Body = templateHeader + "<h2><a href=\"%Site.URL%\">%Site.Name%</a></h2><br /><br />To activate your account <a href=\"%User.AccountActivationURL%\">click here</a>.     <br />  <br />  %Site.Name%" + templateFooter,
						IsActive = true,
						EmailAccountId = eaGeneral.Id,
					},
				new MessageTemplate
					{
						Name = "User.NewPM",
						Subject = "%Site.Name%. You have received a new private message",
						Body = templateHeader + "<h2><a href=\"%Site.URL%\">%Site.Name%</a></h2><p><br /><br />You have received a new private message.</p>" + templateFooter,
						IsActive = true,
						EmailAccountId = eaGeneral.Id,
					},
				new MessageTemplate
					{
						Name = "User.PasswordRecovery",
						Subject = "%Site.Name%. Password recovery",
						Body = templateHeader + "<h2><a href=\"%Site.URL%\">%Site.Name%</a></h2>  <br />  <br />  To change your password <a href=\"%User.PasswordRecoveryURL%\">click here</a>.     <br />  <br />  %Site.Name%" + templateFooter,
						IsActive = true,
						EmailAccountId = eaGeneral.Id,
					},
				new MessageTemplate
					{
						Name = "User.WelcomeMessage",
						Subject = "Welcome to %Site.Name%",
						Body = templateHeader + "We welcome you to <a href=\"%Site.URL%\"> %Site.Name%</a>.<br /><br />You can now take part in the various services we have to offer you. Some of these services include:<br /><br />Permanent Cart - Any products added to your online cart remain there until you remove them, or check them out.<br />Address Book - We can now deliver your products to another address other than yours! This is perfect to send birthday gifts direct to the birthday-person themselves.<br />Order History - View your history of purchases that you have made with us.<br />Products Reviews - Share your opinions on products with our other users.<br /><br />For help with any of our online services, please email the store-owner: <a href=\"mailto:%Site.Email%\">%Site.Email%</a>.<br /><br />Note: This email address was provided on our registration page. If you own the email and did not register on our site, please send an email to <a href=\"mailto:%Site.Email%\">%Site.Email%</a>." + templateFooter,
						IsActive = true,
						EmailAccountId = eaGeneral.Id,
					},
				new MessageTemplate
					{
						Name = "NewUser.Notification",
						Subject = "%Site.Name%. New user registration",
						Body = templateHeader + "<h2><a href=\"%Site.URL%\">%Site.Name%</a></h2><p><br /><br />A new user registered with your store. Below are the user's details:<br /><b>Full name:</b> %User.FullName%<br /><b>Email:</b> %User.Email%</p>"  + templateFooter,
						IsActive = true,
						EmailAccountId = eaGeneral.Id,
					},
				new MessageTemplate
					{
						Name = "NewReturnRequest.SiteOwnerNotification",
						Subject = "%Site.Name%. New return request.",
						Body = templateHeader + "<h2><a href=\"%Site.URL%\">%Site.Name%</a></h2><p><br /><br />%User.FullName% has just submitted a new return request. Details are below:<br /><b>Request ID:</b> %ReturnRequest.ID%<br /><b>Product:</b> %ReturnRequest.Product.Quantity% x <b>Product:</b> %ReturnRequest.Product.Name%<br /><b>Reason for return:</b> %ReturnRequest.Reason%<br /><b>Requested action:</b> %ReturnRequest.RequestedAction%<br /><b>User comments:</b><br />%ReturnRequest.UserComment%</p>"  + templateFooter,
						IsActive = true,
						EmailAccountId = eaGeneral.Id,
					},
				new MessageTemplate
					{
						Name = "News.NewsComment",
						Subject = "%Site.Name%. New news comment.",
						Body = templateHeader + "<h2><a href=\"%Site.URL%\">%Site.Name%</a></h2><p><br /><br />A new news comment has been created for news \"%NewsComment.NewsTitle%\".</p>" + templateFooter,
						IsActive = true,
						EmailAccountId = eaGeneral.Id,
					},
				new MessageTemplate
					{
						Name = "NewsLetterSubscription.ActivationMessage",
						Subject = "%Site.Name%. Subscription activation message.",
						Body = templateHeader + "<p><a href=\"%NewsLetterSubscription.ActivationUrl%\">Click here to confirm your subscription to our list.</a></p><p>If you received this email by mistake, simply delete it.</p>"  + templateFooter,
						IsActive = true,
						EmailAccountId = eaGeneral.Id,
					},
				new MessageTemplate
					{
						Name = "NewsLetterSubscription.DeactivationMessage",
						Subject = "%Site.Name%. Subscription deactivation message.",
						Body = templateHeader + "<p><a href=\"%NewsLetterSubscription.DeactivationUrl%\">Click here to unsubscribe from our newsletter list.</a></p><p>If you received this email by mistake, simply delete it.</p>"  + templateFooter,
						IsActive = true,
						EmailAccountId = eaGeneral.Id,
					},
				new MessageTemplate
					{
						Name = "NewVATSubmitted.SiteOwnerNotification",
						Subject = "%Site.Name%. New VAT number is submitted.",
						Body = templateHeader + "<h2><a href=\"%Site.URL%\">%Site.Name%</a></h2><p><br /><br />%User.FullName% (%User.Email%) has just submitted a new VAT number. Details are below:<br /><b>VAT number:</b> %User.VatNumber%<br /><b>VAT number status:</b> %User.VatNumberStatus%<br /><b>Received name:</b> %VatValidationResult.Name%<br /><b>Received address:</b> %VatValidationResult.Address%</p>"  + templateFooter,
						IsActive = true,
						EmailAccountId = eaGeneral.Id,
					},
				 
				new MessageTemplate
					{
						Name = "Service.EmailAFriend",
						Subject = "%Site.Name%. Referred Item",
						Body = templateHeader + "<h2><a href=\"%Site.URL%\">%Site.Name%</a></h2><p><br /><br />%EmailAFriend.Email% was shopping on %Site.Name% and wanted to share the following item with you. <br /><br /><b><a target=\"_blank\" href=\"%Product.ProductURLForUser%\">%Product.Name%</a></b> <br />%Product.ShortDescription% <br /><br />For more info click <a target=\"_blank\" href=\"%Product.ProductURLForUser%\">here</a> <br /><br /><br />%EmailAFriend.PersonalMessage%<br /><br />%Site.Name%</p>" + templateFooter,
						IsActive = true,
						EmailAccountId = eaGeneral.Id,
					},
				new MessageTemplate
					{
						Name = "Wishlist.EmailAFriend",
						Subject = "%Site.Name%. Wishlist",
						Body = templateHeader + "<h2><a href=\"%Site.URL%\">%Site.Name%</a></h2><p><br /><br />%Wishlist.Email% was shopping on %Site.Name% and wanted to share a wishlist with you. <br /><br /><br />For more info click <a target=\"_blank\" href=\"%Wishlist.URLForUser%\">here</a> <br /><br /><br />%Wishlist.PersonalMessage%<br /><br />%Site.Name%</p>" + templateFooter,
						IsActive = true,
						EmailAccountId = eaGeneral.Id,
					},	 
			};
            this.Alter(entities);
            return entities;
        }

        public IList<ISettings> Settings()
        {
            var seName = GetSeName("slider-bg");
            var imgContentSliderBg = _ctx.Set<Picture>().Where(x => x.SeoFilename == seName).FirstOrDefault();

            var entities = new List<ISettings>
			{
				new PdfSettings
				{
				},
				new CommonSettings
				{
				},
				new SeoSettings()
				{
				},
				new SocialSettings()
				{
				},
				new AdminAreaSettings()
				{
				},
				new ArticleCatalogSettings()
				{
				},
                //new LocalizationSettings()
                //{
                //    DefaultAdminLanguageId = _ctx.Set<Language>().First().Id
                //},
				new UserSettings()
				{
				},
				new AddressSettings()
				{
				},
				new MediaSettings()
				{
				},
				new SiteInformationSettings()
				{
				},
				new CurrencySettings()
				{
				},
				new MeasureSettings()
				{
					BaseDimensionId = _ctx.Set<MeasureDimension>().Where(m => m.SystemKeyword == "inch").Single().Id,
					BaseWeightId = _ctx.Set<MeasureWeight>().Where(m => m.SystemKeyword == "lb").Single().Id,
				},
				new MessageTemplatesSettings()
				{
				},
				new SecuritySettings()
				{
				},
				new PaymentSettings()
				{
					ActivePaymentMethodSystemNames = new List<string>()
					{
						"Payments.CashOnDelivery",
						"Payments.Manual",
						"Payments.PayInStore",
						"Payments.Prepayment"
					}
				},
				new TaxSettings()
				{
				},
	
				new ForumSettings()
				{
				},
                new EmailAccountSettings()
                {
                    DefaultEmailAccountId = _ctx.Set<EmailAccount>().First().Id
                },
                new ContentSliderSettings()
                {
                    BackgroundPictureId = imgContentSliderBg.Id,
                },
				new ThemeSettings()
				{
				}
			};

            this.Alter(entities);
            return entities;
        }

        public IList<ActivityLogType> ActivityLogTypes()
        {
            var entities = new List<ActivityLogType>()
									  {
										  //admin area activities
										  new ActivityLogType
											  {
												  SystemKeyword = "AddNewCategory",
												  Enabled = true,
												  Name = "添加栏目"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "AddNewUser",
												  Enabled = true,
												  Name = "添加新用户"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "AddNewUserRole",
												  Enabled = true,
												  Name = "添加新用户角色"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "AddNewArticle",
												  Enabled = true,
												  Name = "添加新内容"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "AddNewArticleAttribute",
												  Enabled = true,
												  Name = "添加新内容属性"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "AddNewSetting",
												  Enabled = true,
												  Name = "添加新配置"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "AddNewWidget",
												  Enabled = true,
												  Name = "添加新块标识"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "DeleteCategory",
												  Enabled = true,
												  Name = "删除栏目"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "DeleteUser",
												  Enabled = true,
												  Name = "删除用户"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "DeleteUserRole",
												  Enabled = true,
												  Name = "删除用户角色"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "DeleteArticle",
												  Enabled = true,
												  Name = "删除内容"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "DeleteArticleAttribute",
												  Enabled = true,
												  Name = "删除内容属性"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "DeleteSetting",
												  Enabled = true,
												  Name = "删除配置"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "DeleteWidget",
												  Enabled = true,
												  Name = "删除块标识"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "EditCategory",
												  Enabled = true,
												  Name = "编辑栏目"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "EditUser",
												  Enabled = true,
												  Name = "编辑用户"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "EditUserRole",
												  Enabled = true,
												  Name = "编辑用户橘色"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "EditArticle",
												  Enabled = true,
												  Name = "编辑内容"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "EditArticleAttribute",
												  Enabled = true,
												  Name = "编辑内容属性"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "EditSettings",
												  Enabled = true,
												  Name = "编辑配置"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "EditWidget",
												  Enabled = true,
												  Name = "编辑块标识"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "EditThemeVars",
												  Enabled = true,
												  Name = "编辑主题变量"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "ResetThemeVars",
												  Enabled = true,
												  Name = "重置主题变量为初始化变量"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "ImportThemeVars",
												  Enabled = true,
												  Name = "导入主题变量"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "ExportThemeVars",
												  Enabled = true,
												  Name = "导出主题变量"
											  },

										  //public store activities
										  new ActivityLogType
											  {
												  SystemKeyword = "PublicStore.ViewCategory",
												  Enabled = false,
												  Name = "网站. 浏览栏目"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "PublicStore.ViewArticle",
												  Enabled = false,
												  Name = "网站. 浏览内容"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "PublicStore.SendPM",
												  Enabled = false,
												  Name = "网站. 发送信息"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "PublicStore.ContactUs",
												  Enabled = false,
												  Name = "网站. 联系我们"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "PublicStore.Login",
												  Enabled = false,
												  Name = "网站. 登录"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "PublicStore.Logout",
												  Enabled = false,
												  Name = "网站. 注销"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "PublicStore.AddArticleReview",
												  Enabled = false,
												  Name = "网站. 添加内容评论"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "PublicStore.AddForumTopic",
												  Enabled = false,
												  Name = "网站. 添加论坛话题"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "PublicStore.EditForumTopic",
												  Enabled = false,
												  Name = "网站. 编辑论坛话题"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "PublicStore.DeleteForumTopic",
												  Enabled = false,
												  Name = "网站. 删除论坛话题"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "PublicStore.AddForumPost",
												  Enabled = false,
												  Name = "网站. 添加论坛帖子"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "PublicStore.EditForumPost",
												  Enabled = false,
												  Name = "网站. 编辑论坛帖子"
											  },
										  new ActivityLogType
											  {
												  SystemKeyword = "PublicStore.DeleteForumPost",
												  Enabled = false,
												  Name = "网站. 删除论坛帖子"
											  },
									  };

            this.Alter(entities);
            return entities;
        }

        public IList<ScheduleTask> ScheduleTasks()
        {
            var entities = new List<ScheduleTask>
			{
				new ScheduleTask
				{
					Name = "Send emails",
					CronExpression = "* * * * *", // every Minute
					Type = "CAF.WebSite.Application.Services.Messages.QueuedMessagesSendTask, CAF.WebSite.Application.Services",
					Enabled = true,
					StopOnError = false,
				},
				new ScheduleTask
				{
					Name = "Delete guests",
					CronExpression = "*/10 * * * *", // Every 10 minutes
					Type = "CAF.WebSite.Application.Services.Users.DeleteGuestsTask, CAF.WebSite.Application.Services",
					Enabled = true,
					StopOnError = false,
				},
				new ScheduleTask
				{
					Name = "Delete logs",
					CronExpression = "0 1 * * *", // At 01:00
					Type = "CAF.WebSite.Application.Services.Logging.DeleteLogsTask, CAF.WebSite.Application.Services",
					Enabled = true,
					StopOnError = false,
				},
				new ScheduleTask
				{
					Name = "Clear cache",
					CronExpression = "0 */4 * * *", // Every 04 hours
					Type = "CAF.WebSite.Application.Services.Caching.ClearCacheTask, CAF.WebSite.Application.Services",
					Enabled = false,
					StopOnError = false,
				},
				new ScheduleTask
				{
					Name = "Clear transient uploads",
					CronExpression = "30 1,13 * * *", // At 01:30 and 13:30
					Type = "CAF.WebSite.Application.Services.Media.TransientMediaClearTask, CAF.WebSite.Application.Services",
					Enabled = true,
					StopOnError = false,
				},
				new ScheduleTask
				{
					Name = "Clear email queue",
					CronExpression = "0 2 * * *", // At 02:00
					Type = "CAF.WebSite.Application.Services.Messages.QueuedMessagesClearTask, CAF.WebSite.Application.Services",
					Enabled = true,
					StopOnError = false,
				},
				new ScheduleTask
				{
					Name = "Cleanup temporary files",
					CronExpression = "30 3 * * *", // At 03:30
					Type = "CAF.WebSite.Application.Services.Common.TempFileCleanupTask, CAF.WebSite.Application.Services",
					Enabled = true,
					StopOnError = false
				}
			};
            this.Alter(entities);
            return entities;
        }

        #endregion

        #region Sample data creators


        public IList<ModelTemplate> ModelTemplates()
        {
            var entities = new List<ModelTemplate>
							   {
									new ModelTemplate
									{
										Name = "明细页",
										ViewPath = "ArticleTemplate.Simple",
										DisplayOrder = 10,
                                        TemplageTypeId=(int)TemplateTypeFormat.Detail
									},
                                    new ModelTemplate
									{
										Name = "列表页-网格",
										ViewPath = "CategoryTemplate.ArticlesInGrid",
										DisplayOrder = 10,
                                        TemplageTypeId=(int)TemplateTypeFormat.List
									},
                                     new ModelTemplate
									{
										Name = "列表页-列表",
										ViewPath = "CategoryTemplate.ArticlesInLines",
										DisplayOrder = 10,
                                        TemplageTypeId=(int)TemplateTypeFormat.List
									},
                                      new ModelTemplate
									{
										Name = "单页-企业信息页面",
										ViewPath = "TopicCompanyView",
										DisplayOrder = 10,
                                        TemplageTypeId=(int)TemplateTypeFormat.SinglePage
									},
                                      new ModelTemplate
									{
										Name = "单页-服务信息页面",
										ViewPath = "TopicServiceView",
										DisplayOrder = 11,
                                        TemplageTypeId=(int)TemplateTypeFormat.SinglePage
									}
							   };
            this.Alter(entities);
            return entities;
        }


        public IList<Topic> Topics()
        {
            var topicTemplate =
            this._ctx.Set<ModelTemplate>().Where(pt => pt.ViewPath == "TopicCompanyView").FirstOrDefault();
            var topicServiceTemplate =
            this._ctx.Set<ModelTemplate>().Where(pt => pt.ViewPath == "TopicServiceView").FirstOrDefault();
            var entities = new List<Topic>()
			{
				new Topic
					{
						SystemName = "AboutUs",
						IncludeInSitemap = false,
						IsPasswordProtected = false,
						Title = "关于我们",
						Body = "<p>你可以在管理网站这里编辑“关于我们信息“。</p>",
                        CreatedOnUtc = DateTime.UtcNow,
                        ModifiedOnUtc = DateTime.UtcNow,
                        TopicTemplateId=topicTemplate.Id
					},
				new Topic
					{
						SystemName = "ContactUs",
						IncludeInSitemap = false,
						IsPasswordProtected = false,
						Title = "联系我们",
						Body = "<p>你可以在管理网站这里编辑“联系我们信息“。</p>",
                        CreatedOnUtc = DateTime.UtcNow,
                        ModifiedOnUtc = DateTime.UtcNow,
                        TopicTemplateId=topicTemplate.Id
					},
                    new Topic
					{
						SystemName = "Information",
						IncludeInSitemap = false,
						IsPasswordProtected = false,
						Title = "信息",
						Body = "<p>你可以在管理网站这里编辑“我们的信息“。</p>",
                        CreatedOnUtc = DateTime.UtcNow,
                        ModifiedOnUtc = DateTime.UtcNow,
                        TopicTemplateId=topicTemplate.Id
					},
				  new Topic
					{
						SystemName = "SalesService",
						IncludeInSitemap = false,
						IsPasswordProtected = false,
						Title = "售后服务",
						Body = "<p>你可以在管理网站这里编辑“售后服务“。</p>",
                        CreatedOnUtc = DateTime.UtcNow,
                        ModifiedOnUtc = DateTime.UtcNow,
                        TopicTemplateId=topicServiceTemplate.Id
					},
				 
			};
            this.Alter(entities);
            return entities;
        }

        public IList<Channel> Channels()
        {
            var channelTemplateInGridAndLines =
               this._ctx.Set<ModelTemplate>().Where(pt => pt.ViewPath == "CategoryTemplate.ArticlesInGrid").FirstOrDefault();
            var channelDetailTemplateInGridAndLines =
               this._ctx.Set<ModelTemplate>().Where(pt => pt.ViewPath == "ArticleTemplate.Simple").FirstOrDefault();
            var entities = new List<Channel>
							   {
									new Channel
									{
										Name = "Product",
										Title = "产品",
										DisplayOrder = 10,
                                        CreatedOnUtc = DateTime.UtcNow,
                                        ModifiedOnUtc = DateTime.UtcNow,
                                        ModelTemplateId = channelTemplateInGridAndLines.Id,
                                        DetailModelTemplateId = channelDetailTemplateInGridAndLines.Id
									},
                                  new Channel
									{
										Name = "New",
										Title = "内容",
										DisplayOrder = 10,
                                        CreatedOnUtc = DateTime.UtcNow,
                                        ModifiedOnUtc = DateTime.UtcNow,
                                        ModelTemplateId = channelTemplateInGridAndLines.Id,
                                        DetailModelTemplateId = channelDetailTemplateInGridAndLines.Id
									},
							   };
            this.Alter(entities);
            return entities;
        }

        public IList<ArticleCategory> CategoriesFirstLevel()
        {
            var channel = _ctx.Set<Channel>()
               .FirstOrDefault();
            // pictures
            var sampleImagesPath = this._sampleImagesPath;
            var categoryTemplateInGridAndLines =
               this._ctx.Set<ModelTemplate>().Where(pt => pt.ViewPath == "CategoryTemplate.ArticlesInGrid").FirstOrDefault();
            var categoryDetailTemplateInGridAndLines =
               this._ctx.Set<ModelTemplate>().Where(pt => pt.ViewPath == "ArticleTemplate.Simple").FirstOrDefault();
            //categories

            #region category definitions

            var categoryProducts = new ArticleCategory
            {
                Name = "产品",
                ModelTemplateId = categoryTemplateInGridAndLines.Id,
                DetailModelTemplateId = categoryDetailTemplateInGridAndLines.Id,
                ParentCategoryId = 0,
                Channel = channel,
                PageSize = 12,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "12,18,36,72,150",
                Published = true,
                Deleted = false,
                ShowOnHomePage = true,
                SubjectToAcl = false,
                LimitedToSites = false,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedOnUtc = DateTime.UtcNow,
                MetaTitle = "产品"
            };

            var categoryNews = new ArticleCategory
            {
                Name = "新闻资讯",
                ModelTemplateId = categoryTemplateInGridAndLines.Id,
                DetailModelTemplateId = categoryDetailTemplateInGridAndLines.Id,
                ParentCategoryId = 0,
                Channel = channel,
                PageSize = 12,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "12,18,36,72,150",
                Published = true,
                DisplayOrder = 2,
                ShowOnHomePage = true,
                SubjectToAcl = false,
                LimitedToSites = false,
                Deleted = false,
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedOnUtc = DateTime.UtcNow,
                MetaTitle = "新闻资讯"
            };



            #endregion category definitions

            var entities = new List<ArticleCategory>
			{
			   categoryProducts, categoryNews
			};

            this.Alter(entities);
            return entities;
        }

        public IList<ArticleCategory> CategoriesSecondLevel()
        {
            var channel = _ctx.Set<Channel>()
            .FirstOrDefault();
            var sampleImagesPath = this._sampleImagesPath;
            var categoryTemplateInGridAndLines =
               this._ctx.Set<ModelTemplate>().Where(pt => pt.ViewPath == "CategoryTemplate.ArticlesInLines").FirstOrDefault();
            var categoryDetailTemplateInGridAndLines =
               this._ctx.Set<ModelTemplate>().Where(pt => pt.ViewPath == "ArticleTemplate.Simple").FirstOrDefault();
            //categories

            #region category definitions

            var categoryBooksSpiegel = new ArticleCategory
            {
                Name = "SPIEGEL-Bestseller",
                ModelTemplateId = categoryTemplateInGridAndLines.Id,
                DetailModelTemplateId = categoryDetailTemplateInGridAndLines.Id,
                Picture = CreatePicture(File.ReadAllBytes(sampleImagesPath + "0000930_spiegel-bestseller.png"), "image/png", GetSeName("SPIEGEL-Bestseller")),
                Published = true,
                ParentCategoryId = _ctx.Set<ArticleCategory>().Where(x => x.MetaTitle == "产品").First().Id,
                Channel = channel,
                PageSize = 12,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "12,18,36,72,150",
                Deleted = false,
                ShowOnHomePage = true,
                SubjectToAcl = false,
                LimitedToSites = false,
                DisplayOrder = 1,
                MetaTitle = "SPIEGEL-Bestseller",
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedOnUtc = DateTime.UtcNow,
            };

            var categoryBooksCookAndEnjoy = new ArticleCategory
            {
                Name = "Cook and enjoy",
                ModelTemplateId = categoryTemplateInGridAndLines.Id,
                DetailModelTemplateId = categoryDetailTemplateInGridAndLines.Id,
                PageSize = 12,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "12,18,36,72,150",
                Picture = CreatePicture(File.ReadAllBytes(sampleImagesPath + "0000936_kochen-geniesen.jpeg"), "image/jpeg", GetSeName("Cook and enjoy")),
                Published = true,
                ParentCategoryId = _ctx.Set<ArticleCategory>().Where(x => x.MetaTitle == "新闻资讯").First().Id,
                Channel = channel,
                Deleted = false,
                ShowOnHomePage = true,
                SubjectToAcl = false,
                LimitedToSites = false,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedOnUtc = DateTime.UtcNow,
                MetaTitle = "Cook and enjoy"
            };

            var categoryDesktops = new ArticleCategory
            {
                Name = "Desktops",
                ModelTemplateId = categoryTemplateInGridAndLines.Id,
                DetailModelTemplateId = categoryDetailTemplateInGridAndLines.Id,
                PageSize = 12,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "12,18,36,72,150",
                ParentCategoryId = _ctx.Set<ArticleCategory>().Where(x => x.MetaTitle == "产品").First().Id,
                Picture = CreatePicture(File.ReadAllBytes(sampleImagesPath + "category_desktops.png"), "image/png", GetSeName("Desktops")),
                Channel = channel,
                Published = true,
                Deleted = false,
                ShowOnHomePage = true,
                SubjectToAcl = false,
                LimitedToSites = false,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedOnUtc = DateTime.UtcNow,
                MetaTitle = "Desktops"
            };

            var categoryNotebooks = new ArticleCategory
            {
                Name = "Notebooks",
                ModelTemplateId = categoryTemplateInGridAndLines.Id,
                DetailModelTemplateId = categoryDetailTemplateInGridAndLines.Id,
                PageSize = 12,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "12,18,36,72,150",
                ParentCategoryId = _ctx.Set<ArticleCategory>().Where(x => x.MetaTitle == "产品").First().Id,
                Picture = CreatePicture(File.ReadAllBytes(sampleImagesPath + "category_notebooks.png"), "image/png", GetSeName("Notebooks")),
                Channel = channel,
                Published = true,
                Deleted = false,
                ShowOnHomePage = true,
                SubjectToAcl = false,
                LimitedToSites = false,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedOnUtc = DateTime.UtcNow,
                MetaTitle = "Notebooks"
            };

            var categoryGamingAccessories = new ArticleCategory
            {
                Name = "Gaming Accessories",
                ModelTemplateId = categoryTemplateInGridAndLines.Id,
                DetailModelTemplateId = categoryDetailTemplateInGridAndLines.Id,
                PageSize = 12,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "12,18,36,72,150",
                ParentCategoryId = _ctx.Set<ArticleCategory>().Where(x => x.MetaTitle == "产品").First().Id,
                Picture = CreatePicture(File.ReadAllBytes(sampleImagesPath + "category_gaming_accessories.png"), "image/png", GetSeName("Gaming Accessories")),
                Channel = channel,
                Published = true,
                Deleted = false,
                ShowOnHomePage = true,
                SubjectToAcl = false,
                LimitedToSites = false,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedOnUtc = DateTime.UtcNow,
                MetaTitle = "Gaming Accessories"
            };

            var categoryGamingGames = new ArticleCategory
            {
                Name = "Games",
                ModelTemplateId = categoryTemplateInGridAndLines.Id,
                DetailModelTemplateId = categoryDetailTemplateInGridAndLines.Id,
                PageSize = 12,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "12,18,36,72,150",
                ParentCategoryId = _ctx.Set<ArticleCategory>().Where(x => x.MetaTitle == "新闻资讯").First().Id,
                Picture = CreatePicture(File.ReadAllBytes(sampleImagesPath + "category_games.png"), "image/png", GetSeName("Games")),
                Channel = channel,
                Published = true,
                Deleted = false,
                ShowOnHomePage = true,
                SubjectToAcl = false,
                LimitedToSites = false,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedOnUtc = DateTime.UtcNow,
                MetaTitle = "Games"
            };

            #endregion category definitions

            var entities = new List<ArticleCategory>
			{
				categoryBooksSpiegel, categoryBooksCookAndEnjoy, categoryDesktops, categoryNotebooks, categoryGamingAccessories, categoryGamingGames
			};

            this.Alter(entities);
            return entities;
        }

        public IList<Article> Articles()
        {
            var sampleImagesPath = this._sampleImagesPath;
            var articleDefaultCategory = _ctx.Set<ArticleCategory>()
                .FirstOrDefault();
            var categoryProduct = this._ctx.Set<ArticleCategory>().First(c => c.MetaTitle == "产品");
            var categoryNew = this._ctx.Set<ArticleCategory>().First(c => c.MetaTitle == "新闻资讯");
            var categoryDetailTemplateInGridAndLines =
               this._ctx.Set<ModelTemplate>().Where(pt => pt.ViewPath == "ArticleTemplate.Simple").FirstOrDefault();
            var articleDemo = new Article()
            {
                Title = "文章1",
                ShortContent = "简介",
                FullContent = "<p>详细内容</p>",
                ArticleCategory = articleDefaultCategory,
                ModelTemplateId = categoryDetailTemplateInGridAndLines.Id,
                AllowUserReviews = true,
                StatusId = (int)StatusFormat.Norma,
                SubjectToAcl = false,
                LimitedToSites = false,
                Deleted = false,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedOnUtc = DateTime.UtcNow,
                MetaTitle = "文章",
            };
            var articleBooksUberMan = new Article()
            {
                Title = "文章1",
                ShortContent = "简介",
                FullContent = "<p>详细内容</p>",
                ArticleCategory = categoryNew,
                ModelTemplateId = categoryDetailTemplateInGridAndLines.Id,
                AllowUserReviews = true,
                StatusId = (int)StatusFormat.Norma,
                SubjectToAcl = false,
                LimitedToSites = false,
                Deleted = false,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedOnUtc = DateTime.UtcNow,
                MetaTitle = "文章",
                Picture = CreatePicture(File.ReadAllBytes(sampleImagesPath + "0000932_uberman-der-roman.jpeg"), "image/jpeg", GetSeName("product")),
            };
            //pictures
            articleBooksUberMan.ArticleAlbum.Add(new ArticleAlbum()
            {
                Picture = CreatePicture(File.ReadAllBytes(sampleImagesPath + "0000932_uberman-der-roman.jpeg"), "image/jpeg", GetSeName("product")),
                DisplayOrder = 1,
            });

            var entities = new List<Article>
			{
				articleDemo,articleBooksUberMan
			};

            this.Alter(entities);
            return entities;
        }


        #region ForumGroups
        public IList<ForumGroup> ForumGroups()
        {
            var forumGroupGeneral = new ForumGroup()
            {
                Name = "General",
                Description = "",
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };

            var entities = new List<ForumGroup>
			{
				forumGroupGeneral
			};

            this.Alter(entities);
            return entities;
        }
        #endregion ForumGroups

        #region Forums
        public IList<Forum> Forums()
        {
            var newProductsForum = new Forum()
            {
                ForumGroup = _ctx.Set<ForumGroup>().Where(c => c.DisplayOrder == 1).Single(),
                Name = "New Products",
                Description = "Discuss new products and industry trends",
                NumTopics = 0,
                NumPosts = 0,
                LastPostUserId = 0,
                LastPostTime = null,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };

            var packagingShippingForum = new Forum()
            {
                ForumGroup = _ctx.Set<ForumGroup>().Where(c => c.DisplayOrder == 1).Single(),
                Name = "Packaging & Shipping",
                Description = "Discuss packaging & shipping",
                NumTopics = 0,
                NumPosts = 0,
                LastPostTime = null,
                DisplayOrder = 20,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };


            var entities = new List<Forum>
			{
				newProductsForum, packagingShippingForum
			};

            this.Alter(entities);
            return entities;
        }
        #endregion Forums

        #region Deliverytimes
        public IList<DeliveryTime> DeliveryTimes()
        {
            var entities = new List<DeliveryTime>()
			{
				new DeliveryTime
					{
						Name = "available and ready to ship",
						DisplayOrder = 0,
						ColorHexValue = "#008000"
					},
				new DeliveryTime
					{
						Name = "2-5 woking days",
						DisplayOrder = 1,
						ColorHexValue = "#FFFF00"
					},
				new DeliveryTime
					{
						Name = "7 working days",
						DisplayOrder = 2,
						ColorHexValue = "#FF9900"
					},
			};
            this.Alter(entities);
            return entities;
        }

        #endregion Deliverytimes

        #region QuantityUnits

        public IList<QuantityUnit> QuantityUnits()
        {
            var entities = new List<QuantityUnit>()
			{
				new QuantityUnit
					{
						Name = "Piece",        
                        Description = "Piece",
                        IsDefault = true,
						DisplayOrder = 0,
					},
				new QuantityUnit
					{
						Name = "Box",           
                        Description = "Box",
						DisplayOrder = 1,
					},
				new QuantityUnit
					{
						Name = "Parcel",        
                        Description = "Parcel",
						DisplayOrder = 2,
					},
                new QuantityUnit
					{
						Name = "Palette",       
                        Description = "Palette",
						DisplayOrder = 3,
					},
			};
            this.Alter(entities);
            return entities;
        }

        #endregion

        #region PollAnswer
        public IList<PollAnswer> PollAnswers()
        {
            var pollAnswer1 = new PollAnswer()
            {
                Name = "Excellent",
                DisplayOrder = 1,
            };
            var pollAnswer2 = new PollAnswer()
            {
                Name = "Good",
                DisplayOrder = 2,
            };
            var pollAnswer3 = new PollAnswer()
            {
                Name = "Poor",
                DisplayOrder = 3,
            };
            var pollAnswer4 = new PollAnswer()
            {
                Name = "Very bad",
                DisplayOrder = 4,
            };
            var pollAnswer5 = new PollAnswer()
            {
                Name = "Daily",
                DisplayOrder = 5,
            };
            var pollAnswer6 = new PollAnswer()
            {
                Name = "Once a week",
                DisplayOrder = 6,
            };
            var pollAnswer7 = new PollAnswer()
            {
                Name = "Every two weeks",
                DisplayOrder = 7,
            };
            var pollAnswer8 = new PollAnswer()
            {
                Name = "Once a month",
                DisplayOrder = 8,
            };

            var entities = new List<PollAnswer>
			{
				pollAnswer1, pollAnswer2, pollAnswer3, pollAnswer4, pollAnswer5,  pollAnswer6,  pollAnswer7,  pollAnswer8
			};

            this.Alter(entities);
            return entities;
        }
        #endregion PollAnswer

        #region Polls
        public IList<Poll> Polls()
        {
            var defaultLanguage = _ctx.Set<Language>().FirstOrDefault();
            var poll1 = new Poll()
            {
                Language = defaultLanguage,
                Name = "How do you like the shop?",
                SystemKeyword = "RightColumnPoll",
                Published = true,
                DisplayOrder = 1,
            };

            poll1.PollAnswers.Add(new PollAnswer()
            {
                Name = "Excellent",
                DisplayOrder = 1,
            });

            poll1.PollAnswers.Add(new PollAnswer()
            {
                Name = "Good",
                DisplayOrder = 2,
            });

            poll1.PollAnswers.Add(new PollAnswer()
            {
                Name = "Poor",
                DisplayOrder = 3,
            });

            poll1.PollAnswers.Add(new PollAnswer()
            {
                Name = "Very bad",
                DisplayOrder = 4,
            });


            //_pollAnswerRepository.Table.Where(x => x.DisplayOrder < 5).Each(y =>
            //    {
            //        poll1.PollAnswers.Add(y);
            //    });

            var poll2 = new Poll()
            {
                Language = defaultLanguage,
                Name = "How often do you buy online?",
                SystemKeyword = "RightColumnPoll",
                Published = true,
                DisplayOrder = 2,
            };

            poll2.PollAnswers.Add(new PollAnswer()
            {
                Name = "Daily",
                DisplayOrder = 1,
            });

            poll2.PollAnswers.Add(new PollAnswer()
            {
                Name = "Once a week",
                DisplayOrder = 2,
            });

            poll2.PollAnswers.Add(new PollAnswer()
            {
                Name = "Every two weeks",
                DisplayOrder = 3,
            });

            poll2.PollAnswers.Add(new PollAnswer()
            {
                Name = "Once a month",
                DisplayOrder = 4,
            });

            //_pollAnswerRepository.Table.Where(x => x.DisplayOrder > 4).Each(y =>
            //{
            //    poll2.PollAnswers.Add(y);
            //});


            var entities = new List<Poll>
			{
				poll1, poll2
			};

            this.Alter(entities);
            return entities;
        }
        #endregion Polls

        #region Alterations

        protected virtual void Alter(IList<MeasureDimension> entities)
        {
        }

        protected virtual void Alter(IList<MeasureWeight> entities)
        {
        }

        protected virtual void Alter(IList<TaxCategory> entities)
        {
        }


        protected virtual void Alter(IList<Country> entities)
        {
        }

        protected virtual void Alter(IList<UserRole> entities)
        {
        }

        protected virtual void Alter(Address entity)
        {
        }

        protected virtual void Alter(User entity)
        {
        }

        protected virtual void Alter(IList<DeliveryTime> entities)
        {
        }

        protected virtual void Alter(IList<QuantityUnit> entities)
        {
        }

        protected virtual void Alter(IList<EmailAccount> entities)
        {
        }

        protected virtual void Alter(IList<MessageTemplate> entities)
        {
        }

        protected virtual void Alter(IList<Topic> entities)
        {
        }

        protected virtual void Alter(IList<Site> entities)
        {
        }

        protected virtual void Alter(IList<Picture> entities)
        {
        }

        protected virtual void Alter(IList<ISettings> settings)
        {
        }

        protected virtual void Alter(IList<SiteInformationSettings> settings)
        {
        }


        protected virtual void Alter(IList<MeasureSettings> settings)
        {
        }

        protected virtual void Alter(IList<PaymentSettings> settings)
        {
        }

        protected virtual void Alter(IList<TaxSettings> settings)
        {
        }

        protected virtual void Alter(IList<EmailAccountSettings> settings)
        {
        }

        protected virtual void Alter(IList<ActivityLogType> entities)
        {
        }

        protected virtual void Alter(IList<ModelTemplate> entities)
        {
        }


        protected virtual void Alter(IList<ScheduleTask> entities)
        {
        }


        protected virtual void Alter(IList<ArticleAttribute> entities)
        {
        }


        protected virtual void Alter(IList<Channel> entities)
        {
        }
        protected virtual void Alter(IList<ArticleCategory> entities)
        {
        }



        protected virtual void Alter(IList<Article> entities)
        {
        }

        protected virtual void Alter(IList<ForumGroup> entities)
        {
        }

        protected virtual void Alter(IList<Forum> entities)
        {
        }



        protected virtual void Alter(IList<Poll> entities)
        {
        }

        protected virtual void Alter(IList<PollAnswer> entities)
        {
        }

        protected virtual void Alter(IList<ArticleTag> entities)
        {
        }



        #endregion Alterations

        #endregion Sample data creators

        #region Helpers

        protected DefaultObjectContext DbContext
        {
            get
            {
                return _ctx;
            }
        }

        protected string SampleImagesPath
        {
            get
            {
                return this._sampleImagesPath;
            }
        }

        protected string SampleDownloadsPath
        {
            get
            {
                return this._sampleDownloadsPath;
            }
        }

        protected Picture CreatePicture(byte[] pictureBinary, string mimeType, string seoFilename)
        {
            mimeType = mimeType.EmptyNull();
            mimeType = mimeType.Truncate(20);

            seoFilename = seoFilename.Truncate(100);

            var picture = _ctx.Set<Picture>().Create();
            picture.PictureBinary = pictureBinary;
            picture.MimeType = mimeType;
            picture.SeoFilename = seoFilename;
            picture.UpdatedOnUtc = DateTime.Now;
            return picture;
        }

        protected string GetSeName(string name)
        {
            return SeoHelper.GetSeName(name, true, false);
        }

        //protected Currency CreateCurrency(string locale, decimal rate = 1M, string formatting = "", bool published = false, int order = 1)
        //{
        //    RegionInfo info = null;
        //    Currency currency = null;
        //    try
        //    {
        //        info = new RegionInfo(locale);
        //        if (info != null)
        //        {
        //            currency = new Currency();
        //            currency.DisplayLocale = locale;
        //            currency.Name = info.CurrencyNativeName;
        //            currency.CurrencyCode = info.ISOCurrencySymbol;
        //            currency.Rate = rate;
        //            currency.CustomFormatting = formatting;
        //            currency.Published = published;
        //            currency.DisplayOrder = order;
        //            currency.CreatedOnUtc = DateTime.UtcNow;
        //            currency.ModifiedOnUtc = DateTime.UtcNow;
        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }

        //    return currency;
        //}

        #endregion
    }
}