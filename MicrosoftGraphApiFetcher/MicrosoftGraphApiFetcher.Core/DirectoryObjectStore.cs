using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Core.Strategies;
using System.Text.Json;

namespace MicrosoftGraphApiFetcher.Core
{
    /// <summary>
    /// A store for Microsoft Graph <see cref="DirectoryObject"/> resources.
    /// </summary>
    /// <remarks> If no <see cref="baseDirectoryPath"></see> is provided, the default value is the current application path combined with "MSGraph".</remarks>
    /// <param name="baseDirectoryPath">The base directory path to store resources</param>
    public class DirectoryObjectStore<T>(string? baseDirectoryPath = null) : IDirectoryObjectStore<T>
        where T : DirectoryObject, new()
    {
        private readonly string _baseDirectoryPath = string.IsNullOrWhiteSpace(baseDirectoryPath) ?
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MSGraph") : baseDirectoryPath;

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

        /// <summary>
        /// Save the provided list of directory objects in distinct JSON files.
        /// </summary>
        /// <remarks> The base destination folder is set on the constructor of this class. You can set the <paramref name="containingFolder"/>
        /// to have a specific container for this directory object type. The Folder must be empty if already existing.
        /// The directory structure is created automatically if not existing. You need write permission on the specified folder.
        /// If no serialization options are provided, the default <see cref="JsonSerializerOptions"/> are used.
        /// An error on a group save does not abort the operation.
        /// Any exception raised in the process is collected in the <see cref="Exceptions"/> list.
        /// </remarks>
        /// <param name="directoryObjects">The list of directory objects to save.</param>
        /// <param name="nameObjectStrategy">A strategy to obtain the name of the specific <see cref="DirectoryObject"/>.</param>
        /// <param name="containingFolder">The containing folder for the JSON files. Pass null if you do not want a containing folder.</param>
        /// <param name="serializationOptions">The JSON serialization preferences.</param>
        /// <returns>The save location for the JSON files or null if the operation was aborted before completion.</returns>
        public string? SaveDirectoryObjectJson(List<T> directoryObjects, INameDirectoryObject<T> nameObjectStrategy, string? containingFolder = null, JsonSerializerOptions? serializationOptions = null)
        {
            SavedCount = 0;
            Exceptions = [];

            // Check if the list of objects is provided.
            if (directoryObjects == null)
            {
                Exceptions.Add(new ArgumentNullException(nameof(directoryObjects)));
                return null;
            }

            string directoryObjectContainingFolderPath = Path.Combine(_baseDirectoryPath, containingFolder ?? string.Empty);
            try
            {
                if (Directory.Exists(directoryObjectContainingFolderPath))
                {
                    // Containing directory should be empty to avoid overwrite other content. An exception is raised otherwise.
                    Directory.Delete(directoryObjectContainingFolderPath);
                }
                Directory.CreateDirectory(directoryObjectContainingFolderPath);
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
                return null;
            }

            foreach (var directoryObject in directoryObjects)
            {
                // First get the directory object name using the provided function
                string? directoryObjectName = nameObjectStrategy.GetDirectoryObjectName(directoryObject);
                // Validate directory object: should have at least Display Name and ID.
                if (string.IsNullOrWhiteSpace(directoryObjectName) || string.IsNullOrWhiteSpace(directoryObject?.Id))
                {
                    Exceptions.Add(new Exception("An object cannot be saved due to undefined required properties."));
                    continue;
                }

                try
                {
                    // Construct the file path.
                    string filePath = Path.Combine(directoryObjectContainingFolderPath, $"{directoryObjectName}.json");
                    // Make sure that the file name is unique adding a suffix in case it already exists.
                    int i = 2;
                    while (File.Exists(filePath))
                    {
                        filePath = Path.Combine(directoryObjectContainingFolderPath, $"{directoryObjectName} ({i++}).json");
                    }

                    File.WriteAllText(filePath, JsonSerializer.Serialize(directoryObject, options: serializationOptions));
                    SavedCount++;
                }
                // One error in a group save does not abort the operation.
                catch (Exception ex)
                {
                    Exceptions.Add(ex);
                }
            }

            return directoryObjectContainingFolderPath;
        }
    }
}