using CMS;
using CMS.DocumentEngine;

using Kentico.Xperience.AmazonPersonalize.Admin;

[assembly: RegisterImplementation(typeof(IItemClientService), typeof(ItemClientService), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]


namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    public interface IItemClientService
    {
        void PutItem(TreeNode page);

        void DeleteItem(TreeNode page);

    }
}