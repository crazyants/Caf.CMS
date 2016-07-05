using CAF.Infrastructure.Core;
using System;
using System.Linq;

namespace CAF.WebSite.Application.Services.Common
{
    public static class GenericAttributeExtentions
    {
        /// <summary>
        /// Get an attribute of an entity
        /// </summary>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="key">Key</param>
        /// <param name="siteId">Load a value specific for a certain store; pass 0 to load a value shared for all stores</param>
        /// <returns>Attribute</returns>
        public static TPropType GetAttribute<TPropType>(this BaseEntity entity, string key, int siteId = 0)
        {
            var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            return GetAttribute<TPropType>(entity, key, genericAttributeService, siteId);
        }
        /// <summary>
        /// Get an attribute of an entity
        /// </summary>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="key">Key</param>
        /// <param name="genericAttributeService">GenericAttributeService</param>
        /// <param name="siteId">Load a value specific for a certain store; pass 0 to load a value shared for all stores</param>
        /// <returns>Attribute</returns>
        public static TPropType GetAttribute<TPropType>(this BaseEntity entity,
            string key, IGenericAttributeService genericAttributeService, int siteId = 0)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (genericAttributeService == null)
                genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();

            string keyGroup = entity.GetUnproxiedEntityType().Name;

            return genericAttributeService.GetAttribute<TPropType>(keyGroup, entity.Id, key, siteId);

            #region Old
            //var props = genericAttributeService.GetAttributesForEntity(entity.Id, keyGroup);
            ////little hack here (only for unit testing). we should write expect-return rules in unit tests for such cases
            //if (props == null)
            //    return default(TPropType);
            //props = props.Where(x => x.SiteId == siteId).ToList();
            //if (props.Count == 0)
            //    return default(TPropType);

            //var prop = props.FirstOrDefault(ga =>
            //    ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

            //if (prop == null || string.IsNullOrEmpty(prop.Value))
            //    return default(TPropType);

            //return CommonHelper.To<TPropType>(prop.Value);
            #endregion
        }

        public static TPropType GetAttribute<TPropType>(this IGenericAttributeService genericAttributeService, string entityName, int entityId, string key, int siteId = 0)
        {
            Guard.ArgumentNotNull(() => genericAttributeService);
            Guard.ArgumentNotEmpty(() => entityName);

            var props = genericAttributeService.GetAttributesForEntity(entityId, entityName);

            // little hack here (only for unit testing). we should write expect-return rules in unit tests for such cases
            if (props == null)
            {
                return default(TPropType);
            }

            //if (!props.Any(x => x.SiteId == siteId))
            //{
            //    return default(TPropType);
            //}

            var prop = props.FirstOrDefault(ga => ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

            if (prop == null || prop.Value.IsEmpty())
            {
                return default(TPropType);
            }

            return prop.Value.Convert<TPropType>();
        }
    }
}
