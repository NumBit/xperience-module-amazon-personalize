using CMS;

using Kentico.Xperience.AmazonPersonalize;

[assembly: RegisterImplementation(typeof(IServiceConfigurationProvider), typeof(ServiceConfigurationProvider), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace Kentico.Xperience.AmazonPersonalize
{
    /// <summary>
    /// Provides configuration of the Amazon Personalize services.
    /// </summary>
    public interface IServiceConfigurationProvider
    {
        /// <summary>
        /// Gets the Amazon Personalize acess key.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the key.</param>
        /// <returns>Returns the acess key for <paramref name="siteName"/>, or null if not configured.</returns>
        string GetAcessKey(string siteName);


        /// <summary>
        /// Gets the Amazon Personalize secret key.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the key.</param>
        /// <returns>Returns the secret key for <paramref name="siteName"/>, or null if not configured.</returns>
        string GetSecretKey(string siteName);


        /// <summary>
        /// Gets the Amazon Personalize campaign arn for personalized recommendations.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the campaign arn.</param>
        /// <returns>Returns the campaign arn for <paramref name="siteName"/>, or null if not configured.</returns>
        string GetPersonalizedCampaignArn(string siteName);


        /// <summary>
        /// Gets the Amazon Personalize campaign arn for simmilar items recommendations.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the campaign arn.</param>
        /// <returns>Returns the campaign arn for <paramref name="siteName"/>, or null if not configured.</returns>
        string GetSimmilarItemsCampaignArn(string siteName);


        /// <summary>
        /// Gets the Amazon Personalize dataset group arn.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the dataset group arn.</param>
        /// <returns>Returns the dataset group arn for <paramref name="siteName"/>, or null if not configured.</returns>
        string GetDatasetGroupArn(string siteName);


        /// <summary>
        /// Gets the Amazon Personalize filter with included page types arn.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the filter arn.</param>
        /// <returns>Returns the filter arn for <paramref name="siteName"/>, or null if not configured.</returns>
        string GetFilterWithPageTypesArn(string siteName);


        /// <summary>
        /// Gets the Amazon Personalize arn of filter which returns all page types.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the filter arn.</param>
        /// <returns>Returns the filter arn for <paramref name="siteName"/>, or null if not configured.</returns>
        string GetFilterWithoutPageTypesArn(string siteName);


        /// <summary>
        /// Gets the Amazon Personalize arn of event tracker.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the event tracker arn.</param>
        /// <returns>Returns the event tracker arn for <paramref name="siteName"/>, or null if not configured.</returns>
        string GetEventTrackerArn(string siteName);


        /// <summary>
        /// Gets the Amazon Personalize region endpoint.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the region endpoint.</param>
        /// <returns>Returns the region endpoint for <paramref name="siteName"/>, or null if not configured.</returns>
        string GetRegionEndpoint(string siteName);
    }
}
