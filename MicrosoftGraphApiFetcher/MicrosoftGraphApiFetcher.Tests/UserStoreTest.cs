using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Core;
using MicrosoftGraphApiFetcher.Infrastructure.NameDirectoryObject;

namespace MicrosoftGraphApiFetcher.Tests
{
    public class GraphApiStoreTest
    {
        [Fact]
        public void SaveUserJson_NullUsersList_ExceptionThrown()
        {
            // Arrange
            var userStore = new DirectoryObjectStore<User>();
            List<User>? users = null;

            // Act
            var result = userStore.SaveDirectoryObjectJson(users!, new NameUserStrategy());

            // Assert
            Assert.Null(result);
            Assert.Single(userStore.Exceptions);
            Assert.IsType<ArgumentNullException>(userStore.Exceptions[0]);
        }

        [Fact]
        public void SaveUserJson_SavesJsonFiles_Successfully()
        {
            // Arrange
            var userStore = new DirectoryObjectStore<User>();
            var users = new List<User>
            {
                new() { DisplayName = "Mario", Id="1" },
                new() { DisplayName = "Luigi", Id="2" }
            };
            string containingFolder = "Users";
                
            // Act
            var saveLocation = userStore.SaveDirectoryObjectJson(users, new NameUserStrategy(), containingFolder);

            // Assert
            Assert.Equal(2, userStore.SavedCount);
            Assert.NotNull(saveLocation);
            Assert.Equal(saveLocation, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MSGraph", containingFolder));
            foreach (var user in users)
            {
                var filePath = Path.Combine(saveLocation, $"{user.DisplayName}.json");
                Assert.True(File.Exists(filePath));
                File.Delete(filePath); // Clean up
            }
        }

        [Fact]
        public void SaveUserJson_CreatesDirectory_Successfully()
        {
            // Arrange
            var testDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDirectoryUser1");
            var userStore = new DirectoryObjectStore<User>(testDirectory);

            // Act
            var saveLocation = userStore.SaveDirectoryObjectJson([], new NameUserStrategy(), containingFolder: null);

            // Assert
            Assert.True(Directory.Exists(testDirectory));
            Assert.NotNull(saveLocation);
            Assert.Equal(testDirectory, saveLocation);
            Directory.Delete(testDirectory); // Clean up
        }

        [Fact]
        public void SaveUserJson_HandlesInvalidUser()
        {
            // Arrange
            var userStore = new DirectoryObjectStore<User>();
            var invalidUser = new User(); // Invalid user without required properties (DisplayName and Id)
            var users = new List<User> { invalidUser };
            var usersContainingFolder = "TestUsers1";

            // Act
            var saveLocation = userStore.SaveDirectoryObjectJson(users, new NameUserStrategy(), usersContainingFolder);

            // Assert
            Assert.NotEmpty(userStore.Exceptions); // Exceptions should be thrown
            Assert.NotNull(saveLocation);
            Assert.Equal(saveLocation, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MSGraph", usersContainingFolder));
            Assert.False(Directory.EnumerateFileSystemEntries(saveLocation).Any());
        }

        [Fact]
        public void SaveUserJson_HandlesExceptionOnWrite()
        {
            // Arrange
            var invalidPath = "invalid:path"; // Invalid path to cause an exception
            var userStore = new DirectoryObjectStore<User>(invalidPath);
            var user = new User { DisplayName = "user1", Id = "1" };
            var users = new List<User> { user };

            // Act
            var saveLocation = userStore.SaveDirectoryObjectJson(users, new NameUserStrategy());

            // Assert
            Assert.Null(saveLocation);
            Assert.NotEmpty(userStore.Exceptions); // Exceptions should be thrown
        }
    }
}
