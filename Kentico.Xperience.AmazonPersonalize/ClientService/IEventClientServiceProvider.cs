using CMS;
using Kentico.Xperience.AmazonPersonalize;

[assembly: RegisterImplementation(typeof(IEventClientServiceProvider), typeof(EventClientServiceProvider), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]


namespace Kentico.Xperience.AmazonPersonalize
{
    public interface IEventClientServiceProvider
    {
        bool IsAvailable(string siteName);
        
        IEventClientService Get(string siteName);
    }
}