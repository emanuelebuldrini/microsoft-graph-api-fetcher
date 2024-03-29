using Microsoft.Graph;

namespace MicrosoftGraphApiFetcher.Core
{
    public interface IGraphClientInitializer
    {
        GraphServiceClient GetInstance();
    }
}
