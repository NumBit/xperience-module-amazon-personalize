
using System;
using System.Collections.Generic;

using CMS.DocumentEngine;

namespace Kentico.Xperience.AmazonPersonalize
{
    public interface IRecommendationClientService
    {
        IEnumerable<PageIdentifier> GetRecommendations(string siteName, Guid contactGuid, int count, string culture, IEnumerable<string> pageTypes = null);

        IEnumerable<PageIdentifier> GetPagesRecommendationForPage(TreeNode page, string siteName, int count, string culture, IEnumerable<string> pageTypes = null);
    }
}