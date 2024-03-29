using Microsoft.Graph.Models;

namespace MicrosoftGraphApiFetcher.Core.Strategies
{
    public interface INameDirectoryObject<T> where T: DirectoryObject
    {
        string? GetDirectoryObjectName(T directoryObject);
    }
}