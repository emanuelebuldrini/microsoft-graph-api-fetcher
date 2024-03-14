using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using MicrosoftGraphApiFetcher.RestClient;
using MicrosoftGraphApiFetcher.Tests.Factories;
using Moq;

namespace MicrosoftGraphApiFetcher.Tests
{
    public class GraphApiRestClientTest
    {
        [Fact]
        public async Task GetGroupsAsync_ReturnsGroupsSuccessfully()
        {
            // Arrange           
            var expectedGroup = new Group { DisplayName = "Test Group", Id = "1" };
            var mockCollectionResponse = new GroupCollectionResponse
            {
                Value = [new Group { DisplayName = "Test Group", Id = "1" }]
            };
            var mockRequestAdapter = RequestAdapterMockFactory.Create();
            mockRequestAdapter.Setup(adapter => adapter.SendAsync(
                // Needs to be correct HTTP Method of the desired method 👇🏻
                It.Is<RequestInformation>(info => info.HttpMethod == Method.GET),
                // Needs to be method from 👇🏻 object type that will be returned from the SDK method.
                GroupCollectionResponse.CreateFromDiscriminatorValue,
                It.IsAny<Dictionary<string, ParsableFactory<IParsable>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCollectionResponse);            
            var graphServiceClient = new GraphServiceClient(mockRequestAdapter.Object);

            var graphApiRestClient= new GraphApiRestClient(graphServiceClient);

            // Act
            var groups = await graphApiRestClient.GetGroupsAsync();

            // Assert
            Assert.Single(groups);
            var fetchedGroup = groups[0];
            Assert.NotNull(fetchedGroup);
            Assert.Equal(expectedGroup.DisplayName, fetchedGroup.DisplayName);
            Assert.Equal(expectedGroup.Id, fetchedGroup.Id);
        }

        [Fact]
        public async Task GetGroupsAsync_ReturnsEmptyListWhenGraphServiceClientReturnsNull()
        {
            // Arrange           
            var expectedGroup = new Group { DisplayName = "Test Group", Id = "1" };
            var mockCollectionResponse = new GroupCollectionResponse
            {
                Value = null
            };
            var mockRequestAdapter = RequestAdapterMockFactory.Create();
            mockRequestAdapter.Setup(adapter => adapter.SendAsync(
                // Needs to be correct HTTP Method of the desired method 👇🏻
                It.Is<RequestInformation>(info => info.HttpMethod == Method.GET),
                // Needs to be method from 👇🏻 object type that will be returned from the SDK method.
                GroupCollectionResponse.CreateFromDiscriminatorValue,
                It.IsAny<Dictionary<string, ParsableFactory<IParsable>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCollectionResponse);
            var graphServiceClient = new GraphServiceClient(mockRequestAdapter.Object);

            var graphApiRestClient = new GraphApiRestClient(graphServiceClient);

            // Act
            var groups = await graphApiRestClient.GetGroupsAsync();

            // Assert
            Assert.Empty(groups);
        }

        [Fact]
        public async Task GetGroupsAsync_ReturnsEmptyListWhenGraphServiceClientReturnsEmptyCollection()
        {
            // Arrange           
            var expectedGroup = new Group { DisplayName = "Test Group", Id = "1" };
            var mockCollectionResponse = new GroupCollectionResponse
            {
                Value = []
            };
            var mockRequestAdapter = RequestAdapterMockFactory.Create();
            mockRequestAdapter.Setup(adapter => adapter.SendAsync(
                // Needs to be correct HTTP Method of the desired method 👇🏻
                It.Is<RequestInformation>(info => info.HttpMethod == Method.GET),
                // Needs to be method from 👇🏻 object type that will be returned from the SDK method.
                GroupCollectionResponse.CreateFromDiscriminatorValue,
                It.IsAny<Dictionary<string, ParsableFactory<IParsable>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCollectionResponse);
            var graphServiceClient = new GraphServiceClient(mockRequestAdapter.Object);

            var graphApiRestClient = new GraphApiRestClient(graphServiceClient);

            // Act
            var groups = await graphApiRestClient.GetGroupsAsync();

            // Assert
            Assert.Empty(groups);
        }

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

            var graphApiRestClient = new GraphApiRestClient(graphServiceClient);

            // Act
            var users = await graphApiRestClient.GetUsersAsync();

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

            var graphApiRestClient = new GraphApiRestClient(graphServiceClient);

            // Act
            var users = await graphApiRestClient.GetUsersAsync();

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

            var graphApiRestClient = new GraphApiRestClient(graphServiceClient);

            // Act
            var users = await graphApiRestClient.GetUsersAsync();

            // Assert
            Assert.Empty(users);
        }

    }
}
