using CMS;

using Kentico.Xperience.AmazonPersonalize.Admin;

[assembly: RegisterImplementation(typeof(IDatasetClientServiceProvider), typeof(DatasetClientServiceProvider), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    /// <summary>
    /// Provider of site-specific <see cref="IDatasetClientService"/>.
    /// </summary>
    public interface IDatasetClientServiceProvider
    {
        /// <summary>
        /// Gets a value indicating whether the Amazon Personalize dataset client service is available for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to test the availability.</param>
        /// <returns>Returns true if the client service is available for <paramref name="siteName"/>, otherwise returns false.</returns>
        bool IsAvailable(string siteName);


        /// <summary>
        /// Gets the Amazon Personalize dataset client services for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the client service.</param>
        /// <returns>Returns the client service, or null if client service is not available for the site.</returns>
        IDatasetClientService Get(string siteName);
    }
}
