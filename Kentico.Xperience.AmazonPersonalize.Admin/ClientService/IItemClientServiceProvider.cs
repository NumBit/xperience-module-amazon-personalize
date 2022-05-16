using CMS;
using Kentico.Xperience.AmazonPersonalize.Admin;

[assembly: RegisterImplementation(typeof(IItemClientServiceProvider), typeof(ItemClientServiceProvider), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]


namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    /// <summary>
    /// Provider of site-specific <see cref="IItemClientServiceProvider"/>.
    /// </summary>
    public interface IItemClientServiceProvider
    {
        /// <summary>
        /// Gets a value indicating whether the Amazon Personalize item client service is available for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to test the availability.</param>
        /// <returns>Returns true if the client service is available for <paramref name="siteName"/>, otherwise returns false.</returns>
        bool IsAvailable(string siteName);


        /// <summary>
        /// Gets the Amazon Personalize item client services for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the client service.</param>
        /// <returns>Returns the client service, or null if client service is not available for the site.</returns>
        IItemClientService Get(string siteName);
    }
}