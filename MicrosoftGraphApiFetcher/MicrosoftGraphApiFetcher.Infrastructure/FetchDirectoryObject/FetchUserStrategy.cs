using Microsoft.Graph;
using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Core.Strategies;

namespace MicrosoftGraphApiFetcher.RestClient.DirectoryObjectStrategies
{
    /// <summary>
    /// A strategy to fetch a <see cref="User"/> using a given <see cref="GraphServiceClient"/>.
    /// </summary>
    public class FetchUserStrategy() : IFetchDirectoryObject<UserCollectionResponse, User>
    {
        public Task<UserCollectionResponse?> GetDirectoryObjectCollectionFromGraph(GraphServiceClient graphServiceClient)
            => graphServiceClient.Users.GetAsync();

        public List<User>? GetDirectoryObjectCollectionValue(UserCollectionResponse? directoryObjectCollection)
            => directoryObjectCollection?.Value;

        public Task<UserCollectionResponse?> GetDirectoryObjectPageFromGraph(GraphServiceClient graphServiceClient, string pageUrl)
            => graphServiceClient.Users.WithUrl(pageUrl).GetAsync();
    }
}
