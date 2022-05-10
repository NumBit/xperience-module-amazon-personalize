using System;
using System.Collections.Generic;
using Amazon.Personalize;
using Amazon.PersonalizeEvents;
using Amazon.Runtime;
using CMS.Core;

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    /// <summary>
    /// Provider of site-specific <see cref="IDatasetClientService"/>.
    /// </summary>
    public class ItemClientServiceProvider : IItemClientServiceProvider
    {
        private readonly IServiceConfigurationProvider serviceConfigurationProvider;
        private readonly IEventLogService eventLogService;
        private readonly IServiceConfigurationProvider configProvider;
        private readonly IFieldMapper fieldMapper;


        private readonly object initLock = new object();
        private readonly Dictionary<string, IItemClientService> clientServices = new Dictionary<string, IItemClientService>(StringComparer.InvariantCultureIgnoreCase);


        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetClientServiceProvider"/> class.
        /// </summary>
        /// <param name="serviceConfigurationProvider">Provider of the Amazon Personalize item service configuration.</param>
        /// <param name="eventLogService">Event log service.</param>
        public ItemClientServiceProvider(IServiceConfigurationProvider serviceConfigurationProvider, IEventLogService eventLogService, IServiceConfigurationProvider configProvider, IFieldMapper fieldMapper)
        {
            this.serviceConfigurationProvider = serviceConfigurationProvider ?? throw new ArgumentNullException(nameof(serviceConfigurationProvider));
            this.eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
            this.configProvider = configProvider;
            this.fieldMapper = fieldMapper;
        }


        /// <summary>
        /// Gets a value indicating whether the Amazon Personalize item client service is available for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to test the availability.</param>
        /// <returns>Returns true if the client service is available for <paramref name="siteName"/>, otherwise returns false.</returns>
        public bool IsAvailable(string siteName)
        {
            return Get(siteName) != null;
        }


        /// <summary>
        /// Gets the Amazon Personalize item client services for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the client service.</param>
        /// <returns>Returns the client service, or null if client service is not available for the site.</returns>
        public IItemClientService Get(string siteName)
        {
            if (clientServices.TryGetValue(siteName, out var cs))
            {
                return cs;
            }

            lock (initLock)
            {
                if (clientServices.TryGetValue(siteName, out var cs2))
                {
                    return cs2;
                }


                var accessKey = serviceConfigurationProvider.GetAcessKey(siteName);
                var secretKey = serviceConfigurationProvider.GetSecretKey(siteName);
                if (String.IsNullOrEmpty(accessKey) || String.IsNullOrEmpty(secretKey))
                {
                    clientServices.Add(siteName, null);

                    eventLogService.LogWarning("AmazonPersonalize", "MISSINGCREDENTIALS", $"Live site app settings do not contain Amazon Personalize access key or secret key for site '{siteName}'.");

                    return null;
                }
                
                var amazonClient = new AmazonPersonalizeEventsClient(accessKey, secretKey, Amazon.RegionEndpoint.EUCentral1);

                var clientService = new ItemClientService(amazonClient, siteName, configProvider, fieldMapper, eventLogService);

                clientServices.Add(siteName, clientService);

                return clientService;
            }
        }
    }
}
