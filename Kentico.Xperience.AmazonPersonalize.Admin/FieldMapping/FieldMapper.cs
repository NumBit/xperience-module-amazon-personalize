using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using CMS.DocumentEngine;

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    /// <summary>
    /// Performs mapping of page type fields to Amazon Personalize database item properties.
    /// </summary>
    public class FieldMapper : IFieldMapper
    {
        /// <summary>
        /// Gets the name of the item property the system uses to store the page type.
        /// </summary>
        public const string PAGE_TYPE_PROPERTY_NAME = "PageType";


        /// <summary>
        /// Gets the name of the item property the system uses to store the culture of a page.
        /// </summary>
        public const string CULTURE_PROPERTY_NAME = "Culture";


        /// <summary>
        /// Gets the name of the item property the system uses to store if the page is deleted.
        /// </summary>
        public const string DELETED_PROPERTY_NAME = "Deleted";


        /// <summary>
        /// An ordered collection of individual field mappings for a page type.
        /// </summary>
        private readonly ConcurrentDictionary<string, PageTypeMappings> configurations = new ConcurrentDictionary<string, PageTypeMappings>(4, 4, StringComparer.InvariantCultureIgnoreCase);


        /// <summary>
        /// Gets the <see cref="PageTypeMappings"/> for a site.
        /// </summary>
        /// <param name="siteName">Name of site for which to retrieve the configuration.</param>
        /// <returns>Returns the site's configurations of page type mappings.</returns>
        public PageTypeMappings GetConfigurations(string siteName)
        {
            if (configurations.TryGetValue(siteName, out var configuration))
            {
                return configuration;
            }

            var newConfiguration = new PageTypeMappings();
            if (configurations.TryAdd(siteName, newConfiguration))
            {
                return newConfiguration;
            }

            configurations.TryGetValue(siteName, out var configuration2);

            return configuration2;
        }


        /// <summary>
        /// Maps a page to Amazon Personalize database item.
        /// </summary>
        /// <param name="page">Page to be mapped.</param>
        /// <returns>Returns the mapped Amazon Personalize database item.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the page's site does not contain the configuration for page's type.</exception>
        public Dictionary<string, string> Map(TreeNode page)
        {
            var pageType = page.TypeInfo.ObjectType;
            var siteName = page.NodeSiteName;

            if (!configurations.TryGetValue(siteName, out var fieldTypesMappings))
            {
                throw new InvalidOperationException($"Could not find page type mappings container for site '{siteName}'. Use the {typeof(IFieldMapper)}.{nameof(IFieldMapper.GetConfigurations)} to initialize new mappings container for the site.");
            }

            if (!fieldTypesMappings.Mappings.TryGetValue(pageType, out var mapping))
            {
                throw new InvalidOperationException($"Page type '{pageType}' has no mapping configured. Use the {typeof(IFieldMapper)}.{nameof(IFieldMapper.GetConfigurations)} property to define the page type mapping.");
            }

            var result = new Dictionary<string, string>();

            MapSystemFields(page, result);

            foreach(var field in mapping)
            {
                if (result.ContainsKey(field.TargetName))
                {
                    result[field.TargetName] = MergeValues(GetMappedValue(field, page), result[field.TargetName]); 
                }
                else
                {
                    result[field.TargetName] = GetMappedValue(field, page);
                }
            }

            return result;
        }


        /// <summary>
        /// Gets source value from <paramref name="page"/>.
        /// </summary>
        /// <param name="fieldMapping">Descriptor of the field mapping.</param>
        /// <param name="page">Page to be mapped.</param>
        /// <returns>Returns the value to be mapped to the Amazon Personalize DB.</returns>
        protected virtual string GetMappedValue(FieldMapping fieldMapping, TreeNode page)
        {
            if (fieldMapping.SourceValue != null)
            {
                return fieldMapping.SourceValue(page);
            }
            return page.GetValue(fieldMapping.SourceName).ToString();
        }


        /// <summary>
        /// Maps system fields of <paramref name="page"/> to the resulting Amazon Personalize item.
        /// </summary>
        /// <param name="page">Page to be mapped.</param>
        /// <param name="result">Amazon Personalize database item to map into.</param>
        /// <seealso cref="PAGE_TYPE_PROPERTY_NAME"/>
        /// <seealso cref="CULTURE_PROPERTY_NAME"/>
        /// <seealso cref="DELETED_PROPERTY_NAME"/>
        protected virtual void MapSystemFields(TreeNode page, Dictionary<string, string> result)
        {
            result[PAGE_TYPE_PROPERTY_NAME] = page.TypeInfo.ObjectType.ToLowerInvariant();
            result[CULTURE_PROPERTY_NAME] = page.DocumentCulture.ToLowerInvariant();
            result[DELETED_PROPERTY_NAME] = "false";
        }


        /// <summary>
        /// Merges values when multiple page fields are mapped to a single Amazon Personalize dataset property.
        /// </summary>
        /// <param name="fieldValue">Page field value to be merged with existing value in <paramref name="targetPropertyValue"/>.</param>
        /// <param name="targetPropertyValue">Current property value to be merged with <paramref name="fieldValue"/>.</param>
        /// <returns>Returns the merged value for <paramref name="fieldValue"/> and <paramref name="targetPropertyValue"/>.</returns>
        protected virtual string MergeValues(string fieldValue, string targetPropertyValue)
        {
            return $"{targetPropertyValue} {fieldValue}";
        }
    }
}
