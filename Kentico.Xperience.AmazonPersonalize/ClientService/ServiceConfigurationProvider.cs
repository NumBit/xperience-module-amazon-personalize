using System;

using CMS.Base;
using CMS.Core;

namespace Kentico.Xperience.AmazonPersonalize
{
    /// <inheritdoc/>
    public class ServiceConfigurationProvider : IServiceConfigurationProvider
    {
        private const string ACCESS_KEY_ID = "AmazonPersonalize.ContentRecommendation.AccessKey";
        private const string SECRET_ACCESS_KEY = "AmazonPersonalize.ContentRecommendation.SecretKey";
        private const string PERSONALIZED_CAMPAIGN_ARN = "AmazonPersonalize.ContentRecommendation.PersonalizedCampaignArn";
        private const string SIMMILAR_ITEMS_CAMPAIGN_ARN = "AmazonPersonalize.ContentRecommendation.SimmilarItemsCampaignArn";
        private const string DATASET_GROUP_ARN = "AmazonPersonalize.ContentRecommendation.DatasetGroupArn";
        private const string FILTER_WITH_PAGE_TYPES_ARN = "AmazonPersonalize.ContentRecommendation.FilterWithPageTypesArn";
        private const string FILTER_WITHOUT_PAGE_TYPES_ARN = "AmazonPersonalize.ContentRecommendation.FilterWithoutPageTypesArn";
        private const string EVENT_TRACKER_ARN = "AmazonPersonalize.ContentRecommendation.EventTrackerArn";

        private readonly IAppSettingsService appSettingsService;


        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceConfigurationProvider"/> class.
        /// </summary>
        /// <param name="appSettingsService">Application settings service.</param>
        public ServiceConfigurationProvider(IAppSettingsService appSettingsService, ISiteService siteService)
        {
            this.appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
        }


        /// <inheritdoc/>
        public string GetAcessKey(string siteName)
        {
            var keyName = $"{siteName}.{ACCESS_KEY_ID}";

            return appSettingsService[keyName];
        }

        /// <inheritdoc/>
        public string GetSecretKey(string siteName)
        {
            var keyName = $"{siteName}.{SECRET_ACCESS_KEY}";

            return appSettingsService[keyName];
        }


        public string GetPersonalizedCampaignArn(string siteName)
        {
            var keyName = $"{siteName}.{PERSONALIZED_CAMPAIGN_ARN}";

            return appSettingsService[keyName];
        }

        public string GetSimmilarItemsCampaignArn(string siteName)
        {
            var keyName = $"{siteName}.{SIMMILAR_ITEMS_CAMPAIGN_ARN}";

            return appSettingsService[keyName];
        }


        /// <inheritdoc/>
        public string GetDatasetGroupArn(string siteName)
        {
            var keyName = $"{siteName}.{DATASET_GROUP_ARN}";

            return appSettingsService[keyName];
        }


        /// <inheritdoc/>
        public string GetFilterWithPageTypesArn(string siteName)
        {
            var keyName = $"{siteName}.{FILTER_WITH_PAGE_TYPES_ARN}";

            return appSettingsService[keyName];
        }


        /// <inheritdoc/>
        public string GetFilterWithoutPageTypesArn(string siteName)
        {
            var keyName = $"{siteName}.{FILTER_WITHOUT_PAGE_TYPES_ARN}";

            return appSettingsService[keyName];
        }


        /// <inheritdoc/>
        public string GetEventTrackerArn(string siteName)
        {
            var keyName = $"{siteName}.{EVENT_TRACKER_ARN}";

            return appSettingsService[keyName];
        }
    }
}
