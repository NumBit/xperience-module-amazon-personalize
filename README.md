
# Kentico Xperience Amazon Personalize Modules

This repository contains the source for modules that integrate  Kentico Xperience with Amazon Personalize recommender system.

## Description

This project consists of two modules:

* Administration module - adds the Amazon PErsonalize content recommendation application to the application menu in the Xperience administration interface, where you can populate the dataset with specified page types.
* Live site module - contains API that allows you to track contact activities and load recommended pages, based on the current contact or the content of the current page.

## Requirements and prerequisites

* Kentico Xperience 13 installed. Both ASP.NET Core and ASP.NET MVC 5 development models are supported. However, the demo site in this repository is an ASP.NET Core project.
* Xperience Enterprise license edition for your site, as the integration uses on-line marketing features (e.g. Contacts).
* The Enable on-line marketing setting needs to be selected in the Settings application.
* You need to have a AWS account.

>This module serves primarily as a demonstration of recommendation possibilities. The current implementation may not fully reflect all possible scenarios (e.g. page workflow or processing of page builder content), and further development is needed for production usage that covers all use cases.

# Setup

## Amazon Personalize setup

The Amazon Personalize setup [guide](https://docs.aws.amazon.com/personalize/latest/dg/getting-started-console.html) can be followed for more information about
these steps.

1. Create a custom dataset group.
1. Create datasets with correct schemas.
    >Example schemas can be found in schemas subfolder. They can be extended for specific needs.

1. Create an event tracker

1. Create filters.
    Because only one filter can be applied at the same time, we need two
    filters: one filtering page types and culture and one only filtering
    culture.

    1. Create culture filter.

        `INCLUDE ItemID WHERE Items.DELETED IN ("false") AND Items.CULTURE IN ($CULTURE)`

    1. Create page types and culture filter.

        `INCLUDE ItemID WHERE Items.DELETED IN ("false") AND Items.CULTURE IN ($CULTURE) AND Items.PAGE_TYPE IN ($PAGE_TYPES)`

## Amazon Personalize solution and campaign

This step can be performed after succesfull initialization of Amazon Personalize.
>To be able to create solution you need:
>
>* 1000 records of combined interaction data
>* 25 unique users with at least 2 interactions each

1. Create solutions.

    Create two solutions:

    * AWS-user-personalization - for personalized recommendations
    * AWS-similar-items - for similar pages

    >The solution needs to be trained sucesfully before the campaign can be created.  
    >The user personalization solution is retrained automaticaly every 2 hours.
    >The similar items solution needs to be retrained manualy.

1. Create campaign.
    Create two campaigns:
    * campaign for the AWS-user-personalization solution
    * campaign for the AWS-similar-items solution
    >The user personalization campaign include always the newest solution.
    >The similar items campaign needs to be updated manualy.

## Administration setup

1. Open the solution with your administration project (~/WebApp.sln).

1. Include Kentico.AmazonPersonalize.Admin.KX13 -Version 1.0.0

1. Edit the administration project's web.config file and add the following keys into the *appSettings* section:

    ```xml
    <add key="DancingGoatCore.AmazonPersonalize.ContentRecommendation.AccessKey" value="" />
    <add key="DancingGoatCore.AmazonPersonalize.ContentRecommendation.SecretKey" value="" />
    <add key="DancingGoatCore.AmazonPersonalize.ContentRecommendation.InteractionsSchemaArn" value="" />
    <add key="DancingGoatCore.AmazonPersonalize.ContentRecommendation.RegionEndpoint" value="" />
    ```

    >Replace DancingGoatCore with the code name of your Xperience site, and the values with your Amazon Personalize API Tokens.

1. Rebuild the CMSApp project.

## Live site setup

1. Open the solution with your live site project (e.g., ~/DancingGoatCore.sln).

1. Include Kentico.AmazonPersonalize.KX13 -Version 1.0.0

1. Add the following keys into the configuration file of the live site project (e.g., appsettings.json for ASP.NET Core projects):

    ```json
    "DancingGoatCore.AmazonPersonalize.ContentRecommendation.AccessKey": "",
    "DancingGoatCore.AmazonPersonalize.ContentRecommendation.SecretKey": "",
    "DancingGoatCore.AmazonPersonalize.ContentRecommendation.SimmilarItemsCampaignArn": "",
    "DancingGoatCore.AmazonPersonalize.ContentRecommendation.PersonalizedCampaignArn": "",
    "DancingGoatCore.AmazonPersonalize.ContentRecommendation.EventTrackerArn": "",
    "DancingGoatCore.AmazonPersonalize.ContentRecommendation.DatasetGroupArn": "",
    "DancingGoatCore.AmazonPersonalize.ContentRecommendation.FilterWithPageTypesArn": "",
    "DancingGoatCore.AmazonPersonalize.ContentRecommendation.FilterWithoutPageTypesArn": "",
    "DancingGoatCore.AmazonPersonalize.ContentRecommendation.RegionEndpoint": ""
    ```

    >Replace DancingGoatCore with the code name of your Xperience site, and the values with your Amazon Personalize API Tokens.

1. Rebuild your live site project.

## Amazon Personalize dataset structure setup

To give relevant recommendations, the Amazon Personalize service requires the data of your Xperience pages.

By default, the integration module automatically maps the page type name and culture for pages into the Amazon Personalize dataset. For the fields of your page types, you need to manually ensure mapping to fields within the Amazon Personalize dataset.

Use the API provided by the integration package, as demonstrated by the example below. Run the code *on application start* of your Xperience administration application.

```cs
const string SITE_NAME = "DancingGoatCore";

var fieldMapper = Service.Resolve<IFieldMapper>();
var configurations = fieldMapper.GetConfigurations(SITE_NAME);
configurations.IncludedCultures.Add("en-us");

// Adds mappings for the 'DancingGoatCore.Article' page type
configurations.Mappings.Add("DancingGoatCore.Article", new List<FieldMapping>
{
    // Maps the article page type fields to the corresponding fields in the Amazon  Personalize dataset
    new FieldMapping("ArticleTitle", "Title"),
    new FieldMapping("ArticleSummary", "Summary"),

    // Example demonstrating advanced field mapping
    // This approach allows mapping of data outside standard page type fields, such as page tags, categories or images (URLs)
    new FieldMapping(article => article.GetValue("ArticleText").ToString(), "Text")
});
```

>We recommend using a custom Xperience module to run this initialization code on application start. See the example in the Kentico.Xperience.AmazonPersonalize.Admin.SampleConfiguration folder.

## Amazon Personalize initialization

1. Once the previous steps are done, open the Xperience administration interface and navigate to the Amazon Personalize content recommendation application.
1. Click the Populate dataset button to populate Amazon Personalize dataset with configured page types.

## Getting recommendation data

To work with the data from Amazon Personalize on the live site, you need to use the API of the live site integration module. Two types of recommendation scenarios are currently supports:

* Log page visits by contacts and then recommend pages to contacts according to their previous activity.
* Recommend related pages to visitors based on the content of the page they are currently viewing.

### Recommending pages based on contact page views

First, you need to ensure logging of page views:

```cs
private readonly IAmazonPersonalizeService amazonPersonalizeService;

public ExampleController(IAmazonPersonalizeService amazonPersonalizeService)
{
  this.amazonPersonalizeService = amazonPersonalizeService;
}

public void ExampleMethod()
{
  ...

  amazonPersonalizeService.LogPageView(TreeNode page, Guid contactGuid, string sessionId = null, List<TreeNode> impression = null);
}
```

Perform the logging for all page types where you have set up field mapping to the Amazon Personalize dataset.

Afterwards, you can get a list of recommended pages based on the contact's previous page views:

```cs
// Gets a collection of 'PageIdentifier' objects, 
// containing the identifiers of Xperience pages recommended for the specified contact
amazonPersonalizeService.GetPagesRecommendationForContact(string siteName, Guid contactGuid, int count, string culture, IEnumerable<string> pageTypes = null);
```

Use the data to display a list of recommendation on your website.

### Recommending related pages based on the current page

You can retrieve a list of pages that are chosen by the Amazon PErsonalize service based on the content of the current page:

```cs
private readonly IAmazonPersonalizeService amazonPersonalizeService;

public ExampleController(IAmazonPersonalizeService amazonPersonalizeService)
{
    this.amazonPersonalizeService = amazonPersonalizeService;
}

public void ExampleMethod()
{
    ...

    // Gets a collection of 'PageIdentifier' objects, 
    // containing the identifiers of Xperience pages recommended for the specified page and contact
    amazonPersonalizeService.GetPagesRecommendationForPage(TreeNode page, string siteName, int count, string culture, IEnumerable<string> pageTypes = null);
}
```
