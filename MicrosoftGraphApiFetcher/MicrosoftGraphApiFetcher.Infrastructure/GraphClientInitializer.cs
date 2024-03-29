using Azure.Identity;
using Microsoft.Graph;
using MicrosoftGraphApiFetcher.Core;
using MicrosoftGraphApiFetcher.Infrastructure.Config;

namespace MicrosoftGraphApiFetcher.Infrastructure
{
    /// <summary>
    /// An initializer for <see cref="GraphServiceClient"/>.
    /// </summary>
    public class GraphClientInitializer: IGraphClientInitializer
    {
        private GraphServiceClient? graphServiceClient;
        private readonly AzureAdConfig azureAdConfig;
        private readonly object instanceAccessLock = new();

        /// <summary>
        /// Construct an instance of <see cref="GraphClientInitializer"/>
        /// given the related <see cref="AzureAdConfig"/>.
        /// </summary>
        /// <param name="azureAdConfig"></param>
        public GraphClientInitializer(AzureAdConfig azureAdConfig)
        {
            this.azureAdConfig = azureAdConfig;
        }

        /// <summary>
        /// Get the <see cref="GraphServiceClient"></see> instance.
        /// </summary>
        /// <returns> A singleton GraphServiceClient instance within the class scope.</returns>
        public GraphServiceClient GetInstance()
        {
            // Use Double-Check Locking pattern to manage concurrency on first initialization.
            if (graphServiceClient == null)
            {
                lock (instanceAccessLock)
                {
                    graphServiceClient ??= GetGraphClient();
                }
            }
            
            return graphServiceClient;
        }

        private GraphServiceClient GetGraphClient()
        {
            var scopes = new[] { ".default" };

            //// https://learn.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
            var clientSecretCredential = new ClientSecretCredential(
                azureAdConfig.TenantId, azureAdConfig.AppId, azureAdConfig.Secret);

            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

            return graphClient;
        }
    }
}
