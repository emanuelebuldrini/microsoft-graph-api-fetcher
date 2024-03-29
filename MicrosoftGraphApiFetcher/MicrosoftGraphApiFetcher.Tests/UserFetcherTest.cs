using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using MicrosoftGraphApiFetcher.Core;
using MicrosoftGraphApiFetcher.RestClient.DirectoryObjectStrategies;
using MicrosoftGraphApiFetcher.Tests.Factories;
using Moq;

namespace MicrosoftGraphApiFetcher.Tests
{
    public class UserFetcherTest
    {
        [Fact]
        public async Task GetUsersAsync_ReturnsUsersSuccessfully()
        {
            // Arrange           
            var expectedUser = new User { DisplayName = "Test User", Id = "1" };
            var mockCollectionResponse = new UserCollectionResponse
            {
                Value = [new User { DisplayName = "Test User", Id = "1" }]
            };
            var mockRequestAdapter = RequestAdapterMockFactory.Create();
            mockRequestAdapter.Setup(adapter => adapter.SendAsync(
                // Needs to be correct HTTP Method of the desired method 👇🏻
                It.Is<RequestInformation>(info => info.HttpMethod == Method.GET),
                // Needs to be method from 👇🏻 object type that will be returned from the SDK method.
                UserCollectionResponse.CreateFromDiscriminatorValue,
                It.IsAny<Dictionary<string, ParsableFactory<IParsable>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCollectionResponse);
            var graphServiceClient = new GraphServiceClient(mockRequestAdapter.Object);
            var fetcher = new DirectoryObjectFetcher<UserCollectionResponse, User>(graphServiceClient);

            // Act
            var users = await fetcher.GetDirectoryObjectsAsync(new FetchUserStrategy());

            // Assert
            Assert.Single(users);
            var fetchedUser = users[0];
            Assert.NotNull(fetchedUser);
            Assert.Equal(expectedUser.DisplayName, fetchedUser.DisplayName);
            Assert.Equal(expectedUser.Id, fetchedUser.Id);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsEmptyListWhenGraphServiceClientReturnsNull()
        {
            // Arrange           
            var expectedUser = new User { DisplayName = "Test User", Id = "1" };
            var mockCollectionResponse = new UserCollectionResponse
            {
                Value = null
            };
            var mockRequestAdapter = RequestAdapterMockFactory.Create();
            mockRequestAdapter.Setup(adapter => adapter.SendAsync(
                // Needs to be correct HTTP Method of the desired method 👇🏻
                It.Is<RequestInformation>(info => info.HttpMethod == Method.GET),
                // Needs to be method from 👇🏻 object type that will be returned from the SDK method.
                UserCollectionResponse.CreateFromDiscriminatorValue,
                It.IsAny<Dictionary<string, ParsableFactory<IParsable>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCollectionResponse);
            var graphServiceClient = new GraphServiceClient(mockRequestAdapter.Object);
            var fetcher = new DirectoryObjectFetcher<UserCollectionResponse, User>(graphServiceClient);

            // Act
            var users = await fetcher.GetDirectoryObjectsAsync(new FetchUserStrategy());

            // Assert
            Assert.Empty(users);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsEmptyListWhenGraphServiceClientReturnsEmptyCollection()
        {
            // Arrange           
            var expectedUser = new User { DisplayName = "Test User", Id = "1" };
            var mockCollectionResponse = new UserCollectionResponse
            {
                Value = []
            };
            var mockRequestAdapter = RequestAdapterMockFactory.Create();
            mockRequestAdapter.Setup(adapter => adapter.SendAsync(
                // Needs to be correct HTTP Method of the desired method 👇🏻
                It.Is<RequestInformation>(info => info.HttpMethod == Method.GET),
                // Needs to be method from 👇🏻 object type that will be returned from the SDK method.
                UserCollectionResponse.CreateFromDiscriminatorValue,
                It.IsAny<Dictionary<string, ParsableFactory<IParsable>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCollectionResponse);
            var graphServiceClient = new GraphServiceClient(mockRequestAdapter.Object);
            var fetcher = new DirectoryObjectFetcher<UserCollectionResponse, User>(graphServiceClient);

            // Act
            var users = await fetcher.GetDirectoryObjectsAsync(new FetchUserStrategy());

            // Assert
            Assert.Empty(users);
        }

    }
}
