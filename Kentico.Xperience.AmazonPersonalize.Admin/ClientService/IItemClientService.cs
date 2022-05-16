using CMS;
using CMS.DocumentEngine;

using Kentico.Xperience.AmazonPersonalize.Admin;

[assembly: RegisterImplementation(typeof(IItemClientService), typeof(ItemClientService), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]


namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    /// <summary>
    /// Manages pages in the corresponding site's Amazon Personalize dataset.
    /// </summary>
    public interface IItemClientService
    {
        /// <summary>
        /// Creates or updates <paramref name="page"/> in the Amazon Personalize dataset. 
        /// Page is updated if page with identical identifier already exists in the Amazon Personalize dataset.
        /// </summary>
        /// <param name="page">Page to be created/updated in the Amazon Personalize dataset.</param>
        void PutItem(TreeNode page);


        /// <summary>
        /// Deletes <paramref name="page"/> from the Amazon Personalize dataset.
        /// </summary>
        /// <param name="page">Page to be deleted from the Amazon Personalize dataset.</param>
        void DeleteItem(TreeNode page);
    }
}