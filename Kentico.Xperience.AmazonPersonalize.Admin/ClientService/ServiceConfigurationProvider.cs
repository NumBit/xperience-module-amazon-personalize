using System;

using CMS.Base;
using CMS.Core;

namespace Kentico.Xperience.AmazonPersonalize.Admin
{

    /// <summary>
    /// Provides configuration of the Amazon Personalize services.
    /// </summary>
    public class ServiceConfigurationProvider : IServiceConfigurationProvider
    {
        private const string ACCESS_KEY = "AmazonPersonalize.ContentRecommendation.AccessKey";
        private const string SECRET_KEY = "AmazonPersonalize.ContentRecommendation.SecretKey";
        private const string ITEMS_DATASET_ARN = "AmazonPersonalize.ContentRecommendation.ItemsDatasetArn";
        private const string REGION_ENDPOINT = "AmazonPersonalize.ContentRecommendation.RegionEndpoint";

        private readonly IAppSettingsService appSettingsService;


        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceConfigurationProvider"/> class.
        /// </summary>
        /// <param name="appSettingsService">Application settings service.</param>
        public ServiceConfigurationProvider(IAppSettingsService appSettingsService)
        {
            this.appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
        }


        /// <inheritdoc/>
        public string GetAcessKey(string siteName)
        {
            var keyName = $"{siteName}.{ACCESS_KEY}";

            return appSettingsService[keyName];
        }


        /// <inheritdoc/>
        public string GetSecretKey(string siteName)
        {
            var keyName = $"{siteName}.{SECRET_KEY}";

            return appSettingsService[keyName];
        }


        /// <inheritdoc/>
        public string GetItemsDatasetArn(string siteName)
        {
            var keyName = $"{siteName}.{ITEMS_DATASET_ARN}";

            return appSettingsService[keyName];
        }


        /// <inheritdoc/>
        public string GetRegionEndpoint(string siteName)
        {
            var keyName = $"{siteName}.{REGION_ENDPOINT}";

            return appSettingsService[keyName];
        }
    }
}
