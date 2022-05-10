using CMS;
using Kentico.Xperience.AmazonPersonalize.Admin;

[assembly: RegisterImplementation(typeof(IItemClientServiceProvider), typeof(ItemClientServiceProvider), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]


namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    public interface IItemClientServiceProvider
    {
        bool IsAvailable(string siteName);
        
        IItemClientService Get(string siteName);
    }
}