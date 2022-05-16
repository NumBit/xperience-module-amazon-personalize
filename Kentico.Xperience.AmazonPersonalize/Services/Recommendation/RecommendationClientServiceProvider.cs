using System;
using System.Collections.Generic;

using Amazon.PersonalizeRuntime;

using CMS.Core;

namespace Kentico.Xperience.AmazonPersonalize
{
    /// <summary>
    /// Provider of site-specific <see cref="IRecommendationClientService"/>.
    /// </summary>
    public class RecommendationClientServiceProvider : IRecommendationClientServiceProvider
    {
        private readonly IServiceConfigurationProvider configurationProvider;
        private readonly IEventLogService eventLogService;

        private readonly object initLock = new object();
        private readonly Dictionary<string, IRecommendationClientService> clientServices = new Dictionary<string, IRecommendationClientService>(StringComparer.InvariantCultureIgnoreCase);


        /// <summary>
        /// Initializes a new instance of the <see cref="RecommendationClientServiceProvider"/> class.
        /// </summary>
        /// <param name="configurationProvider">Provider of the Amazon Personalize service configuration.</param>
        /// <param name="eventLogService">Event log service.</param>
        public RecommendationClientServiceProvider(IServiceConfigurationProvider configurationProvider, IEventLogService eventLogService)
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
        public IRecommendationClientService Get(string siteName)
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

                    eventLogService.LogWarning("AmazonPersonalize", "MISSINGCREDENTIALS", $"Live site app settings do not contain Amazon Personalize access key, secret key or endpoint name for site '{siteName}'.");

                    return null;
                }

                var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionEndpointName);
                var amazonClient = new AmazonPersonalizeRuntimeClient(accessKey, secretKey, regionEndpoint);

                var clientService = new RecommendationClientService(amazonClient, configurationProvider, eventLogService);

                clientServices.Add(siteName, clientService);

                return clientService;
            }
        }
    }
}
