using System.Globalization;
using StudioX.Configuration;
using StudioX.Domain.Uow;
using StudioX.Localization;
using StudioX.Localization.Sources;
using StudioX.ObjectMapping;
using Castle.Core.Logging;

namespace StudioX
{
    /// <summary>
    /// This class can be used as a base class for services.
    /// It has some useful objects property-injected and has some basic methods
    /// most of services may need to.
    /// </summary>
    public abstract class StudioXServiceBase
    {
        /// <summary>
        /// Reference to the setting manager.
        /// </summary>
        public ISettingManager SettingManager { get; set; }

        /// <summary>
        /// Reference to <see cref="IUnitOfWorkManager"/>.
        /// </summary>
        public IUnitOfWorkManager UnitOfWorkManager
        {
            get
            {
                if (unitOfWorkManager == null)
                {
                    throw new StudioXException("Must set UnitOfWorkManager before use it.");
                }

                return unitOfWorkManager;
            }
            set => unitOfWorkManager = value;
        }
        private IUnitOfWorkManager unitOfWorkManager;

        /// <summary>
        /// Gets current unit of work.
        /// </summary>
        protected IActiveUnitOfWork CurrentUnitOfWork => UnitOfWorkManager.Current;

        /// <summary>
        /// Reference to the localization manager.
        /// </summary>
        public ILocalizationManager LocalizationManager { get; set; }

        /// <summary>
        /// Gets/sets name of the localization source that is used in this application service.
        /// It must be set in order to use <see cref="L(string)"/> and <see cref="L(string,CultureInfo)"/> methods.
        /// </summary>
        protected string LocalizationSourceName { get; set; }

        /// <summary>
        /// Gets localization source.
        /// It's valid if <see cref="LocalizationSourceName"/> is set.
        /// </summary>
        protected ILocalizationSource LocalizationSource
        {
            get
            {
                if (LocalizationSourceName == null)
                {
                    throw new StudioXException("Must set LocalizationSourceName before, in order to get LocalizationSource");
                }

                if (localizationSource == null || localizationSource.Name != LocalizationSourceName)
                {
                    localizationSource = LocalizationManager.GetSource(LocalizationSourceName);
                }

                return localizationSource;
            }
        }
        private ILocalizationSource localizationSource;

        /// <summary>
        /// Reference to the logger to write logs.
        /// </summary>
        public ILogger Logger { protected get; set; }

        /// <summary>
        /// Reference to the object to object mapper.
        /// </summary>
        public IObjectMapper ObjectMapper { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected StudioXServiceBase()
        {
            Logger = NullLogger.Instance;
            ObjectMapper = NullObjectMapper.Instance;
            LocalizationManager = NullLocalizationManager.Instance;
        }

        /// <summary>
        /// Gets localized string for given key name and current language.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name)
        {
            return LocalizationSource.GetString(name);
        }

        /// <summary>
        /// Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected string L(string name, params object[] args)
        {
            return LocalizationSource.GetString(name, args);
        }

        /// <summary>
        /// Gets localized string for given key name and specified culture information.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, CultureInfo culture)
        {
            return LocalizationSource.GetString(name, culture);
        }

        /// <summary>
        /// Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected string L(string name, CultureInfo culture, params object[] args)
        {
            return LocalizationSource.GetString(name, culture, args);
        }
    }
}