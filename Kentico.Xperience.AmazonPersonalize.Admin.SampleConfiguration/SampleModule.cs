using System.Collections.Generic;

using CMS;
using CMS.Core;

using Kentico.Xperience.AmazonPersonalize.Admin.SampleConfiguration;

[assembly: AssemblyDiscoverable]

[assembly: RegisterModule(typeof(SampleModule))]

namespace Kentico.Xperience.AmazonPersonalize.Admin.SampleConfiguration
{
    /// <summary>
    /// Represents a sample that shows how to configure page type mappings for DancingGoatCore site for the Amazon Personalize content recommendation module for the Xperience admin interface.
    /// </summary>
    public class SampleModule : CMS.DataEngine.Module
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleModule"/> class.
        /// </summary>
        public SampleModule() : base("Kentico.Xperience.AmazonPersonalize.Admin.SampleConfiguration")
        {
        }


        protected override void OnInit()
        {
            base.OnInit();
            ConfigurePageTypes();
        }

        private void ConfigurePageTypes()
        {
            const string SITE_NAME = "DancingGoatCore";
            var fieldMapper = Service.Resolve<IFieldMapper>();
            var configurations = fieldMapper.GetConfigurations(SITE_NAME);
            configurations.IncludedCultures.Add("en-us");
            configurations.Mappings.Add("cms.document.DancingGoatCore.Article", new List<FieldMapping>
            {
                new FieldMapping("ArticleTitle", "Title"),
                new FieldMapping("ArticleSummary", "Summary"),
                new FieldMapping(article => article.GetValue("ArticleText").ToString(), "Text")
            });
        }
    }
}