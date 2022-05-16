using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Core;
using CMS.DocumentEngine;

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    /// <summary>
    /// Manages pages in the corresponding site's Amazon Personalize Amazon Personalize.
    /// </summary>
    public class AmazonPersonalizeService : IAmazonPersonalizeService
    {
        private readonly IItemClientServiceProvider itemClientServiceProvider;
        private readonly IFieldMapper fieldMapper;
        private readonly IEventLogService eventLogService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmazonPersonalizeService"/> class.
        /// </summary>
        /// <param name="itemClientServiceProvider">Provider of <see cref="ItemClientService"/>s.</param>
        /// <param name="fieldMapper">Mapper for page fields.</param>
        /// <param name="eventLogService">Event log service.</param>
        public AmazonPersonalizeService(IFieldMapper fieldMapper, IEventLogService eventLogService, IItemClientServiceProvider itemClientServiceProvider)
        {
            this.fieldMapper = fieldMapper ?? throw new ArgumentNullException(nameof(fieldMapper));
            this.eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
            this.itemClientServiceProvider = itemClientServiceProvider ?? throw new ArgumentNullException(nameof(itemClientServiceProvider));
        }


        /// <inheritdoc/>
        public bool IsProcessed(TreeNode page)
        {
            var configuration = fieldMapper.GetConfigurations(page.NodeSiteName);

            return configuration.IncludedCultures.Contains(page.DocumentCulture) && configuration.Mappings.ContainsKey(page.TypeInfo.ObjectType);
        }


        /// <summary>
        /// Gets a value indicating whether <paramref name="page"/> in its current state is to be published to the Amazon Personalize dataset.
        /// Non-published pages are deleted from the Amazon Personalize dataset.
        /// </summary>
        /// <param name="page">Page for which to return the publishing status.</param>
        /// <returns>Returns true if the page should be sent to the Amazon Personalize dataset. Otherwise returns false.</returns>
        /// <remarks>
        /// The implementation decides the publishing state based on the <see cref="TreeNode.IsPublished"/> property.
        /// </remarks>
        protected virtual bool IsPublished(TreeNode page)
        {
            return page.IsPublished;
        }


        /// <inheritdoc/>
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


        /// <inheritdoc/>
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
                SendPageType(pageType, siteName, pageTypesMappings.IncludedCultures);
            }
        }


        /// <inheritdoc/>
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


        /// <summary>
        /// Sends all pages of <paramref name="pageType"/> on the specified site to the Amazon Personalize dataset.
        /// </summary>
        /// <param name="pageType">Page type of pages to be sent to the dataset.</param>
        /// <param name="siteName">Name of site whose pages are to be sent.</param>
        /// <param name="includedCultures">Culture of pages to be sent. All cultures are sent if the set is empty.</param>
        /// <remarks>
        /// <para>
        /// Only published versions of pages in included cultures are sent to the Amazon Personalize dataset.
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
        /// Only published versions of pages in included cultures are sent to the Amazon Personalize dataset.
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


        /// <inheritdoc/>
        public virtual void PageCreated(TreeNode page)
        {
            if (!IsProcessed(page) || !IsPublished(page))
            {
                return;
            }

            Send(page);
        }


        /// <inheritdoc/>
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


        /// <inheritdoc/>
        public virtual void PageDeleted(TreeNode page)
        {
            if (!IsProcessed(page))
            {
                return;
            }

            Delete(page);
        }
    }
}
