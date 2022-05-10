using System;

using CMS.Base;
using CMS.Core;

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    /// <summary>
    /// Provides configuration of the Amazon Personalize service.
    /// </summary>
    public class ServiceConfigurationProvider : IServiceConfigurationProvider
    {
        private const string ACCESS_KEY = "AmazonPersonalize.ContentRecommendation.AccessKey";
        private const string SECRET_KEY = "AmazonPersonalize.ContentRecommendation.SecretKey";
        private const string DATASET_GROUP_ARN = "AmazonPersonalize.ContentRecommendation.DatasetGrpouArn";
        private const string ITEMS_SCHEMA_ARN = "AmazonPersonalize.ContentRecommendation.ItemsSchemaArn";
        private const string USERS_SCHEMA_ARN = "AmazonPersonalize.ContentRecommendation.UsersSchemaArn";
        private const string INTERACTIONS_SCHEMA_ARN = "AmazonPersonalize.ContentRecommendation.InteractionsSchemaArn";

        private readonly IAppSettingsService appSettingsService;


        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceConfigurationProvider"/> class.
        /// </summary>
        /// <param name="appSettingsService">Application settings service.</param>
        public ServiceConfigurationProvider(IAppSettingsService appSettingsService, ISiteService siteService)
        {
            this.appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
        }


        public string GetAcessKey(string siteName)
        {
            var keyName = $"{siteName}.{ACCESS_KEY}";

            return appSettingsService[keyName];
        }


        public string GetSecretKey(string siteName)
        {
            var keyName = $"{siteName}.{SECRET_KEY}";

            return appSettingsService[keyName];
        }


        public string GetDatasetGroupArn(string siteName)
        {
            var keyName = $"{siteName}.{DATASET_GROUP_ARN}";

            return appSettingsService[keyName];
        }


        public string GetItemsDatasetArn(string siteName)
        {
            var keyName = $"{siteName}.{DATASET_GROUP_ARN}";

            return $"{GetDatasetArn(keyName)}/ITEMS";
        }


        public string GetItemSchemaArn(string siteName)
        {
            var keyName = $"{siteName}.{ITEMS_SCHEMA_ARN}";

            return appSettingsService[keyName];
        }


        public string GetInteractionsDatasetArn(string siteName)
        {
            var keyName = $"{siteName}.{DATASET_GROUP_ARN}";

            return $"{GetDatasetArn(keyName)}/INTERACTIONS";
        }


        public string GetInteractionsSchemaArn(string siteName)
        {
            var keyName = $"{siteName}.{INTERACTIONS_SCHEMA_ARN}";

            return appSettingsService[keyName];
        }


        public string GetUsersDatasetArn(string siteName)
        {
            var keyName = $"{siteName}.{DATASET_GROUP_ARN}";

            return $"{GetDatasetArn(keyName)}/USERS";
        }


        public string GetUsersSchemaArn(string siteName)
        {
            var keyName = $"{siteName}.{USERS_SCHEMA_ARN}";

            return appSettingsService[keyName];
        }
        

        private string GetDatasetArn(string keyName)
        {
            return appSettingsService[keyName].Replace("dataset-group", "dataset");
        }
    }
}
