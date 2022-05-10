using CMS;
using CMS.Core;
using CMS.DocumentEngine;

using Kentico.Xperience.AmazonPersonalize.Admin;

[assembly: AssemblyDiscoverable]

[assembly: RegisterModule(typeof(Module))]

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    public class Module : CMS.DataEngine.Module
    {
        public const string NAME = "Kentico.Experience.AmazonPersonalize.Admin";

        private IAmazonPersonalizeService mContentService;


        private IAmazonPersonalizeService ContentService
        {
            get
            {
                return mContentService ?? (mContentService = Service.Resolve<IAmazonPersonalizeService>());
            }
        }


        public Module() : base(NAME)
        {
        }

        protected override void OnInit()
        {
            base.OnInit();

            DocumentEvents.Insert.After += PageCreated;
            DocumentEvents.Update.After += PageUpdated;
            DocumentEvents.Delete.After += PageDeleted;
        }


        private void PageCreated(object sender, DocumentEventArgs e)
        {
            ContentService.PageCreated(e.Node);
        }


        private void PageUpdated(object sender, DocumentEventArgs e)
        {
            ContentService.PageUpdated(e.Node);
        }


        private void PageDeleted(object sender, DocumentEventArgs e)
        {
            ContentService.PageDeleted(e.Node);
        }
    }
}
