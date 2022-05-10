using System;
using System.Collections.Generic;

using CMS.DocumentEngine;

namespace Kentico.Xperience.AmazonPersonalize
{
    public interface IEventClientService
    {
        void LogPageView(TreeNode page, Guid contactGuid, string sessionId = null, IEnumerable<TreeNode> impression = null);
    }
}