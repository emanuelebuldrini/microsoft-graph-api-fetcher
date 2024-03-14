using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Store;

namespace MicrosoftGraphApiFetcher.Tests
{
    public class GraphApiStoreTest
    {
        [Fact]
        public void SaveGroupJsons_NullGroupsList_ExceptionThrown()
        {
            // Arrange
            var groupStore = new GraphApiStore();
            List<Group>? groups = null;

            // Act
            var saveLocation = groupStore.SaveGroupJsons(groups!);

            // Assert
            Assert.Null(saveLocation);
            Assert.NotNull(groupStore.Exceptions);
            Assert.Single(groupStore.Exceptions);
            Assert.IsType<ArgumentNullException>(groupStore.Exceptions[0]);
        }

        [Fact]
        public void SaveGroupJsons_CreatesDirectory_Successfully()
        {
            // Arrange
            var testDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDirectoryGroup1");
            var groupStore = new GraphApiStore(testDirectory);

            // Act
            var saveLocation = groupStore.SaveGroupJsons([], containingFolder: null);

            // Assert
            Assert.True(Directory.Exists(testDirectory));
            Assert.NotNull(saveLocation);
            Assert.Equal(testDirectory, saveLocation);
            Directory.Delete(testDirectory); // Clean up
        }

        [Fact]
        public void SaveGroupJsons_SavesJsonFiles_Successfully()
        {
            // Arrange
            var groupStore = new GraphApiStore();
            var groups = new List<Group>
            {
                new() { DisplayName = "Group1", Id="1" },
                new() { DisplayName = "Group2", Id="2" }
            };

            // Act
            var saveLocation = groupStore.SaveGroupJsons(groups);

            // Assert
            Assert.Equal(2, groupStore.SavedCount);
            Assert.NotNull(saveLocation);
            Assert.Equal(saveLocation, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MSGraph", "Groups"));
            foreach (var group in groups)
            {
                var filePath = Path.Combine(saveLocation, $"{group.DisplayName}.json");
                Assert.True(File.Exists(filePath));
                File.Delete(filePath); // Clean up
            }
        }

        [Fact]
        public void SaveGroupJsons_HandlesInvalidGroup()
        {
            // Arrange
            var groupStore = new GraphApiStore();
            var invalidGroup = new Group(); // Invalid group without required properties (DisplayName and Id)
            var groups = new List<Group> { invalidGroup };
            var groupContainingFolder = "TestGroups1";

            // Act
            var saveLocation = groupStore.SaveGroupJsons(groups, groupContainingFolder);

            // Assert
            Assert.NotEmpty(groupStore.Exceptions); // Exceptions should be thrown
            Assert.NotNull(saveLocation);
            Assert.Equal(saveLocation, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MSGraph", groupContainingFolder));
            Assert.False(Directory.EnumerateFileSystemEntries(saveLocation).Any());
        }

        [Fact]
        public void SaveGroupJsons_HandlesExceptionOnWrite()
        {
            // Arrange
            var invalidPath = "invalid:path"; // Invalid path to cause an exception
            var groupStore = new GraphApiStore(invalidPath);
            var group = new Group { DisplayName = "Group1", Id = "1" };
            var groups = new List<Group> { group };

            // Act
            var saveLocation = groupStore.SaveGroupJsons(groups);

            // Assert
            Assert.Null(saveLocation);
            Assert.NotEmpty(groupStore.Exceptions); // Exceptions should be thrown
        }

        [Fact]
        public void SaveUsersJsons_NullUsersList_ExceptionThrown()
        {
            // Arrange
            var graphApiStore = new GraphApiStore();
            List<User>? users = null;

            // Act
            var result = graphApiStore.SaveUserJsons(users!);

            // Assert
            Assert.Null(result);
            Assert.Single(graphApiStore.Exceptions);
            Assert.IsType<ArgumentNullException>(graphApiStore.Exceptions[0]);
        }

        [Fact]
        public void SaveUsersJsons_SavesJsonFiles_Successfully()
        {
            // Arrange
            var userStore = new GraphApiStore();
            var users = new List<User>
            {
                new() { DisplayName = "Mario", Id="1" },
                new() { DisplayName = "Luigi", Id="2" }
            };

            // Act
            var saveLocation = userStore.SaveUserJsons(users);

            // Assert
            Assert.Equal(2, userStore.SavedCount);
            Assert.NotNull(saveLocation);
            Assert.Equal(saveLocation, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MSGraph", "Users"));
            foreach (var user in users)
            {
                var filePath = Path.Combine(saveLocation, $"{user.DisplayName}.json");
                Assert.True(File.Exists(filePath));
                File.Delete(filePath); // Clean up
            }
        }

        [Fact]
        public void SaveUsersJsons_CreatesDirectory_Successfully()
        {
            // Arrange
            var testDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDirectoryUser1");
            var groupStore = new GraphApiStore(testDirectory);

            // Act
            var saveLocation = groupStore.SaveUserJsons([], containingFolder: null);

            // Assert
            Assert.True(Directory.Exists(testDirectory));
            Assert.NotNull(saveLocation);
            Assert.Equal(testDirectory, saveLocation);
            Directory.Delete(testDirectory); // Clean up
        }

        [Fact]
        public void SaveGroupJsons_HandlesInvalidUser()
        {
            // Arrange
            var groupStore = new GraphApiStore();
            var invalidUser = new User(); // Invalid user without required properties (DisplayName and Id)
            var users = new List<User> { invalidUser };
            var usersContainingFolder = "TestUsers1";

            // Act
            var saveLocation = groupStore.SaveUserJsons(users, usersContainingFolder);

            // Assert
            Assert.NotEmpty(groupStore.Exceptions); // Exceptions should be thrown
            Assert.NotNull(saveLocation);
            Assert.Equal(saveLocation, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MSGraph", usersContainingFolder));
            Assert.False(Directory.EnumerateFileSystemEntries(saveLocation).Any());
        }

        [Fact]
        public void SaveUserJsons_HandlesExceptionOnWrite()
        {
            // Arrange
            var invalidPath = "invalid:path"; // Invalid path to cause an exception
            var userStore = new GraphApiStore(invalidPath);
            var user = new User { DisplayName = "user1", Id = "1" };
            var users = new List<User> { user };

            // Act
            var saveLocation = userStore.SaveUserJsons(users);

            // Assert
            Assert.Null(saveLocation);
            Assert.NotEmpty(userStore.Exceptions); // Exceptions should be thrown
        }
    }
}
