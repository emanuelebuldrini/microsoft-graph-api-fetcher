using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Core.Strategies;

namespace MicrosoftGraphApiFetcher.Core
{
    public interface IDirectoryObjectFetcher<T, V>
         where T : BaseCollectionPaginationCountResponse
         where V : DirectoryObject, new()
    {
        Task<List<V>> GetDirectoryObjectsAsync(IFetchDirectoryObject<T, V> strategy);
    }
}
