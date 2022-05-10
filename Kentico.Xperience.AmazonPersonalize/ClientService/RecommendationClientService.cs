using System;
using System.Collections.Generic;
using System.Linq;

using Amazon.PersonalizeRuntime;
using Amazon.PersonalizeRuntime.Model;

using CMS.DocumentEngine;

namespace Kentico.Xperience.AmazonPersonalize
{
    public class RecommendationClientService : IRecommendationClientService
    {
        private readonly IServiceConfigurationProvider configurationProvider;
        private readonly AmazonPersonalizeRuntimeClient amazonClient;


        /// <summary>
        /// Initializes a new instance of the <see cref="RecommendationClientService"/> class.
        /// </summary>
        /// <param name="amazonClient">Amazon Personalize recommendation client to be managed.</param>
        public RecommendationClientService(AmazonPersonalizeRuntimeClient amazonClient, IServiceConfigurationProvider configurationProvider)
        {
            this.amazonClient = amazonClient ?? throw new ArgumentNullException(nameof(amazonClient));
            this.configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
        }


        /// <summary>
        /// Sends a <see cref="Request"/>.
        /// </summary>
        /// <param name="request">Request to be sent.</param>
        /// <returns>Returns the request's response.</returns>
        public IEnumerable<PageIdentifier> GetRecommendations(string siteName, Guid contactGuid, int count, string culture, IEnumerable<string> pageTypes = null)
        {
            var filterArn = GetFilterArn(siteName, culture, pageTypes);

            var filterValues = GetFilterValues(culture, pageTypes);

            var recommendationRequest = new GetRecommendationsRequest
            {
                UserId = contactGuid.ToString(),
                NumResults = count,
                CampaignArn = configurationProvider.GetPersonalizedCampaignArn(siteName),
                FilterArn = filterArn,
                FilterValues = filterValues,
            };

            return amazonClient.GetRecommendationsAsync(recommendationRequest).Result.ItemList.Select(r => ParseItemId(r.ItemId));
        }


        public IEnumerable<PageIdentifier> GetPagesRecommendationForPage(TreeNode page, string siteName, int count, string culture, IEnumerable<string> pageTypes = null)
        {
            var filterArn = GetFilterArn(siteName, culture, pageTypes);

            var filterValues = GetFilterValues(culture, pageTypes);

            var recommendationRequest = new GetRecommendationsRequest
            {
                ItemId = GetItemId(page),
                NumResults = count,
                CampaignArn = configurationProvider.GetSimmilarItemsCampaignArn(siteName),
                FilterArn = filterArn,
                FilterValues = filterValues,
            };

            return amazonClient.GetRecommendationsAsync(recommendationRequest).Result.ItemList.Select(r => ParseItemId(r.ItemId));
        }


        Dictionary<string, string> GetFilterValues(string culture, IEnumerable<string> pageTypes)
        {
            var filterValues = new Dictionary<string, string>();

            filterValues.Add("CULTURE", culture);

            if(pageTypes != null)
            {
                filterValues.Add("PAGE_TYPES", String.Join(",", pageTypes));
            }

            return filterValues;
        }


        string GetFilterArn(string siteName, string culture, IEnumerable<string> pageTypes)
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
