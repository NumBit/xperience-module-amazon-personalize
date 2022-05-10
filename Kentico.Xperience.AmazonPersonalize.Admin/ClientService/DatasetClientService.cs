using System;

using Amazon.Personalize;

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    public class DatasetClientService : IDatasetClientService
    {
        private readonly AmazonPersonalizeClient amazonClient;
        private readonly IDatasetManager databaseManager;
        private readonly string siteName;


        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetClientService"/> class.
        /// </summary>
        /// <param name="amazonClient">Amazon Personalize client to be managed.</param>
        public DatasetClientService(AmazonPersonalizeClient amazonClient, IDatasetManager databaseManager, string siteName)
        {
            this.amazonClient = amazonClient ?? throw new ArgumentNullException(nameof(amazonClient));
            this.databaseManager = databaseManager;
            this.siteName = siteName;
        }


        public void Init()
        {
            databaseManager.Init(amazonClient, siteName);
        }

        public void Reset()
        {
            databaseManager.Reset(amazonClient, siteName);
        }
    }
}
