﻿using System;
using System.Collections.Generic;

using Amazon.PersonalizeEvents;

using CMS.Core;

namespace Kentico.Xperience.AmazonPersonalize
{
    /// <summary>
    /// Provider of site-specific <see cref="IEventClientService"/>.
    /// </summary>
    public class EventClientServiceProvider : IEventClientServiceProvider
    {
        private readonly IServiceConfigurationProvider configurationProvider;
        private readonly IEventTrackerServiceProvider eventTrackerServiceProvider;
        private readonly IEventLogService eventLogService;


        private readonly object initLock = new object();
        private readonly Dictionary<string, IEventClientService> clientServices = new Dictionary<string, IEventClientService>(StringComparer.InvariantCultureIgnoreCase);


        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetClientServiceProvider"/> class.
        /// </summary>
        /// <param name="configurationProvider">Provider of the Amazon Personalize service configuration.</param>
        /// <param name="eventLogService">Event log service.</param>
        public EventClientServiceProvider(IServiceConfigurationProvider configurationProvider, IEventLogService eventLogService, IEventTrackerServiceProvider eventTrackerServiceProvider)
        {
            this.configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
            this.eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
            this.eventTrackerServiceProvider = eventTrackerServiceProvider ?? throw new ArgumentNullException(nameof(eventTrackerServiceProvider));
        }


        /// <summary>
        /// Gets a value indicating whether the Amazon Personalize event client service is available for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to test the availability.</param>
        /// <returns>Returns true if the client service is available for <paramref name="siteName"/>, otherwise returns false.</returns>
        public bool IsAvailable(string siteName)
        {
            return Get(siteName) != null;
        }


        /// <summary>
        /// Gets the Amazon Personalize event client services for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the client service.</param>
        /// <returns>Returns the event client service, or null if client service is not available for the site.</returns>
        public IEventClientService Get(string siteName)
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


                var accessKey = configurationProvider.GetAcessKey(siteName);
                var secretKey = configurationProvider.GetSecretKey(siteName);
                var regionEndpointName = configurationProvider.GetRegionEndpoint(siteName);
                if (String.IsNullOrEmpty(accessKey) || String.IsNullOrEmpty(secretKey) || regionEndpointName == null)
                {
                    clientServices.Add(siteName, null);

                    eventLogService.LogWarning("AmazonPersonalize", "MISSINGCREDENTIALS", $"Live site app settings do not contain Amazon Personalize access key, secret key or endpoint for site '{siteName}'.");

                    return null;
                }

                var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionEndpointName);
                var amazonClient = new AmazonPersonalizeEventsClient(accessKey, secretKey, regionEndpoint);
                
                var eventTrackerService = eventTrackerServiceProvider.Get(siteName);

                var clientService = new EventClientService(amazonClient, eventLogService, eventTrackerService);

                clientServices.Add(siteName, clientService);

                return clientService;
            }
        }
    }
}
