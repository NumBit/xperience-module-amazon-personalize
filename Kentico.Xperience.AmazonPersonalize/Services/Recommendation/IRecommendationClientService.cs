
using System;
using System.Collections.Generic;

using CMS.DocumentEngine;

namespace Kentico.Xperience.AmazonPersonalize
{
    /// <summary>
    /// Encapsulates retrieving recommendations from Amazon Personalize runtime client.
    /// </summary>
    public interface IRecommendationClientService
    {
        /// <summary>
        /// Gets recommendation for pages for a contact.
        /// </summary>
        /// <param name="siteName">Name of site for which to retrieve the recommendation.</param>
        /// <param name="contactGuid">GUID of the contact for which to recommend.</param>
        /// <param name="count">Number of items to return.</param>
        /// <param name="culture">Culture to be considered for recommendation.</param>
        /// <param name="pageTypes">Constrain on page types to be considered for recommendation, or null.</param>
        /// <returns>Returns an enumeration of identifiers of recommended pages.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the Amazon Personalize client services are not configured for <paramref name="siteName"/>.</exception>
        IEnumerable<PageIdentifier> GetRecommendationsForContact(string siteName, Guid contactGuid, int count, string culture, IEnumerable<string> pageTypes = null);


        /// <summary>
        /// Gets recommendation of sillilar pages to the <paramref name="page"/>.
        /// </summary>
        /// <param name="page">Page for which to recommend simmilar pages. If page is null or not found, most popular pages are retrieved.</param>
        /// <param name="count">Number of items to return.</param>
        /// <param name="culture">Culture to be considered for recommendation.</param>
        /// <param name="pageTypes">Constrain on page types to be considered for recommendation, or null.</param>
        /// <returns>Returns an enumeration of identifiers of recommended pages.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the Amazon Personalize client services are not configured for <paramref name="siteName"/>.</exception>

        IEnumerable<PageIdentifier> GetPagesRecommendationForPage(TreeNode page, string siteName, int count, string culture, IEnumerable<string> pageTypes = null);
    }
}