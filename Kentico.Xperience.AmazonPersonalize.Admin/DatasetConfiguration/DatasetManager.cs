using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Personalize;
using Amazon.Personalize.Model;
using CMS.Core;

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    /// <summary>
    /// Performs initialization and reset of the Amazon Personalize dataset.
    /// </summary>
    public class DatasetManager : IDatasetManager
    {
        private readonly HashSet<string> structureEnsured = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        private readonly object syncLock = new object();


        private readonly IEventLogService eventLogService;
        private readonly IServiceConfigurationProvider serviceConfigurationProvider;


        public DatasetManager(IEventLogService eventLogService, IServiceConfigurationProvider serviceConfigurationProvider)
        {
            this.eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
            this.serviceConfigurationProvider = serviceConfigurationProvider ?? throw new ArgumentNullException(nameof(serviceConfigurationProvider));
        }


        /// <summary>
        /// Initializes the Amazon Personalize database based on its <see cref="IDatasetConfiguration"/>.
        /// The initialization is performed only once per application lifetime unless <see cref="Reset(string)"/> is performed.
        /// </summary>
        /// <param name="amazonClient">Amazon Personalize client to be used for the Amazon Personalize database initialization.</param>
        /// <param name="siteName">Name of site to which the <see cref="AmazonPersonalizeClient"/> belongs.</param>
        public void Init(AmazonPersonalizeClient amazonClient, string siteName)
        {
            if (structureEnsured.Contains(siteName))
            {
                return;
            }

            lock (syncLock)
            {
                if (structureEnsured.Contains(siteName))
                {
                    return;
                }

                InitializeItemProperties(amazonClient, siteName);

                structureEnsured.Add(siteName);
            }
        }


        /// <summary>
        /// Resets the Amazon Personalize database content and structure.
        /// </summary>
        /// <param name="amazonClient">Amazon Personalize client to be used for the reset.</param>
        /// <param name="siteName">Name of site to which the <see cref="AmazonPersonalizeClient"/> belongs.</param>
        public void Reset(AmazonPersonalizeClient amazonClient, string siteName)
        {
            lock (syncLock)
            {
                structureEnsured.Remove(siteName);
            }

            DeleteDataset(amazonClient, siteName, "USERS");
            DeleteDataset(amazonClient, siteName, "ITEMS");
            DeleteDataset(amazonClient, siteName, "INTERACTIONS");
        }

        private void DeleteDataset(AmazonPersonalizeClient amazonClient, string siteName, string dataset)
        {
            var request = new DeleteDatasetRequest
            {
                DatasetArn = $"{serviceConfigurationProvider.GetDatasetGroupArn(siteName)}/{dataset}"
            };
            amazonClient.DeleteDatasetAsync(request);
        }


        protected virtual void InitializeItemProperties(AmazonPersonalizeClient amazonClient, string siteName)
        {
            try
            {
                InitializeItemsDataset(amazonClient, siteName);
                InitializeInteractionsDataset(amazonClient, siteName);
                InitializeUsersDataset(amazonClient, siteName);
            }
            catch (Exception ex)
            {
                eventLogService.LogException("AmazonPersonalize.", "Datasets init", ex);

                throw;
            }
        }

        private string InitializeItemsDataset(AmazonPersonalizeClient amazonClient, string siteName)
        {
            var request = new CreateDatasetRequest
            {
                Name = "Items-Dataset",
                DatasetGroupArn = serviceConfigurationProvider.GetDatasetGroupArn(siteName),
                SchemaArn = serviceConfigurationProvider.GetItemSchemaArn(siteName),
                DatasetType = "Items"
            };

            var resp = amazonClient.CreateDatasetAsync(request).Result;
            return resp.DatasetArn;
        }

        private string InitializeInteractionsDataset(AmazonPersonalizeClient amazonClient, string siteName)
        {
            var request = new CreateDatasetRequest
            {
                Name = "Interactions-Dataset",
                DatasetGroupArn = serviceConfigurationProvider.GetDatasetGroupArn(siteName),
                SchemaArn = serviceConfigurationProvider.GetInteractionsSchemaArn(siteName),
                DatasetType = "Interactions"
            };
            var resp = amazonClient.CreateDatasetAsync(request).Result;
            return resp.DatasetArn;
        }

        private string InitializeUsersDataset(AmazonPersonalizeClient amazonClient, string siteName)
        {
            var request = new CreateDatasetRequest
            {
                Name = "Users-Dataset",
                DatasetGroupArn = serviceConfigurationProvider.GetDatasetGroupArn(siteName),
                SchemaArn = serviceConfigurationProvider.GetUsersSchemaArn(siteName),
                DatasetType = "Users"
            };
            var resp = amazonClient.CreateDatasetAsync(request).Result;
            return resp.DatasetArn;
        }
    }
}
