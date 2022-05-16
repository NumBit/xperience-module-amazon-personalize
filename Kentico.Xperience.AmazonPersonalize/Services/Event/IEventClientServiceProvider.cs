using CMS;
using Kentico.Xperience.AmazonPersonalize;

[assembly: RegisterImplementation(typeof(IEventClientServiceProvider), typeof(EventClientServiceProvider), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]


namespace Kentico.Xperience.AmazonPersonalize
{
    /// <summary>
    /// Provider of site-specific <see cref="IEventClientService"/>.
    /// </summary>
    public interface IEventClientServiceProvider
    {
        /// <summary>
        /// Gets a value indicating whether the Amazon Personalize event client service is available for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to test the availability.</param>
        /// <returns>Returns true if the event client service is available for <paramref name="siteName"/>, otherwise returns false.</returns>
        bool IsAvailable(string siteName);


        /// <summary>
        /// Gets the Amazon Personalize event client services for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the client service.</param>
        /// <returns>Returns the event client service, or null if event client service is not available for the site.</returns>
        IEventClientService Get(string siteName);
    }
}