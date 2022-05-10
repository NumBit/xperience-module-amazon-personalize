using System;
using System.Collections.Generic;

using CMS.DocumentEngine;

namespace Kentico.Xperience.AmazonPersonalize
{
    /// <inheritdoc/>
    public class AmazonPersonalizeService : IAmazonPersonalizeService
    {
        private readonly IRecommendationClientServiceProvider recommendationClientServiceProvider;
        private readonly IEventClientServiceProvider eventClientServiceProvider;


        /// <summary>
        /// Initializes a new instance of the <see cref="AmazonPersonalizeService"/> class.
        /// </summary>
        /// <param name="recommendationClientServiceProvider">Provider of <see cref="IRecommendationClientService"/>s.</param>
        /// <param name="eventClientServiceProvider">Provider of <see cref="IEventClientService"/>s.</param>
        public AmazonPersonalizeService(IRecommendationClientServiceProvider recommendationClientServiceProvider, IEventClientServiceProvider eventClientServiceProvider)
        {
            this.recommendationClientServiceProvider = recommendationClientServiceProvider ?? throw new ArgumentNullException(nameof(recommendationClientServiceProvider));
            this.eventClientServiceProvider = eventClientServiceProvider;
        }


        /// <inheritdoc/>
        public IEnumerable<PageIdentifier> GetPagesRecommendationForContact(string siteName, Guid contactGuid, int count, string culture, IEnumerable<string> pageTypes = null)
        {
            var client = GetClientServiceOrThrow(siteName);

            
            var recommendation = client.GetRecommendations(siteName, contactGuid, count, culture, pageTypes);

            return recommendation;
        }


        /// <inheritdoc/>
        public IEnumerable<PageIdentifier> GetPagesRecommendationForPage(TreeNode page, string siteName, int count, string culture, IEnumerable<string> pageTypes = null)
        {
            var client = GetClientServiceOrThrow(siteName);


            var recommendation = client.GetPagesRecommendationForPage(page, siteName, count, culture, pageTypes);

            return recommendation;
        }


        /// <inheritdoc/>
        public void LogPageView(TreeNode page, Guid contactGuid, string sessionId, List<TreeNode> impression = null)
        {
            var client = GetEventClientServiceOrThrow(page.NodeSiteName);

            client.LogPageView(page, contactGuid, sessionId, impression);
        }


        private IRecommendationClientService GetClientServiceOrThrow(string siteName)
        {
            return recommendationClientServiceProvider.Get(siteName) ?? throw new InvalidOperationException($"The Amazon Personalize recommendation client service is not available on site '{siteName}'. Specify the Amazon acess key and secret key in the application configuration file.");
        }


        private IEventClientService GetEventClientServiceOrThrow(string siteName)
        {
            return eventClientServiceProvider.Get(siteName) ?? throw new InvalidOperationException($"The Amazon Personalize event client service is not available on site '{siteName}'. Specify the Amazon acess key and secret key in the application configuration file.");
        }
    }
}
