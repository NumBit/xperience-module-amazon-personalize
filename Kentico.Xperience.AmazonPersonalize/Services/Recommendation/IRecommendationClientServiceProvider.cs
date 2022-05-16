using CMS;

using Kentico.Xperience.AmazonPersonalize;

[assembly: RegisterImplementation(typeof(IRecommendationClientServiceProvider), typeof(RecommendationClientServiceProvider), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace Kentico.Xperience.AmazonPersonalize
{
    /// <summary>
    /// Provider of site-specific <see cref="IRecommendationClientService"/>.
    /// </summary>
    public interface IRecommendationClientServiceProvider
    {
        /// <summary>
        /// Gets a value indicating whether the Amazon Personalize recommendation client service is available for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to test the availability.</param>
        /// <returns>Returns true if the recommendation client service is available for <paramref name="siteName"/>, otherwise returns false.</returns>
        bool IsAvailable(string siteName);


        /// <summary>
        /// Gets the Amazon Personalize recommendation client services for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the client service.</param>
        /// <returns>Returns the recommendation client service, or null if recommendation client service is not available for the site.</returns>
        IRecommendationClientService Get(string siteName);
    }
}
