using System;

using Amazon.Personalize;
using Amazon.Personalize.Model;

using CMS.Core;

namespace Kentico.Xperience.AmazonPersonalize
{
    /// <summary>
    /// Encapsulates event tracker for Amazon Personalize.
    /// </summary>
    public class EventTrackerService : IEventTrackerService
    {
        private readonly AmazonPersonalizeClient amazonClient;
        private readonly IServiceConfigurationProvider configProvider;
        private readonly IEventLogService eventLogService;
        private readonly string siteName;
        private string trackingId;
        private static readonly object padlock = new object();


        /// <inheritdoc/>
        public string TrackingId
        {
            get
            {
                lock (padlock)
                {
                    if (string.IsNullOrEmpty(trackingId))
                    {
                        try
                        {
                            trackingId = GetTrackingId();
                        }
                        catch (Exception ex)
                        {
                            eventLogService.LogException("AmazonPersonalize", "Create event tracker", ex);
                            throw;
                        }
                    }
                    return trackingId;
                }
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EventTrackerService"/> class.
        /// </summary>
        /// <param name="amazonClient">Amazon Personalize client for managing event tracker.</param>
        public EventTrackerService(AmazonPersonalizeClient amazonClient, string siteName, IServiceConfigurationProvider configProvider, IEventLogService eventLogService)
        {
            this.amazonClient = amazonClient ?? throw new ArgumentNullException(nameof(amazonClient));
            this.siteName = siteName ?? throw new ArgumentNullException(nameof(siteName));
            this.configProvider = configProvider ?? throw new ArgumentNullException(nameof(configProvider));
            this.eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
        }


        private string GetTrackingId()
        {
            var describeRequest = new DescribeEventTrackerRequest
            {
                EventTrackerArn = configProvider.GetEventTrackerArn(siteName),
            };

            var describeResult = amazonClient.DescribeEventTrackerAsync(describeRequest).Result;
            
            return describeResult.EventTracker.TrackingId;

        }

    }
}
