using CMS;
using CMS.DocumentEngine;

using Kentico.Xperience.AmazonPersonalize.Admin;

[assembly: RegisterImplementation(typeof(IAmazonPersonalizeService), typeof(AmazonPersonalizeService), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    public interface IAmazonPersonalizeService
    {
        bool IsProcessed(TreeNode page);

        void Init(string siteName);

        void Reset(string siteName);

        void Send(TreeNode page);

        void SendAll(string siteName);

        void PageCreated(TreeNode page);

        void PageUpdated(TreeNode page);

        void PageDeleted(TreeNode page);
    }
}
