using Microsoft.Graph;
using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Core.Strategies;

namespace MicrosoftGraphApiFetcher.Core
{
    /// <summary>
    /// A <see cref="DirectoryObject"/> fetcher that uses <see cref="GraphServiceClient"/> .
    /// </summary>
    /// <param name="graphServiceClient">The <see cref="GraphServiceClient"/> to fetch directory objects.</param>
    public class DirectoryObjectFetcher<T, V>(GraphServiceClient graphServiceClient) : IDirectoryObjectFetcher<T, V>
         where T : BaseCollectionPaginationCountResponse
         where V : DirectoryObject, new()
    {
        private readonly GraphServiceClient graphServiceClient = graphServiceClient;

        /// <summary>
        /// Get the list of the specific directory objects of the configured Azure tenant using <see cref="GraphServiceClient"/>.
        /// </summary>
        /// <param name="fetchObjectStrategy">A strategy to fetch a specific <see cref="DirectoryObject"/>.</param>
        /// <returns>The list of all specific directory objects of the configured Azure tenant.</returns>
        /// <remarks>The method fetches automatically all available pages.</remarks>
        public async Task<List<V>> GetDirectoryObjectsAsync(IFetchDirectoryObject<T, V> fetchObjectStrategy)
        {
            List<V> directoryObjects = [];
            var directoryObjectCollection = await fetchObjectStrategy.GetDirectoryObjectCollectionFromGraph(graphServiceClient);
            var directoryObjectsValue = fetchObjectStrategy.GetDirectoryObjectCollectionValue(directoryObjectCollection);
            if (directoryObjectsValue != null)
            {
                directoryObjects.AddRange(directoryObjectsValue);
            }

            // Handle pagination fetching all available pages.
            while (directoryObjectCollection?.OdataNextLink != null)
            {
                directoryObjectCollection = await fetchObjectStrategy.GetDirectoryObjectPageFromGraph(graphServiceClient, directoryObjectCollection.OdataNextLink);
                directoryObjectsValue = fetchObjectStrategy.GetDirectoryObjectCollectionValue(directoryObjectCollection);
                if (directoryObjectsValue != null)
                {
                    directoryObjects.AddRange(directoryObjectsValue);
                }
            }

            return directoryObjects;
        }
    }
}
