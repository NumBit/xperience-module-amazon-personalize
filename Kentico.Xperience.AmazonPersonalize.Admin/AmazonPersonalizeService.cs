using System.Collections.Generic;
using System.Linq;

using CMS.Core;
using CMS.DocumentEngine;

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    public class AmazonPersonalizeService : IAmazonPersonalizeService
    {
        private readonly IDatasetClientServiceProvider datasetClientServiceProvider;
        private readonly IItemClientServiceProvider itemClientServiceProvider;
        private readonly IFieldMapper fieldMapper;
        private readonly IEventLogService eventLogService;

        public AmazonPersonalizeService(IDatasetClientServiceProvider clientServiceProvider, IFieldMapper fieldMapper, IEventLogService eventLogService, IItemClientServiceProvider itemClientServiceProvider)
        {
            this.datasetClientServiceProvider = clientServiceProvider;
            this.fieldMapper = fieldMapper;
            this.eventLogService = eventLogService;
            this.itemClientServiceProvider = itemClientServiceProvider;
        }


        public void Init(string siteName)
        {
            LogNoClientWarning(siteName);
            if (!datasetClientServiceProvider.IsAvailable(siteName))
            {
                LogNoClientWarning(siteName);
                return;
            }

            var datasetService = datasetClientServiceProvider.Get(siteName);

            datasetService.Init();
        }


        public bool IsProcessed(TreeNode page)
        {
            var configuration = fieldMapper.GetConfigurations(page.NodeSiteName);

            return configuration.IncludedCultures.Contains(page.DocumentCulture) && configuration.Mappings.ContainsKey(page.TypeInfo.ObjectType);
        }


        protected virtual bool IsPublished(TreeNode page)
        {
            return page.IsPublished;
        }


        public void Reset(string siteName)
        {
            if (!datasetClientServiceProvider.IsAvailable(siteName))
            {
                LogNoClientWarning(siteName);
                return;
            }

            datasetClientServiceProvider.Get(siteName).Reset();
        }

        public void Send(TreeNode page)
        {
            var siteName = page.NodeSiteName;
            if (!itemClientServiceProvider.IsAvailable(siteName))
            {
                LogNoClientWarning(siteName);
                return;
            }

            itemClientServiceProvider.Get(siteName).PutItem(page);
        }


        public void SendAll(string siteName)
        {
            if (!itemClientServiceProvider.IsAvailable(siteName))
            {
                LogNoClientWarning(siteName);
                return;
            }

            var pageTypesMappings = fieldMapper.GetConfigurations(siteName);
            foreach (var pageType in pageTypesMappings.Mappings.Keys)
            {
                eventLogService.LogInformation("AmazonPersonalize", "Sending page type", $"Sending pages of page type {pageType}");
                SendPageType(pageType, siteName, pageTypesMappings.IncludedCultures);
            }
        }


        /// <summary>
        /// Sends all pages of <paramref name="pageType"/> on the specified site to the Amazon Personalize database.
        /// </summary>
        /// <param name="pageType">Page type of pages to be sent to the database.</param>
        /// <param name="siteName">Name of site whose pages are to be sent.</param>
        /// <param name="includedCultures">Culture of pages to be sent. All cultures are sent if the set is empty.</param>
        /// <remarks>
        /// <para>
        /// Only published versions of pages in included cultures are sent to the Amazon Personalize database.
        /// </para>
        /// <para>
        /// When overriding, the method's implementation must be consistent with the <see cref="IsProcessed(TreeNode)"/> and <see cref="IsPublished(TreeNode)"/> methods.
        /// </para>
        /// </remarks>
        protected virtual void SendPageType(string pageType, string siteName, ISet<string> includedCultures)
        {
            var pages = GetPages(pageType, siteName, includedCultures);

            var itemService = itemClientServiceProvider.Get(siteName);

            foreach (var page in pages)
            {
                itemService.PutItem(page);
            }
        }


        /// <summary>
        /// Gets all pages of <paramref name="pageType"/>  on the specified site.
        /// </summary>
        /// <param name="pageType">Page type of pages to be retrieved.</param>
        /// <param name="siteName">Name of site whose pages are to be retrieved.</param>
        /// <param name="includedCultures">Culture of pages to be retrieved. All cultures are retrieved if the set is empty.</param>
        /// <returns>Returns an enumeration of pages.</returns>
        /// <remarks>
        /// <para>
        /// Only published versions of pages in included cultures are sent to the Amazon Personalize database.
        /// </para>
        /// <para>
        /// When overriding, the method's implementation must be consistent with the <see cref="IsProcessed(TreeNode)"/> and <see cref="IsPublished(TreeNode)"/> methods.
        /// </para>
        /// </remarks>
        protected virtual IEnumerable<TreeNode> GetPages(string pageType, string siteName, ISet<string> includedCultures)
        {
            var query = DocumentHelper.GetDocuments(pageType).OnSite(siteName).PublishedVersion();

            if (includedCultures.Count == 0)
            {
                return query.AllCultures();
            }

            return includedCultures.SelectMany(culture => query.Clone().CombineWithDefaultCulture(false).Culture(culture));
        }


        private void LogNoClientWarning(string siteName)
        {
            eventLogService.LogWarning("AmazonPersonalize", "NOCLIENT", $"Amazon Personalie client service is not available for site '{siteName}'. The operation was cancelled.");
        }


        public virtual void PageCreated(TreeNode page)
        {
            if (!IsProcessed(page) || !IsPublished(page))
            {
                return;
            }

            Send(page);
        }


        /// <summary>
        /// Handles the event of page update. If the <paramref name="page"/> matches the <see cref="IsProcessed(TreeNode)"/> predicate,
        /// it is either sent to the Amazon Personalize DB (if <see cref="IsPublished(TreeNode)"/> is true), or deleted from the Amazon Personalize DB.
        /// </summary>
        /// <param name="page">Page which was updated.</param>
        /// <seealso cref="DocumentEvents.Update"/>
        public virtual void PageUpdated(TreeNode page)
        {
            if (!IsProcessed(page))
            {
                return;
            }

            if (!IsPublished(page))
            {
                Delete(page);
            }
            else
            {
                Send(page);
            }
        }


        public virtual void PageDeleted(TreeNode page)
        {
            if (!IsProcessed(page))
            {
                return;
            }

            Delete(page);
        }


        public void Delete(TreeNode page)
        {
            var siteName = page.NodeSiteName;
            if (!itemClientServiceProvider.IsAvailable(siteName))
            {
                LogNoClientWarning(siteName);
                return;
            }

            itemClientServiceProvider.Get(siteName).DeleteItem(page);
        }
    }
}
