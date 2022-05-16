using CMS;
using Kentico.Xperience.AmazonPersonalize.Admin;

[assembly: RegisterImplementation(typeof(IServiceConfigurationProvider), typeof(ServiceConfigurationProvider), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]


namespace Kentico.Xperience.AmazonPersonalize.Admin
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
        /// Gets the Amazon Personalize items dataset arn.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the items dataset arn.</param>
        /// <returns>Returns the items dataset arn for <paramref name="siteName"/>, or null if not configured.</returns>
        string GetItemsDatasetArn(string siteName);
        

        /// <summary>
        /// Gets the Amazon Personalize region endpoint.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the region endpoint.</param>
        /// <returns>Returns the region endpoint for <paramref name="siteName"/>, or null if not configured.</returns>
        string GetRegionEndpoint(string siteName);
    }
}