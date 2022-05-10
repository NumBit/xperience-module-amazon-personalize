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


        /// <summary>
        /// Gets a value indicating whether the Amazon Personalize recommendation client service is available for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to test the availability.</param>
        /// <returns>Returns true if the client service is available for <paramref name="siteName"/>, otherwise returns false.</returns>
        public bool IsAvailable(string siteName)
        {
            return Get(siteName) != null;
        }


        /// <summary>
        /// Gets the Amazon Personalize recommendation client services for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the client service.</param>
        /// <returns>Returns the client service, or null if client service is not available for the site.</returns>
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
                if (String.IsNullOrEmpty(accessKey) || String.IsNullOrEmpty(secretKey))
                {
                    clientServices.Add(siteName, null);

                    eventLogService.LogWarning("AmazonPersonalize", "MISSINGCREDENTIALS", $"Live site app settings do not contain Amazon Personalize access key or secret key for site '{siteName}'.");

                    return null;
                }

                var amazonClient = new AmazonPersonalizeRuntimeClient(accessKey, secretKey, Amazon.RegionEndpoint.EUCentral1);

                var clientService = new RecommendationClientService(amazonClient, configurationProvider);

                clientServices.Add(siteName, clientService);

                return clientService;
            }
        }
    }
}
