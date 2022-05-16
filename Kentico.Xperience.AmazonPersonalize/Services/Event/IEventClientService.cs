using System;
using System.Collections.Generic;

using CMS.DocumentEngine;

namespace Kentico.Xperience.AmazonPersonalize
{
    /// <summary>
    /// Encapsulates logging events for Amazon Personalize.
    /// </summary>
    public interface IEventClientService
    {
        /// <summary>
        /// Logs contact's view of <paramref name="page"/> .
        /// </summary>
        /// <param name="page">Page whose view to log.</param>
        /// <param name="contactGuid">GUID of the contact who viewed the page.</param>
        /// <param name="sessionId">ID of users session. Used to combine activity to user before they logged in. If null, unique ID is generated for each event.</param>
        /// <param name="impression">List of pages shown to the user.</param>
        /// <exception cref="InvalidOperationException">Thrown when the Amazon Personalize client services are not configured for <paramref name="page"/> <see cref="TreeNode.NodeSiteName"/>.</exception>
        void LogPageView(TreeNode page, Guid contactGuid, string sessionId = null, IEnumerable<TreeNode> impression = null);
    }
}