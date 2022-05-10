using System;
using System.Collections.Generic;

using Amazon.PersonalizeEvents;
using Amazon.PersonalizeEvents.Model;

using CMS.Core;
using CMS.DocumentEngine;

using Newtonsoft.Json;

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    public class ItemClientService : IItemClientService
    {
        private readonly AmazonPersonalizeEventsClient amazonClient;
        private readonly IServiceConfigurationProvider configProvider;
        private readonly IFieldMapper fieldMapper;
        private readonly IEventLogService eventLogService;
        private readonly string siteName;


        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetClientService"/> class.
        /// </summary>
        /// <param name="amazonClient">Amazon Personalize client to be managed.</param>
        public ItemClientService(AmazonPersonalizeEventsClient amazonClient, string siteName, IServiceConfigurationProvider configProvider, IFieldMapper fieldMapper, IEventLogService eventLogService)
        {
            this.amazonClient = amazonClient ?? throw new ArgumentNullException(nameof(amazonClient));
            this.siteName = siteName;
            this.configProvider = configProvider;
            this.fieldMapper = fieldMapper;
            this.eventLogService = eventLogService;
        }


        public void PutItem(TreeNode page)
        {
            var itemValues = fieldMapper.Map(page);
            
            ProcessItem(page, itemValues);
        }


        public void DeleteItem(TreeNode page)
        {
            var itemValues = fieldMapper.Map(page);
            
            itemValues["Deleted"] = "true";
            
            ProcessItem(page, itemValues);
        }

        private void ProcessItem(TreeNode page, Dictionary<string, string> itemValues)
        {
            var properties = JsonConvert.SerializeObject(itemValues);
            var item = new Item
            {
                ItemId = GetItemId(page),
                Properties = properties
            };

            var request = new PutItemsRequest
            {
                DatasetArn = configProvider.GetItemsDatasetArn(siteName),
                Items = new List<Item> { item }
            };

            try
            {
                eventLogService.LogInformation("AmazonPersonalize", "Processing item", $"Processing page {page.DocumentName}");
                amazonClient.PutItemsAsync(request).Wait();
            }
            catch (Exception ex)
            {
                eventLogService.LogException("AmazonPersonalize", "PutItem", ex);

                throw;
            }
        }

        protected virtual string GetItemId(TreeNode page)
        {
            return $"{page.NodeGUID}:{page.DocumentGUID}";
        }
    }
}
