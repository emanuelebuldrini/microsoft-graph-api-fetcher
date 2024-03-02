using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Store;

namespace MicrosoftGraphApiFetcher.Tests
{
    public class GraphApiGroupStoreTest
    {
        [Fact]
        public void SaveGroupJsons_NullGroupsList_ExceptionThrown()
        {
            // Arrange
            var groupStore = new GraphApiGroupStore();
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
            var testDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDirectory1");
            var groupStore = new GraphApiGroupStore(testDirectory);

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
            var groupStore = new GraphApiGroupStore();
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
            var groupStore = new GraphApiGroupStore();
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
            var groupStore = new GraphApiGroupStore(invalidPath);
            var group = new Group { DisplayName = "Group1", Id = "1" };
            var groups = new List<Group> { group };

            // Act
            var saveLocation = groupStore.SaveGroupJsons(groups);

            // Assert
            Assert.Null(saveLocation);
            Assert.NotEmpty(groupStore.Exceptions); // Exceptions should be thrown
        }
    }
}
