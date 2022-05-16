using System;
using System.Collections.Generic;
using System.Linq;

using Amazon.PersonalizeRuntime;
using Amazon.PersonalizeRuntime.Model;

using CMS.Core;
using CMS.DocumentEngine;

namespace Kentico.Xperience.AmazonPersonalize
{
    /// <summary>
    /// Encapsulates retrieving recommendations from Amazon Personalize runtime client.
    /// </summary>
    public class RecommendationClientService : IRecommendationClientService
    {
        private readonly IServiceConfigurationProvider configurationProvider;
        private readonly AmazonPersonalizeRuntimeClient amazonClient;
        private readonly IEventLogService logEventService;


        /// <summary>
        /// Initializes a new instance of the <see cref="RecommendationClientService"/> class.
        /// </summary>
        /// <param name="amazonClient">Amazon Personalize recommendation client to be managed.</param>
        /// <param name="configurationProvider">Configuration </param>
        public RecommendationClientService(AmazonPersonalizeRuntimeClient amazonClient, IServiceConfigurationProvider configurationProvider, IEventLogService logEventService)
        {
            this.amazonClient = amazonClient ?? throw new ArgumentNullException(nameof(amazonClient));
            this.configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
            this.logEventService = logEventService ?? throw new ArgumentNullException(nameof(configurationProvider));
        }


        /// <inheritdoc/>
        public IEnumerable<PageIdentifier> GetRecommendationsForContact(string siteName, Guid contactGuid, int count, string culture, IEnumerable<string> pageTypes = null)
        {
            var campaignArn = configurationProvider.GetPersonalizedCampaignArn(siteName);
            var userId = contactGuid.ToString();

            return GetRecommendation(siteName, count, culture, pageTypes, campaignArn, null, userId);
        }


        /// <inheritdoc/>
        public IEnumerable<PageIdentifier> GetPagesRecommendationForPage(TreeNode page, string siteName, int count, string culture, IEnumerable<string> pageTypes = null)
        {
            var campaignArn = configurationProvider.GetSimmilarItemsCampaignArn(siteName);
            var itemId = GetItemId(page);

            return GetRecommendation(siteName, count, culture, pageTypes, campaignArn, itemId, null);
        }


        private IEnumerable<PageIdentifier> GetRecommendation(string siteName, int count, string culture, IEnumerable<string> pageTypes, string campaignArn, string itemId, string userId)
        {
            var filterArn = GetFilterArn(siteName, pageTypes);

            var filterValues = GetFilterValues(culture, pageTypes);

            var recommendationRequest = new GetRecommendationsRequest
            {
                UserId = userId,
                ItemId = itemId,
                NumResults = count,
                CampaignArn = campaignArn,
                FilterArn = filterArn,
                FilterValues = filterValues,
            };

            try
            {
                return amazonClient.GetRecommendationsAsync(recommendationRequest).Result.ItemList.Select(r => ParseItemId(r.ItemId));
            }
            catch (Exception ex)
            {
                logEventService.LogException("AmazonPersonalize", "GETRECOMMENDATION", ex);
                throw;
            }
        }


        /// <summary>
        /// Gets filter values. 
        /// </summary>
        /// <param name="culture">Culture of filtered recommendations.</param>
        /// <param name="pageTypes">Page types of filtered recommendations.</param>
        /// <returns></returns>
        protected virtual Dictionary<string, string> GetFilterValues(string culture, IEnumerable<string> pageTypes)
        {
            var filterValues = new Dictionary<string, string>();

            filterValues.Add("CULTURE", $"\"{culture.ToLowerInvariant()}\"");

            if(pageTypes != null)
            {
                filterValues.Add("PAGE_TYPES", String.Join(",", pageTypes.Select(pt => $"\"{pt.ToLowerInvariant()}\"")));
            }

            return filterValues;
        }


        /// <summary>
        /// Retieves filter ARN.
        /// </summary>
        /// <param name="siteName">Site name for which to retrieve filter.</param>
        /// <param name="pageTypes">If not null returns filter with page types parameter.</param>
        /// <returns></returns>
        protected virtual string GetFilterArn(string siteName, IEnumerable<string> pageTypes)
        {
            if(pageTypes == null)
            {
                return configurationProvider.GetFilterWithoutPageTypesArn(siteName);
            }

            return configurationProvider.GetFilterWithPageTypesArn(siteName);
        }


        /// <summary>
        /// Gets Amazon Personalize dataset item identifier for <paramref name="page"/>.
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


        /// <summary>
        /// Parses Amazon Personalize dataset item identifier of a page.
        /// </summary>
        /// <param name="id">Identifier to be parsed.</param>
        /// <returns>Returns the page's identifier.</returns>
        /// <remarks>
        /// The method parses identifier in format '&lt;NodeGUID&gt;:&lt;DocumentGUID&gt;'.
        /// </remarks>
        protected virtual PageIdentifier ParseItemId(string id)
        {
            var guids = id.Split(':');

            return new PageIdentifier(Guid.Parse(guids[0]), Guid.Parse(guids[1]));
        }
    }
}
