using System;
using System.Collections.Generic;

using CMS;
using CMS.DocumentEngine;

using Kentico.Xperience.AmazonPersonalize;

[assembly: RegisterImplementation(typeof(IAmazonPersonalizeService), typeof(AmazonPersonalizeService), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace Kentico.Xperience.AmazonPersonalize
{
    /// <summary>
    /// Manages content recommendation operations in the corresponding site's Amazon Personalize dataset.
    /// </summary>
    public interface IAmazonPersonalizeService
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
        IEnumerable<PageIdentifier> GetPagesRecommendationForContact(string siteName, Guid contactGuid, int count, string culture, IEnumerable<string> pageTypes = null);


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


        /// <summary>
        /// Logs contact's view of <paramref name="page"/> .
        /// </summary>
        /// <param name="page">Page whose view to log.</param>
        /// <param name="contactGuid">GUID of the contact who viewed the page.</param>
        /// <param name="sessionId">ID of users session. Used to combine activity to user before they logged in. If null, unique ID is generated for each event.</param>
        /// <param name="impression">List of pages shown to the user.</param>
        /// <exception cref="InvalidOperationException">Thrown when the Amazon Personalize client services are not configured for <paramref name="page"/> <see cref="TreeNode.NodeSiteName"/>.</exception>
        void LogPageView(TreeNode page, Guid contactGuid, string sessionId, List<TreeNode> impression = null);
    }
}