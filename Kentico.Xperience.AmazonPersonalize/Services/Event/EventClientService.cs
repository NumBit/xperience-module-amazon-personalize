using System;
using System.Collections.Generic;
using System.Linq;

using Amazon.PersonalizeEvents;
using Amazon.PersonalizeEvents.Model;

using CMS.Core;
using CMS.DocumentEngine;

namespace Kentico.Xperience.AmazonPersonalize
{
    /// <summary>
    /// Encapsulates logging events for Amazon Personalize.
    /// </summary>
    public class EventClientService : IEventClientService
    {
        private readonly AmazonPersonalizeEventsClient amazonClient;
        private readonly IEventTrackerService eventTrackerService;
        private readonly IEventLogService eventLogService;


        /// <summary>
        /// Initializes a new instance of the <see cref="EventClientService"/> class.
        /// </summary>
        /// <param name="amazonClient">Amazon Personalize client to be managed.</param>
        public EventClientService(AmazonPersonalizeEventsClient amazonClient, IEventLogService eventLogService, IEventTrackerService eventTrackerService)
        {
            this.amazonClient = amazonClient ?? throw new ArgumentNullException(nameof(amazonClient));
            this.eventLogService = eventLogService;
            this.eventTrackerService = eventTrackerService;
        }


        /// <inheritdoc/>
        public void LogPageView(TreeNode page, Guid contactGuid, string sessionId = null, IEnumerable<TreeNode> impression = null)
        {
            LogEvent("page-view", page, contactGuid, sessionId, impression);
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
                TrackingId = eventTrackerService.TrackingId,
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
                amazonClient.PutEventsAsync(request).Wait();
            }
            catch (Exception ex)
            {
                eventLogService.LogException("AmazonPersonalize", $"LOGEVENT", ex);
                throw;
            }
        }


        /// <summary>
        /// Gets Amazon Personalizedataset item identifier for <paramref name="page"/>.
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
