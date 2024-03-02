using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Models;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MicrosoftGraphApiFetcher.Tests")]
namespace MicrosoftGraphApiFetcher.RestClient
{
    /// <summary>
    /// REST Client for Microsoft Graph API.
    /// </summary>
    /// <remarks> It uses <see cref="GraphServiceClient"/>. </remarks>
    public class GraphApiRestClient
    {
        private readonly GraphServiceClient _graphClient;

        /// <summary>
        /// Construct an instance of <see cref="GraphApiRestClient"/>.
        /// </summary>
        /// <param name="azureAdConfig">The Azure AD Configuration  to initialize the <see cref="GraphServiceClient"/>.</param>
        public GraphApiRestClient(AzureAdConfig azureAdConfig)
        {
            _graphClient = InitializeGraphClient(azureAdConfig);
        }

        /// <summary>
        /// Internal constructor used for testing.
        /// </summary>
        /// <param name="graphServiceClient"></param>
        internal GraphApiRestClient(GraphServiceClient graphServiceClient)
        {
            _graphClient = graphServiceClient;
        }

        /// <summary>
        /// Get the list of all groups of the configured Azure tenant using <see cref="GraphServiceClient"/>.
        /// </summary>
        /// <returns>The list of all groups of the configured Azure tenant.</returns>
        /// <remarks>The method fetches automatically all available pages.</remarks>
        public async Task<List<Group>> GetGroupsAsync()
        {
            List<Group> groups = [];
            var graphGroups = await _graphClient.Groups.GetAsync();
            if (graphGroups?.Value != null)
            {
                groups.AddRange(graphGroups.Value);
            }

            // Handle pagination fetching all pages.
            while (graphGroups?.OdataNextLink != null)
            {
                if (graphGroups.Value != null)
                {
                    groups.AddRange(graphGroups.Value);
                }

                graphGroups = await _graphClient.Groups.WithUrl(graphGroups.OdataNextLink).GetAsync();
            }

            return groups;
        }

        private static GraphServiceClient InitializeGraphClient(AzureAdConfig azureAdConfig)
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
