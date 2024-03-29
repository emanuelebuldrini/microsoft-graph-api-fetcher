using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Core.Strategies;
using System.Text.Json;

namespace MicrosoftGraphApiFetcher.Core
{
    public interface IDirectoryObjectStore<T>
        where T : DirectoryObject, new()
    {
        int SavedCount { get; }
        List<Exception> Exceptions { get; }
        string? SaveDirectoryObjectJson(List<T> directoryObjects, INameDirectoryObject<T> strategy, string? containingFolder = null, JsonSerializerOptions? serializationOptions = null);
    }
}
