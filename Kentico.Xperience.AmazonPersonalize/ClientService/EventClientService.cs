using System;
using System.Collections.Generic;
using System.Linq;

using Amazon.PersonalizeEvents;
using Amazon.PersonalizeEvents.Model;

using CMS.Core;
using CMS.DocumentEngine;

namespace Kentico.Xperience.AmazonPersonalize
{
    public class EventClientService : IEventClientService
    {
        private readonly AmazonPersonalizeEventsClient amazonClient;
        private readonly IServiceConfigurationProvider configProvider;
        private readonly IEventLogService eventLogService;
        private readonly string siteName;


        /// <summary>
        /// Initializes a new instance of the <see cref="EventClientService"/> class.
        /// </summary>
        /// <param name="amazonClient">Amazon Personalize client to be managed.</param>
        public EventClientService(AmazonPersonalizeEventsClient amazonClient, string siteName, IServiceConfigurationProvider configProvider, IEventLogService eventLogService)
        {
            this.amazonClient = amazonClient ?? throw new ArgumentNullException(nameof(amazonClient));
            this.siteName = siteName;
            this.configProvider = configProvider;
            this.eventLogService = eventLogService;
        }


        public void LogPageView(TreeNode page, Guid contactGuid, string sessionId = null, IEnumerable<TreeNode> impression = null)
        {
            LogEvent("page-view", page, contactGuid, sessionId, impression);
        }


        public void LogConversion(TreeNode page, Guid contactGuid, string sessionId, IEnumerable<TreeNode> impression = null)
        {
            LogEvent("conversion", page, contactGuid, sessionId, impression);
        }


        private void LogEvent(string eventType, TreeNode page, Guid contactGuid, string sessionId = null, IEnumerable<TreeNode> impression = null)
        {
            var impressionIDs = Enumerable.Empty<string>();
            if (impression != null)
            {
                impressionIDs = impression.Select(i => GetItemId(page));
            }
            var request = new PutEventsRequest
            {
                TrackingId = configProvider.GetEventTrackerArn(siteName),
                SessionId = sessionId ?? Guid.NewGuid().ToString(),
                UserId = contactGuid.ToString(),
                EventList = new List<Event>
                {
                    new Event
                    {
                        SentAt = DateTime.UtcNow,
                        Impression = impressionIDs.ToList(),
                        EventType = eventType,
                        ItemId = GetItemId(page),
                    }
                }
            };

            try
            {
                eventLogService.LogInformation("AmazonPersonalize", $"Log {eventType}", $"Logging {eventType} event for page {page.DocumentName}.");
                amazonClient.PutEventsAsync(request).Wait();
            }
            catch (Exception ex)
            {
                eventLogService.LogException("AmazonPersonalize", $"Log {eventType} event", ex);

                throw;
            }
        }


        protected virtual string GetItemId(TreeNode page)
        {
            return $"{page.NodeGUID}:{page.DocumentGUID}";
        }
    }
}
