using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Core.Strategies;

namespace MicrosoftGraphApiFetcher.Infrastructure.NameDirectoryObject
{
    public class NameGroupStrategy : INameDirectoryObject<Group>
    {
        public string? GetDirectoryObjectName(Group group) => group?.DisplayName;
    }
}
