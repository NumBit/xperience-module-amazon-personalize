using CMS;
using CMS.DocumentEngine;

using Kentico.Xperience.AmazonPersonalize.Admin;

[assembly: RegisterImplementation(typeof(IAmazonPersonalizeService), typeof(AmazonPersonalizeService), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    /// <summary>
    /// Manages pages in the corresponding site's Amazon Personalize Amazon Personalize.
    /// </summary>
    public interface IAmazonPersonalizeService
    {
        /// <summary>
        /// Gets a value indicating whether <paramref name="page"/> is configured to be mapped and sent to the Amazon Personalize dataset.
        /// </summary>
        /// <param name="page">Page to be tested.</param>
        /// <returns>Returns true if the <paramref name="page"/> is configured for the Amazon Personalize dataset.</returns>
        /// <remarks>
        /// Processed pages are those whose culture matches the <see cref="IFieldMapper"/>'s <see cref="PageTypeMappings.IncludedCultures"/> set and whose site contains a corresponding page type mapping in <see cref="PageTypeMappings.Mappings"/>.
        /// </remarks>
        bool IsProcessed(TreeNode page);


        /// <summary>
        /// Sends <paramref name="page"/> to the Amazon Personalize dataset.
        /// The operation is void if the Amazon Personalize service is not configured for the page's site.
        /// </summary>
        /// <param name="page">Page to be sent to the Amazon Personalize dataset.</param>
        void Send(TreeNode page);


        /// <summary>
        /// Sends all pages of configured page types to the Amazon Personalize dataset.
        /// The operation is void if the Amazon Personalize service is not configured for the page's site.
        /// </summary>
        /// <param name="siteName">Name of site whose pages are to be sent.</param>
        /// <remarks>
        /// Only published versions of pages in included cultures are sent to the Amazon Personalize dataset.
        /// </remarks>
        void SendAll(string siteName);


        /// <summary>
        /// Deletes <paramref name="page"/> from the Amazon Personalize dataset.
        /// The operation is void if the Amazon Personalize service is not configured for the page's site.
        /// </summary>
        /// <param name="page">Page to be deleted from the Amazon Personalize dataset.</param>
        void Delete(TreeNode page);


        /// <summary>
        /// Handles the event of page creation. If the <paramref name="page"/> matches the <see cref="IsProcessed(TreeNode)"/> and <see cref="IsPublished(TreeNode)"/>
        /// predicates, it is sent to the Amazon Personalize dataset.
        /// </summary>
        /// <param name="page">Page which was created.</param>
        /// <seealso cref="DocumentEvents.Insert"/>
        void PageCreated(TreeNode page);


        /// <summary>
        /// Handles the event of page update. If the <paramref name="page"/> matches the <see cref="IsProcessed(TreeNode)"/> predicate,
        /// it is either sent to the Amazon Personalize dataset (if <see cref="IsPublished(TreeNode)"/> is true), or deleted from the Amazon Personalize dataset.
        /// </summary>
        /// <param name="page">Page which was updated.</param>
        /// <seealso cref="DocumentEvents.Update"/>
        void PageUpdated(TreeNode page);


        /// <summary>
        /// Handles the event of page delete. If the <paramref name="page"/> matches the <see cref="IsProcessed(TreeNode)"/> predicate,
        /// it is deleted from the Amazon Personalize dataset.
        /// </summary>
        /// <param name="page">Page to be deleted.</param>
        /// <seealso cref="DocumentEvents.Delete"/>
        void PageDeleted(TreeNode page);
    }
}
