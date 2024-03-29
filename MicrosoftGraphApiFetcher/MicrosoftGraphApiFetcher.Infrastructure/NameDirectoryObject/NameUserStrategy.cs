using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Core.Strategies;

namespace MicrosoftGraphApiFetcher.Infrastructure.NameDirectoryObject
{
    public class NameUserStrategy : INameDirectoryObject<User>
    {
        public string? GetDirectoryObjectName(User user) => user?.DisplayName;
    }
}
