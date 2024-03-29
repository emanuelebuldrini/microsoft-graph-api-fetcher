using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace MicrosoftGraphApiFetcher.Core.Strategies
{
    public interface IFetchDirectoryObject<T, V>
        where T : BaseCollectionPaginationCountResponse
        where V : DirectoryObject

    {
        Task<T?> GetDirectoryObjectCollectionFromGraph(GraphServiceClient graphServiceClient);
        List<V>? GetDirectoryObjectCollectionValue(T? directoryObjectCollection);
        Task<T?> GetDirectoryObjectPageFromGraph(GraphServiceClient graphServiceClient, string pageUrl);
    }
}