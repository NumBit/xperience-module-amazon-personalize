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
    private readonly IDatasetClientServiceProvider datasetClientServiceProvider;
    private readonly IFieldMapper fieldMapper;

    public CMSModules_AmazonPersonalize_setup()
    {
        settingsService = Service.Resolve<ISettingsService>();
        siteService = Service.Resolve<ISiteService>();
        contentService = Service.Resolve<IAmazonPersonalizeService>();
        datasetClientServiceProvider = Service.Resolve<IDatasetClientServiceProvider>();
        fieldMapper = Service.Resolve<IFieldMapper>();
    }

    public void Page_Load()
    {
        if(!IsOnlineMarketingEnabled())
        {
            btnIntDbStructure.Enabled = false;
            btnResetDatabase.Enabled = false;
            pageTypesWrapper.Visible = false;

            ShowInformation("On-line marketing must be enabled to use the Amazon Personalize content recommendation.");
        }
        else if(!datasetClientServiceProvider.IsAvailable(CurrentSiteName))
        {
            btnIntDbStructure.Enabled = false;
            btnResetDatabase.Enabled = false;
            pageTypesWrapper.Visible = false;

            ShowInformation("Amazon Personalize content recommendation is not available. Specify the Amazon Personalize Database ID and Private Token in the application configuration file.");
        }
        else if(!UserInfoProvider.IsAuthorizedPerResource(Kentico.Xperience.AmazonPersonalize.Admin.Module.NAME, "Modify", CurrentSiteName, CurrentUser))
        {
            btnIntDbStructure.Enabled = false;
            btnResetDatabase.Enabled = false;

            ShowInformation("You are not authorized to modify Amazon Personalize database structure on this site.");
        }

        pageTypes.InnerText = String.Join(", ", fieldMapper.GetConfigurations(CurrentSiteName).Mappings.Keys);
    }


    protected void ResetDatabase_Click(object sender, EventArgs e)
    {
        try
        {
            contentService.Reset(CurrentSiteName);

            ShowConfirmation("Database reset.");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
            Service.Resolve<IEventLogService>().LogException("AmazonPersonalize setup", "Reset", ex);
        }
    }

    protected void InitDatabase_Click(object sender, EventArgs e)
    {
        try
        {
            contentService.Init(CurrentSiteName);

            ShowConfirmation("Database initialized and populated with current pages.");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
            Service.Resolve<IEventLogService>().LogException("AmazonPersonalize setup", "Init", ex);
        }
    }

    protected void PopulateDatabase_Click(object sender, EventArgs e)
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