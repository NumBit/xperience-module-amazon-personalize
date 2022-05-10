using System;
using System.Linq;
using CMS.ContactManagement;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatCore;
using DancingGoat.Models;
using DancingGoat.Widgets;

using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.AmazonPersonalize;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

[assembly: RegisterWidget(RecWidgetViewComponent.IDENTIFIER, typeof(RecWidgetViewComponent), "Rec", typeof(RecWidgetProperties), Description = "Displays the latest articles from the Dancing Goat sample site.", IconClass = "icon-l-list-article", AllowCache = true)]

namespace DancingGoat.Widgets
{
    public class RecWidgetViewComponent : ViewComponent
    {
        public const string IDENTIFIER = "DancingGoat.HomePage.RecWidget";
        
        private static string[] dependency_keys = new[] { "node|dancinggoatcore|/articles|childnodes" };
        
        private readonly IPageDataContextRetriever pageDataContextRetriver;
        private readonly IAmazonPersonalizeService amazonService;


        public RecWidgetViewComponent(IAmazonPersonalizeService amazonService, IPageDataContextRetriever pageDataContextRetriver)
        {
            this.amazonService = amazonService;
            this.pageDataContextRetriver = pageDataContextRetriver;
        }


        /// <summary>
        /// Returns the model used by widgets' view.
        /// </summary>
        /// <param name="properties">Widget properties.</param>
        public ViewViewComponentResult Invoke(ComponentViewModel<RecWidgetProperties> viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            viewModel.CacheDependencies.CacheKeys = dependency_keys;

            var page = pageDataContextRetriver.Retrieve<TreeNode>().Page;
            var contactGuid = ContactManagementContext.GetCurrentContact()?.ContactGUID ?? Guid.Empty;
            var recommendedPages = amazonService.GetPagesRecommendationForContact(page.NodeSiteName, contactGuid, 10, page.DocumentCulture);

            return View("~/Components/Widgets/Rec/_RecWidget.cshtml", new RecWidgetViewModel { Recommendations = recommendedPages.Select(r => r.DocumentGuid.ToString()) });
        }
    }
}