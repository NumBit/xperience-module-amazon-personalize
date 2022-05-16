using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Core;
using CMS.DocumentEngine.Types.DancingGoatCore;

using DancingGoat.Models;
using DancingGoat.PageTemplates;

using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Kentico.Xperience.AmazonPersonalize;

[assembly: RegisterPageTemplate("DancingGoat.ArticleWithSidebar", "Article with sidebar", typeof(ArticleWithSideBarProperties), "~/PageTemplates/ArticleWithSidebar/_ArticleWithSidebar.cshtml", Description = "Displays an article detail with related articles on the right side.", IconClass = "icon-l-text-col")]

namespace DancingGoat.PageTemplates
{
    public class ArticleWithSidebarPageTemplateService
    {
        private readonly IPageDataContextRetriever pageDataContextRetriver;
        private readonly IPageUrlRetriever pageUrlRetriever;
        private readonly IPageAttachmentUrlRetriever attachmentUrlRetriever;
        private readonly IAmazonPersonalizeService amazonPersonalizeService;
        private readonly ArticleRepository articleRepository;
        private readonly IEventLogService eventLogService;


        public ArticleWithSidebarPageTemplateService(IPageDataContextRetriever pageDataContextRetriver, IPageUrlRetriever pageUrlRetriever, IPageAttachmentUrlRetriever attachmentUrlRetriever, IAmazonPersonalizeService amazonPersonalizeService, ArticleRepository articleRepository, IEventLogService eventLogService)
        {
            this.pageDataContextRetriver = pageDataContextRetriver;
            this.pageUrlRetriever = pageUrlRetriever;
            this.attachmentUrlRetriever = attachmentUrlRetriever;
            this.amazonPersonalizeService = amazonPersonalizeService;
            this.articleRepository = articleRepository;
            this.eventLogService = eventLogService;
        }


        public ArticleWithSideBarViewModel GetTemplateModel(ArticleWithSideBarProperties templateProperties)
        {
            var article = pageDataContextRetriver.Retrieve<Article>().Page;
            
            var recommendedArticles = GetRecommendedArticles(article);

            return ArticleWithSideBarViewModel.GetViewModel(article, templateProperties, pageUrlRetriever, attachmentUrlRetriever, recommendedArticles);
        }


        private IEnumerable<Article> GetRecommendedArticles(Article article)
        {
            try
            {
                var recommendation = amazonPersonalizeService.GetPagesRecommendationForPage(article, article.NodeSiteName, 2, article.DocumentCulture, new[] { article.TypeInfo.ObjectType });

                return articleRepository.GetArticles(recommendation.Select(r => r.NodeGuid));
            }
            catch (Exception ex)
            {
                eventLogService.LogException("Amazon Personalize", "GETRECOMMENDATION", ex);
                return Enumerable.Empty<Article>();
            }
        }
    }
}