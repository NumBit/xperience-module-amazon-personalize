using CMS;
using Kentico.Xperience.AmazonPersonalize.Admin;

[assembly: RegisterImplementation(typeof(IServiceConfigurationProvider), typeof(ServiceConfigurationProvider), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]


namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    public interface IServiceConfigurationProvider
    {
        string GetAcessKey(string siteName);
        
        string GetSecretKey(string siteName);

        string GetDatasetGroupArn(string siteName);

        string GetInteractionsDatasetArn(string siteName);

        string GetInteractionsSchemaArn(string siteName);
        
        string GetItemSchemaArn(string siteName);
        
        string GetItemsDatasetArn(string siteName);
        
        string GetUsersDatasetArn(string siteName);
        
        string GetUsersSchemaArn(string siteName);
    }
}