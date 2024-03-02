using Microsoft.Graph.Models;
using System.Text.Json;

namespace MicrosoftGraphApiFetcher.Store
{
    /// <summary>
    /// A store for Microsoft Graph API resources.
    /// </summary>
    public class GraphApiGroupStore
    {
        /// <summary>
        /// The count of the saved items on the last operation executed.
        /// </summary>
        /// <remarks> A new operation execution reset this count.</remarks>
        public int SavedCount { get; private set; }
        /// <summary>
        /// The list of Exceptions occurred during the last operation executed.
        /// </summary>
        /// <remarks> A new operation execution reset this list.</remarks>
        public List<Exception> Exceptions { get; private set; } = [];

        private readonly string _baseDirectoryPath;

        /// <summary>
        /// Construct an instance of <see cref="GraphApiGroupStore"/>. 
        /// </summary>
        /// <remarks> If no <see cref="baseDirectoryPath"></see> is provided, the default value is the current application path combined with "MSGraph".</remarks>
        /// <param name="baseDirectoryPath">The base directory path to store resources</param>
        public GraphApiGroupStore(string? baseDirectoryPath = null)
        {
            if (string.IsNullOrWhiteSpace(baseDirectoryPath))
            {
                // Default to the assembly location of the app as base path
                _baseDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MSGraph");
            }
            else
            {
                _baseDirectoryPath = baseDirectoryPath;
            }
        }

        /// <summary>
        /// Save the provided list of groups in JSON format.
        /// </summary>
        /// <remarks> The base destination folder is set on the constructor of this class. The Folder must be empty if already existing.
        /// The directory structure is created automatically if not existing. You need write permission on the specified folder.
        /// If no serialization options are provided, the default <see cref="JsonSerializerOptions"></see> are used.
        /// An error on a group save does not abort the operation.
        /// Any exception raised in the process is collected in the <see cref="Exceptions"/> list.
        /// </remarks>
        /// <param name="groups">The list of the groups to save.</param>
        /// <param name="containingFolder">The containing folder for the JSON files. By default is "Groups". Pass null if you do not want a containing folder.</param>
        /// <param name="serializationOptions">The JSON serialization preferences.</param>
        /// <returns>The save location for the JSON files or null if the operation was aborted before completion.</returns>
        public string? SaveGroupJsons(List<Group> groups, string? containingFolder = "Groups", JsonSerializerOptions? serializationOptions = null)
        {
            SavedCount = 0;
            Exceptions = [];

            // Check if the list of groups is provided.
            if (groups == null)
            {
                Exceptions.Add(new ArgumentNullException(nameof(groups)));
                return null;
            }

            string groupContainingFolderPath = Path.Combine(_baseDirectoryPath, containingFolder ?? string.Empty);
            try
            {
                if (Directory.Exists(groupContainingFolderPath))
                {
                    // Containing directory should be empty to avoid overwrite other content. An exception is raised otherwise.
                    Directory.Delete(groupContainingFolderPath);
                }
                Directory.CreateDirectory(groupContainingFolderPath);
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
                return null;
            }

            foreach (var group in groups)
            {
                // Validate group: should have at least Display Name and ID.
                if (string.IsNullOrWhiteSpace(group?.DisplayName) || string.IsNullOrWhiteSpace(group?.Id))
                {
                    Exceptions.Add(new Exception("A Group cannot be saved due to undefined required properties."));
                    continue;
                }

                try
                {
                    // Construct the file path.
                    string filePath = Path.Combine(groupContainingFolderPath, $"{group.DisplayName}.json");
                    // Make sure that the file name is unique adding a suffix in case it already exists.
                    int i = 2;
                    while(File.Exists(filePath))
                    {
                        filePath = Path.Combine(groupContainingFolderPath, $"{group.DisplayName} ({i++}).json");
                    }

                    File.WriteAllText(filePath, JsonSerializer.Serialize(group, options: serializationOptions));
                    SavedCount++;
                }
                // One error in a group save does not abort the operation.
                catch (Exception ex)
                {
                    Exceptions.Add(ex);
                }
            }

            return groupContainingFolderPath;
        }
    }
}