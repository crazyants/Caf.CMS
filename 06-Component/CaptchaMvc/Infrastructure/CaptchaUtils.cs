using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Provides methods to work with a captcha.
    /// </summary>
    public static class CaptchaUtils
    {
        #region Fields

        private static readonly object Locker = new object();
        private static volatile IImageGenerator _defaultImageGenerator;
        private static volatile ICaptchaBuilderProvider _defaultBuilderProvider;
        private static volatile ICaptchaManager _defaultCaptchaManager;
        private static Func<IParameterContainer, ICaptchaBuilderProvider> _builderProviderFactory;
        private static Func<IParameterContainer, ICaptchaManager> _captchaManagerFactory;
        private static Func<IDrawingModel, IImageGenerator> _imageGeneratorFactory;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="CaptchaUtils" /> class.
        /// </summary>
        static CaptchaUtils()
        {
            BuilderProviderFactory = container => BuilderProvider;
            CaptchaManagerFactory = container => CaptchaManager;
            ImageGeneratorFactory = model => ImageGenerator;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the current <see cref="ICaptchaBuilderProvider" />.
        /// </summary>
        public static ICaptchaBuilderProvider BuilderProvider
        {
            get
            {
                if (_defaultBuilderProvider == null)
                {
                    lock (Locker)
                    {
                        if (_defaultBuilderProvider == null)
                        {
                            _defaultBuilderProvider =
                                GetService<ICaptchaBuilderProvider>("DefaultCaptchaBuilderProvider",
                                                                    () => new DefaultCaptchaBuilderProvider());
                        }
                    }
                }
                return _defaultBuilderProvider;
            }
            set
            {
                lock (Locker)
                {
                    Validate.PropertyNotNull(value, "BuilderProvider");
                    _defaultBuilderProvider = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the current <see cref="ICaptchaManager" />.
        /// </summary>
        public static ICaptchaManager CaptchaManager
        {
            get
            {
                if (_defaultCaptchaManager == null)
                {
                    lock (Locker)
                    {
                        if (_defaultCaptchaManager == null)
                        {
                            _defaultCaptchaManager = GetService<ICaptchaManager>("DefaultCaptchaManager",
                                                                                 () =>
                                                                                 new DefaultCaptchaManager(
                                                                                     new SessionStorageProvider()));
                        }
                    }
                }

                return _defaultCaptchaManager;
            }
            set
            {
                lock (Locker)
                {
                    Validate.PropertyNotNull(value, "CaptchaManager");
                    _defaultCaptchaManager = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the current <see cref="IImageGenerator" />.
        /// </summary>
        public static IImageGenerator ImageGenerator
        {
            get
            {
                if (_defaultImageGenerator == null)
                {
                    lock (Locker)
                    {
                        if (_defaultImageGenerator == null)
                        {
                            _defaultImageGenerator = GetService<IImageGenerator>("CaptchaIGenerate",
                                                                                 () => new DefaultImageGenerator());
                        }
                    }
                }
                return _defaultImageGenerator;
            }
            set
            {
                lock (Locker)
                {
                    Validate.PropertyNotNull(value, "ImageGeneratorGenerator");
                    _defaultImageGenerator = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the factory to create <see cref="ICaptchaBuilderProvider" />.
        /// </summary>
        public static Func<IParameterContainer, ICaptchaBuilderProvider> BuilderProviderFactory
        {
            get { return _builderProviderFactory; }
            set
            {
                Validate.PropertyNotNull(value, "BuilderProviderFactory");
                _builderProviderFactory = value;
            }
        }

        /// <summary>
        ///     Gets or sets the factory to create <see cref="ICaptchaManager" />.
        /// </summary>
        public static Func<IParameterContainer, ICaptchaManager> CaptchaManagerFactory
        {
            get { return _captchaManagerFactory; }
            set
            {
                Validate.PropertyNotNull(value, "CaptchaManagerFactory");
                _captchaManagerFactory = value;
            }
        }

        /// <summary>
        ///     Gets or sets the factory to create <see cref="IImageGenerator" />.
        /// </summary>
        public static Func<IDrawingModel, IImageGenerator> ImageGeneratorFactory
        {
            get { return _imageGeneratorFactory; }
            set
            {
                Validate.PropertyNotNull(value, "ImageGeneratorFactory");
                _imageGeneratorFactory = value;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Creates a new captcha with the specified arguments.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The specified <see cref="HtmlHelper" />.
        /// </param>
        /// <param name="parameters">The specified attributes.</param>
        /// <returns>
        ///     An instance of <see cref="ICaptcha" />.
        /// </returns>
        public static ICaptcha GenerateCaptcha(HtmlHelper htmlHelper, IList<ParameterModel> parameters)
        {
            var parameterContainer = new ParameterModelContainer(parameters);
            IBuildInfoModel buildInfoModel = CaptchaManagerFactory(parameterContainer)
                .GenerateNew(htmlHelper, parameterContainer);
            return BuilderProviderFactory(parameterContainer).GenerateCaptcha(buildInfoModel);
        }

        /// <summary>
        ///     Determines whether the captcha is valid, and write error message if need.
        /// </summary>
        /// <param name="controller">
        ///     The specified <see cref="ControllerBase" />.
        /// </param>
        /// <param name="attributes">The specified attributes.</param>
        /// <returns>
        ///     <c>True</c> if the captcha is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool ValidateCaptcha(ControllerBase controller, IList<ParameterModel> attributes)
        {
            var container = new CombinedParameterContainer(
                new RequestParameterContainer(controller.ControllerContext.HttpContext.Request),
                new ParameterModelContainer(attributes));
            return CaptchaManagerFactory(container).ValidateCaptcha(controller, container);
        }

        #endregion

        #region Internal methods

        internal static T GetFromSession<T>(string key, Func<T> getItem) where T : class
        {
            var item = HttpContext.Current.Session[key] as T;
            if (item == null)
            {
                item = getItem();
                HttpContext.Current.Session[key] = item;
            }
            return item;
        }

        internal static void ClearIfNeed<TKey, TValue>(this IDictionary<KeyTimeEntry<TKey>, TValue> dictionary, uint maxCount)
        {
            if (dictionary.Count < maxCount) return;
            var list = new List<KeyTimeEntry<TKey>>(5);
            list.AddRange(dictionary.Keys.OrderBy(entry => entry.Timestamp).Take(5));
            foreach (var source in list)
            {
                dictionary.Remove(source);
            }
        }

        internal static void ClearIfNeed<TKey>(this HashSet<KeyTimeEntry<TKey>> hashSet, uint maxCount)
        {
            if (hashSet.Count < maxCount) return;
            var list = new List<KeyTimeEntry<TKey>>(5);
            list.AddRange(hashSet.OrderBy(entry => entry.Timestamp).Take(5));
            foreach (var source in list)
            {
                hashSet.Remove(source);
            }
        }

        internal static List<ParameterModel> GetParameters(ParameterModel[] parameters)
        {
            var list = new List<ParameterModel>();
            if (parameters != null && parameters.Length > 0)
                list.AddRange(parameters);
            return list;
        }

        internal static T GetService<T>(string configName, Func<T> defaultValue) where T : class
        {
            string nameType = ConfigurationManager.AppSettings[configName];
            if (!string.IsNullOrEmpty(nameType))
            {
                Type type = Type.GetType(nameType, false, true);
                if (type == null)
                    throw new TypeLoadException(
                        string.Format(
                            "When load the {1}. Type the name of the {0} can not be found in assemblies.",
                            nameType, configName));
                return (T)type.Assembly.CreateInstance(type.FullName, true);
            }
            T service;
            if (!TryGetService(out service))
                service = defaultValue();
            return service;
        }

        internal static bool TryGetService<T>(out T result) where T : class
        {
            result = default(T);
            if (DependencyResolver.Current == null) return false;
            try
            {
                result = DependencyResolver.Current.GetService<T>();
            }
            catch
            {
                return false;
            }
            return result != null;
        }

        /// <summary>
        ///     Gets the value associated with the specified name.
        /// </summary>
        internal static bool TryFindParameter<T>(this IEnumerable<ParameterModel> parameters, string name, out T result,
                                                 T defaultValue)
        {
            ParameterModel parameter = parameters.FirstOrDefault(model => model.Name.Equals(name));
            if (parameter == null)
            {
                result = defaultValue;
                return false;
            }
            result = (T)parameter.Value;
            return true;
        }

        /// <summary>
        ///     Gets the value associated with the specified name.
        /// </summary>
        internal static bool TryFindParameter<T>(this IEnumerable<ParameterModel> parameters, string name, out T result)
        {
            return TryFindParameter(parameters, name, out result, default(T));
        }

        /// <summary>
        ///     Gets the value associated with the specified name.
        /// </summary>
        internal static T FindParameter<T>(this IEnumerable<ParameterModel> parameters, string name)
        {
            ParameterModel parameter = parameters.FirstOrDefault(model => model.Name.Equals(name));
            if (parameter == null)
                return default(T);
            return (T)parameter.Value;
        }

        /// <summary>
        ///     Determines whether the collection of parameters contains a specific value.
        /// </summary>
        /// <param name="parameters">The specified collection of parameters.</param>
        /// <param name="name">The parameter name for search.</param>
        /// <returns>
        ///     <c>True</c> if the parameter is found in the collection; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsContains(this IEnumerable<ParameterModel> parameters, string name)
        {
            return parameters.Any(model => model.Name.Equals(name));
        }

        #endregion
    }
}