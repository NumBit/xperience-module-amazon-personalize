using System;
using System.Collections.Generic;

using Amazon.PersonalizeEvents;

using CMS.Core;

namespace Kentico.Xperience.AmazonPersonalize.Admin
{

    /// <summary>
    /// Provider of site-specific <see cref="IItemClientServiceProvider"/>.
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
        /// Initializes a new instance of the <see cref="ItemClientServiceProvider"/> class.
        /// </summary>
        /// <param name="serviceConfigurationProvider">Provider of the Amazon Personalize item service configuration.</param>
        /// <param name="eventLogService">Event log service.</param>
        public ItemClientServiceProvider(IServiceConfigurationProvider serviceConfigurationProvider, IEventLogService eventLogService, IServiceConfigurationProvider configProvider, IFieldMapper fieldMapper)
        {
            this.serviceConfigurationProvider = serviceConfigurationProvider ?? throw new ArgumentNullException(nameof(serviceConfigurationProvider));
            this.eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
            this.configProvider = configProvider ?? throw new ArgumentNullException(nameof(configProvider));
            this.fieldMapper = fieldMapper ?? throw new ArgumentNullException(nameof(fieldMapper));
        }


        /// <inheritdoc/>
        public bool IsAvailable(string siteName)
        {
            return Get(siteName) != null;
        }


        /// <inheritdoc/>
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
                var regionEndpointName = configProvider.GetRegionEndpoint(siteName);
                if (String.IsNullOrEmpty(accessKey) || String.IsNullOrEmpty(secretKey) || regionEndpointName == null)

                {
                    clientServices.Add(siteName, null);

                    eventLogService.LogWarning("AmazonPersonalize", "MISSINGCREDENTIALS", $"App settings do not contain Amazon Personalize access key, secret key or endpoint region for site '{siteName}'.");

                    return null;
                }

                var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionEndpointName);
                var amazonClient = new AmazonPersonalizeEventsClient(accessKey, secretKey, regionEndpoint);

                var clientService = new ItemClientService(amazonClient, siteName, configProvider, fieldMapper, eventLogService);

                clientServices.Add(siteName, clientService);

                return clientService;
            }
        }
    }
}
