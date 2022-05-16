using System;
using System.Collections.Generic;

using CMS;
using CMS.DocumentEngine;

using Kentico.Xperience.AmazonPersonalize.Admin;

[assembly: RegisterImplementation(typeof(IFieldMapper), typeof(FieldMapper), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    /// <summary>
    /// Performs mapping of page type fields to Amazon Personalize dataset item properties.
    /// </summary>
    public interface IFieldMapper
    {
        /// <summary>
        /// Gets the <see cref="PageTypeMappings"/> for a site.
        /// </summary>
        /// <param name="siteName">Name of site for which to retrieve the configuration.</param>
        /// <returns>Returns the site's configurations of page type mappings.</returns>
        PageTypeMappings GetConfigurations(string siteName);


        /// <summary>
        /// Maps a page to Amazon Personalize dataset item.
        /// </summary>
        /// <param name="page">Page to be mapped.</param>
        /// <returns>Returns the mapped Amazon Personalize dataset item.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the page's site does not contain the configuration for page's type.</exception>
        Dictionary<string, string> Map(TreeNode page);
    }
}
