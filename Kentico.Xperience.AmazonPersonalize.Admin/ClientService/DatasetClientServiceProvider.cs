using System;
using System.Collections.Generic;

using Amazon.Personalize;

using CMS.Core;

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    /// <summary>
    /// Provider of site-specific <see cref="IDatasetClientService"/>.
    /// </summary>
    public class DatasetClientServiceProvider : IDatasetClientServiceProvider
    {
        private readonly IServiceConfigurationProvider serviceConfigurationProvider;
        private readonly IEventLogService eventLogService;
        private readonly IDatasetManager databaseManager;


        private readonly object initLock = new object();
        private readonly Dictionary<string, IDatasetClientService> clientServices = new Dictionary<string, IDatasetClientService>(StringComparer.InvariantCultureIgnoreCase);


        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetClientServiceProvider"/> class.
        /// </summary>
        /// <param name="serviceConfigurationProvider">Provider of the Amazon Personalize service configuration.</param>
        /// <param name="eventLogService">Event log service.</param>
        public DatasetClientServiceProvider(IServiceConfigurationProvider serviceConfigurationProvider, IEventLogService eventLogService, IDatasetManager databaseManager)
        {
            this.serviceConfigurationProvider = serviceConfigurationProvider ?? throw new ArgumentNullException(nameof(serviceConfigurationProvider));
            this.eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
            this.databaseManager = databaseManager;
        }


        /// <summary>
        /// Gets a value indicating whether the Amazon Personalize client service is available for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to test the availability.</param>
        /// <returns>Returns true if the client service is available for <paramref name="siteName"/>, otherwise returns false.</returns>
        public bool IsAvailable(string siteName)
        {
            return Get(siteName) != null;
        }


        /// <summary>
        /// Gets the Amazon Personalize client services for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the client service.</param>
        /// <returns>Returns the client service, or null if client service is not available for the site.</returns>
        public IDatasetClientService Get(string siteName)
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

                var amazonClient = new AmazonPersonalizeClient(accessKey, secretKey, Amazon.RegionEndpoint.EUCentral1);

                var clientService = new DatasetClientService(amazonClient, databaseManager, siteName);

                clientServices.Add(siteName, clientService);

                return clientService;
            }
        }
    }
}
