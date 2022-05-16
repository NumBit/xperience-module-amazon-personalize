using CMS;

using Kentico.Xperience.AmazonPersonalize;

[assembly: RegisterImplementation(typeof(IEventTrackerServiceProvider), typeof(EventTrackerServiceProvider), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]


namespace Kentico.Xperience.AmazonPersonalize
{
    /// <summary>
    /// Provider of site-specific <see cref="IEventTrackerService"/>.
    /// </summary>
    public interface IEventTrackerServiceProvider
    {
        /// <summary>
        /// Gets a value indicating whether the Amazon Personalize event tracker service is available for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to test the availability.</param>
        /// <returns>Returns true if the event tracker service is available for <paramref name="siteName"/>, otherwise returns false.</returns>
        bool IsAvailable(string siteName);


        /// <summary>
        /// Gets the Amazon Personalize event tracker services for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the client service.</param>
        /// <returns>Returns the event tracker service, or null if event tracker service is not available for the site.</returns>
        IEventTrackerService Get(string siteName);
    }
}