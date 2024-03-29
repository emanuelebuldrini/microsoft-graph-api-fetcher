using Microsoft.Graph;
using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Core.Strategies;

namespace MicrosoftGraphApiFetcher.Core
{
    /// <summary>
    /// A <see cref="DirectoryObject"/> fetcher that uses <see cref="GraphServiceClient"/> .
    /// </summary>
    /// <param name="graphServiceClient">The graph service client used to fetch directory objects.</param>
    public class DirectoryObjectFetcher<T, V>(GraphServiceClient graphServiceClient) : IDirectoryObjectFetcher<T, V>
         where T : BaseCollectionPaginationCountResponse
         where V : DirectoryObject, new()
    {
        private readonly GraphServiceClient graphServiceClient = graphServiceClient;

        /// <summary>
        /// Get the list of the specific directory objects of the configured Azure tenant using <see cref="GraphServiceClient"/>.
        /// </summary>
        /// <returns>The list of all specific directory objects of the configured Azure tenant.</returns>
        /// <remarks>The method fetches automatically all available pages.</remarks>
        public async Task<List<V>> GetDirectoryObjectsAsync(IFetchDirectoryObject<T, V> strategy)
        {
            List<V> directoryObjects = [];
            var directoryObjectCollection = await strategy.GetDirectoryObjectCollectionFromGraph(graphServiceClient);
            var directoryObjectsValue = strategy.GetDirectoryObjectCollectionValue(directoryObjectCollection);
            if (directoryObjectsValue != null)
            {
                directoryObjects.AddRange(directoryObjectsValue);
            }

            // Handle pagination fetching all available pages.
            while (directoryObjectCollection?.OdataNextLink != null)
            {
                directoryObjectCollection = await strategy.GetDirectoryObjectPageFromGraph(graphServiceClient, directoryObjectCollection.OdataNextLink);
                directoryObjectsValue = strategy.GetDirectoryObjectCollectionValue(directoryObjectCollection);
                if (directoryObjectsValue != null)
                {
                    directoryObjects.AddRange(directoryObjectsValue);
                }
            }

            return directoryObjects;
        }
    }
}
