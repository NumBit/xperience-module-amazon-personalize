using CMS;
using Amazon.Personalize;

using Kentico.Xperience.AmazonPersonalize.Admin;

[assembly: RegisterImplementation(typeof(IDatasetManager), typeof(DatasetManager), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    /// <summary>
    /// Performs initialization and reset of the Amazon Personalize dataset.
    /// </summary>
    public interface IDatasetManager
    {
        /// <summary>
        /// Initializes the Amazon Personalize database based on set <see cref="IServiceConfigurationProvider"/> dataset schemas.
        /// The initialization is performed only once per application lifetime unless <see cref="AmazonPersonalizeService.Reset(string)"/> is performed.
        /// </summary>
        /// <param name="amazonClient">Amazon Personalize client to be used for the Amazon Personalize dataset initialization.</param>
        /// <param name="siteName">Name of site to which the <see cref="AmazonPersonalizeClient"/> belongs.</param>
        void Init(AmazonPersonalizeClient amazonClient, string siteName);


        /// <summary>
        /// Resets the Amazon Personalize dataset content and structure.
        /// </summary>
        /// <param name="amazonClient">Amazon Personalize client to be used for the reset.</param>
        /// <param name="siteName">Name of site to which the <see cref="AmazonPersonalizeClient"/> belongs.</param>
        void Reset(AmazonPersonalizeClient amazonClient, string siteName);
    }
}
