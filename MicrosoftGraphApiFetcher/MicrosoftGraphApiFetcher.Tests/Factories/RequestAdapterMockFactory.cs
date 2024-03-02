using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using Moq;
using Microsoft.Kiota.Serialization.Json;
using Microsoft.Kiota.Abstractions.Store;


namespace MicrosoftGraphApiFetcher.Tests.Factories
{
    internal static class RequestAdapterMockFactory
    {
        internal static Mock<IRequestAdapter> Create(MockBehavior mockBehavior = MockBehavior.Strict)
        {
            var mockSerializationWriterFactory = new Mock<ISerializationWriterFactory>();
            mockSerializationWriterFactory.Setup(factory => factory.GetSerializationWriter(It.IsAny<string>()))
                .Returns((string _) => new JsonSerializationWriter());

            var mockRequestAdapter = new Mock<IRequestAdapter>(mockBehavior);
            mockRequestAdapter.SetupGet(adapter => adapter.BaseUrl).Returns("http://test.internal");
            mockRequestAdapter.SetupSet(adapter => adapter.BaseUrl = It.IsAny<string>());
            mockRequestAdapter.Setup(adapter => adapter.EnableBackingStore(It.IsAny<IBackingStoreFactory>()));
            mockRequestAdapter.SetupGet(adapter => adapter.SerializationWriterFactory).Returns(mockSerializationWriterFactory.Object);

            return mockRequestAdapter;
        }
    }
}
