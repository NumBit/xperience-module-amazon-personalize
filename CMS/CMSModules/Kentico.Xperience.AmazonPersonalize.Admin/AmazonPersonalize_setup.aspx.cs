using System;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.Membership;
using CMS.UIControls;

using Kentico.Xperience.AmazonPersonalize.Admin;

[UIElement("Kentico.Xperience.AmazonPersonalize.Admin", "Kentico.Xperience.AmazonPersonalize.Admin")]
public partial class CMSModules_AmazonPersonalize_setup : CMSPage
{
    private readonly ISettingsService settingsService;
    private readonly ISiteService siteService;
    private readonly IAmazonPersonalizeService contentService;
    private readonly IFieldMapper fieldMapper;

    public CMSModules_AmazonPersonalize_setup()
    {
        settingsService = Service.Resolve<ISettingsService>();
        siteService = Service.Resolve<ISiteService>();
        contentService = Service.Resolve<IAmazonPersonalizeService>();
        fieldMapper = Service.Resolve<IFieldMapper>();
    }

    public void Page_Load()
    {
        if(!IsOnlineMarketingEnabled())
        {
            pageTypesWrapper.Visible = false;

            ShowInformation("On-line marketing must be enabled to use the Amazon Personalize content recommendation.");
        }

        pageTypes.InnerText = String.Join(", ", fieldMapper.GetConfigurations(CurrentSiteName).Mappings.Keys);
    }

    protected void PopulateDataset_Click(object sender, EventArgs e)
    {
        try
        {
            contentService.SendAll(CurrentSiteName);

            ShowConfirmation("Dataset populated with current pages.");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
            Service.Resolve<IEventLogService>().LogException("AmazonPersonalize setup", "Init", ex);
        }
    }

    private bool IsOnlineMarketingEnabled()
    {
        return settingsService[siteService.CurrentSite?.SiteName + ".CMSEnableOnlineMarketing"].ToBoolean(false);
    }
}