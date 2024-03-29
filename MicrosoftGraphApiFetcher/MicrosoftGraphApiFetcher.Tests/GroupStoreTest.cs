using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Core;
using MicrosoftGraphApiFetcher.Infrastructure.NameDirectoryObject;

namespace MicrosoftGraphApiFetcher.Tests
{
    public class GroupStoreTest
    {
        [Fact]
        public void SaveGroupJson_NullGroupsList_ExceptionThrown()
        {
            // Arrange
            var groupStore = new DirectoryObjectStore<Group>();
            List<Group>? groups = null;

            // Act
            var saveLocation = groupStore.SaveDirectoryObjectJson(groups!, new NameGroupStrategy());

            // Assert
            Assert.Null(saveLocation);
            Assert.NotNull(groupStore.Exceptions);
            Assert.Single(groupStore.Exceptions);
            Assert.IsType<ArgumentNullException>(groupStore.Exceptions[0]);
        }

        [Fact]
        public void SaveGroupJson_CreatesDirectory_Successfully()
        {
            // Arrange
            var testDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDirectoryGroup1");
            var groupStore = new DirectoryObjectStore<Group>(testDirectory);

            // Act
            var saveLocation = groupStore.SaveDirectoryObjectJson([], new NameGroupStrategy(), containingFolder: null);

            // Assert
            Assert.True(Directory.Exists(testDirectory));
            Assert.NotNull(saveLocation);
            Assert.Equal(testDirectory, saveLocation);
            Directory.Delete(testDirectory); // Clean up
        }

        [Fact]
        public void SaveGroupJson_SavesJsonFiles_Successfully()
        {
            // Arrange
            var groupStore = new DirectoryObjectStore<Group>();
            var groups = new List<Group>
            {
                new() { DisplayName = "Group1", Id="1" },
                new() { DisplayName = "Group2", Id="2" }
            };
            string containingFolder = "Groups";

            // Act
            var saveLocation = groupStore.SaveDirectoryObjectJson(groups, new NameGroupStrategy(), containingFolder);

            // Assert
            Assert.Equal(2, groupStore.SavedCount);
            Assert.NotNull(saveLocation);
            Assert.Equal(saveLocation, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MSGraph", containingFolder));
            foreach (var group in groups)
            {
                var filePath = Path.Combine(saveLocation, $"{group.DisplayName}.json");
                Assert.True(File.Exists(filePath));
                File.Delete(filePath); // Clean up
            }
        }

        [Fact]
        public void SaveGroupJson_HandlesInvalidGroup()
        {
            // Arrange
            var groupStore = new DirectoryObjectStore<Group>();
            var invalidGroup = new Group(); // Invalid group without required properties (DisplayName and Id)
            var groups = new List<Group> { invalidGroup };
            var groupContainingFolder = "TestGroups1";

            // Act
            var saveLocation = groupStore.SaveDirectoryObjectJson(groups, new NameGroupStrategy(), groupContainingFolder);

            // Assert
            Assert.NotEmpty(groupStore.Exceptions); // Exceptions should be thrown
            Assert.NotNull(saveLocation);
            Assert.Equal(saveLocation, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MSGraph", groupContainingFolder));
            Assert.False(Directory.EnumerateFileSystemEntries(saveLocation).Any());
        }

        [Fact]
        public void SaveGroupJson_HandlesExceptionOnWrite()
        {
            // Arrange
            var invalidPath = "invalid:path"; // Invalid path to cause an exception
            var groupStore = new DirectoryObjectStore<Group>(invalidPath);
            var group = new Group { DisplayName = "Group1", Id = "1" };
            var groups = new List<Group> { group };

            // Act
            var saveLocation = groupStore.SaveDirectoryObjectJson(groups, new NameGroupStrategy());

            // Assert
            Assert.Null(saveLocation);
            Assert.NotEmpty(groupStore.Exceptions); // Exceptions should be thrown
        }
    }
}
