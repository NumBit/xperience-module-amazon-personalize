using System;
using System.Linq;

using CMS.ContactManagement;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatCore;

using DancingGoat.Controllers;
using DancingGoat.Models;

using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Kentico.Xperience.AmazonPersonalize;

using Microsoft.AspNetCore.Mvc;

[assembly: RegisterPageRoute(ArticleSection.CLASS_NAME, typeof(ArticlesController))]
[assembly: RegisterPageRoute(Article.CLASS_NAME, typeof(ArticlesController), ActionName = "Detail")]

namespace DancingGoat.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ArticleRepository articleRepository;
        private readonly IAmazonPersonalizeService amazonPersonalizeService;
        private readonly IEventLogService eventLogService;

        public ArticlesController(ArticleRepository articleRepository, IAmazonPersonalizeService amazonPersonalizeService, IEventLogService eventLogService)
        {
            this.articleRepository = articleRepository;
            this.amazonPersonalizeService = amazonPersonalizeService;
            this.eventLogService = eventLogService;
        }


        public IActionResult Index([FromServices] IPageDataContextRetriever dataContextRetriever,
                                   [FromServices] IPageUrlRetriever pageUrlRetriever,
                                   [FromServices] IPageAttachmentUrlRetriever attachmentUrlRetriever)
        {
            var section = dataContextRetriever.Retrieve<TreeNode>().Page;
            var articles = articleRepository.GetArticles(section.NodeAliasPath);
                
            return View(articles.Select(article => ArticleViewModel.GetViewModel(article, pageUrlRetriever, attachmentUrlRetriever)));
        }


        public IActionResult Detail([FromServices] ArticleRepository articleRepository)
        {
            var article = articleRepository.GetCurrent();

            var contactGuid = ContactManagementContext.GetCurrentContact()?.ContactGUID ?? Guid.Empty;

            try
            {
                amazonPersonalizeService.LogPageView(article, contactGuid, Guid.NewGuid().ToString());
            }
            catch (Exception ex)
            {
                eventLogService.LogWarning("AmazonPersonalize", "LOGPAGEVIEW", ex);
            }

            return new TemplateResult(article);
        }
    }
}