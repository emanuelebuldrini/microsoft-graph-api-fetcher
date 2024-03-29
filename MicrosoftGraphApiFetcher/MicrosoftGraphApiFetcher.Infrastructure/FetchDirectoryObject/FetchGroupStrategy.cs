using Microsoft.Graph;
using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Core.Strategies;

namespace MicrosoftGraphApiFetcher.RestClient.DirectoryObjectStrategies
{
    public class FetchGroupStrategy : IFetchDirectoryObject<GroupCollectionResponse, Group>
    {
        public Task<GroupCollectionResponse?> GetDirectoryObjectCollectionFromGraph(GraphServiceClient graphServiceClient)
            => graphServiceClient.Groups.GetAsync();

        public List<Group>? GetDirectoryObjectCollectionValue(GroupCollectionResponse? directoryObjectCollection)
            => directoryObjectCollection?.Value;

        public Task<GroupCollectionResponse?> GetDirectoryObjectPageFromGraph(GraphServiceClient graphServiceClient, string pageUrl)
            => graphServiceClient.Groups.WithUrl(pageUrl).GetAsync();
    }
}
