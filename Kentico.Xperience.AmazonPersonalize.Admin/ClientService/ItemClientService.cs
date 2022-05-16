using System;
using System.Collections.Generic;

using Amazon.PersonalizeEvents;
using Amazon.PersonalizeEvents.Model;

using CMS.Core;
using CMS.DocumentEngine;

using Newtonsoft.Json;

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    /// <summary>
    /// Manages pages in the corresponding site's Amazon Personalize dataset.
    /// </summary>
    public class ItemClientService : IItemClientService
    {
        private readonly AmazonPersonalizeEventsClient amazonClient;
        private readonly IServiceConfigurationProvider configProvider;
        private readonly IFieldMapper fieldMapper;
        private readonly IEventLogService eventLogService;
        private readonly string siteName;


        /// <summary>
        /// Initializes a new instance of the <see cref="ItemClientService"/> class.
        /// </summary>
        /// <param name="amazonClient">Amazon Personalize client to be managed.</param>
        /// <param name="siteName">Name of site the Amazon Personalize client belongs to.</param>
        /// <param name="configProvider">Provider of the Amazon Personalize services configuration.</param>
        /// <param name="fieldMapper">Mapper for page fields.</param>
        /// <param name="eventLogService">Event log service.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ItemClientService(AmazonPersonalizeEventsClient amazonClient, string siteName, IServiceConfigurationProvider configProvider, IFieldMapper fieldMapper, IEventLogService eventLogService)
        {
            this.amazonClient = amazonClient ?? throw new ArgumentNullException(nameof(amazonClient));
            this.siteName = siteName;
            this.configProvider = configProvider;
            this.fieldMapper = fieldMapper;
            this.eventLogService = eventLogService;
        }


        /// <inheritdoc/>
        public void PutItem(TreeNode page)
        {
            var itemValues = fieldMapper.Map(page);
            
            ProcessItem(page, itemValues);
        }


        /// <inheritdoc/>
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
                amazonClient.PutItemsAsync(request).Wait();
            }
            catch (Exception ex)
            {
                eventLogService.LogException("AmazonPersonalize", "PutItem", ex);

                throw;
            }
        }


        /// <summary>
        /// Gets Amazon PErsonalize dataset item identifier for <paramref name="page"/>.
        /// </summary>
        /// <param name="page">Page for which to return an identifier.</param>
        /// <returns>Returns the page's identifier.</returns>
        /// <remarks>
        /// The method returns identifier in format '&lt;NodeGUID&gt;:&lt;DocumentGUID&gt;'.
        /// </remarks>
        protected virtual string GetItemId(TreeNode page)
        {
            return $"{page.NodeGUID}:{page.DocumentGUID}";
        }
    }
}
