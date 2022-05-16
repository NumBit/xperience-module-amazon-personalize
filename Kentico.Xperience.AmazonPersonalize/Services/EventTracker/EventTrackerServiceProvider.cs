using System;
using System.Collections.Generic;

using Amazon.Personalize;

using CMS.Core;

namespace Kentico.Xperience.AmazonPersonalize
{
    /// <summary>
    /// Provider of site-specific <see cref="IEventTrackerService"/>.
    /// </summary>
    public class EventTrackerServiceProvider : IEventTrackerServiceProvider
    {
        private readonly IServiceConfigurationProvider configurationProvider;
        private readonly IEventLogService eventLogService;

        private readonly object initLock = new object();
        private readonly Dictionary<string, IEventTrackerService> eventTrackerServices = new Dictionary<string, IEventTrackerService>(StringComparer.InvariantCultureIgnoreCase);


        /// <summary>
        /// Initializes a new instance of the <see cref="EventTrackerServiceProvider"/> class.
        /// </summary>
        /// <param name="configurationProvider">Provider of the Amazon Personalize service configuration.</param>
        /// <param name="eventLogService">Event log service.</param>
        public EventTrackerServiceProvider(IServiceConfigurationProvider configurationProvider, IEventLogService eventLogService)
        {
            this.configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
            this.eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
        }


        /// <inheritdoc/>
        public bool IsAvailable(string siteName)
        {
            return Get(siteName) != null;
        }


        /// <inheritdoc/>
        public IEventTrackerService Get(string siteName)
        {
            if (eventTrackerServices.TryGetValue(siteName, out var cs))
            {
                return cs;
            }

            lock (initLock)
            {
                if (eventTrackerServices.TryGetValue(siteName, out var cs2))
                {
                    return cs2;
                }

                var accessKey = configurationProvider.GetAcessKey(siteName);
                var secretKey = configurationProvider.GetSecretKey(siteName);
                var regionEndpointName = configurationProvider.GetRegionEndpoint(siteName);
                if (String.IsNullOrEmpty(accessKey) || String.IsNullOrEmpty(secretKey) || regionEndpointName == null)
                {
                    eventTrackerServices.Add(siteName, null);

                    eventLogService.LogWarning("AmazonPersonalize", "MISSINGCREDENTIALS", $"Live site app settings do not contain Amazon Personalize access key, secret key or region endpoint for site '{siteName}'.");

                    return null;
                }

                var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionEndpointName);
                var amazonClient = new AmazonPersonalizeClient(accessKey, secretKey, regionEndpoint);

                var clientService = new EventTrackerService(amazonClient, siteName, configurationProvider, eventLogService);

                eventTrackerServices.Add(siteName, clientService);

                return clientService;
            }
        }
    }
}
