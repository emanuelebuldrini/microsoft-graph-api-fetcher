using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Models;
using MicrosoftGraphApiFetcher.RestClient;
using MicrosoftGraphApiFetcher.Store;
using System.Text.Json;

// List of available commands
List<string> availableCommands =
[
    "Download groups",
    "Download users",
    // Add more commands here as needed
];

// By default the app writes beautified JSONs
var jsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true };

Console.WriteLine("Welcome to the Microsoft Graph API App!");

while (true)
{
    // Display available commands to the user
    Console.WriteLine("\nAvailable commands:");
    foreach (string command in availableCommands)
    {
        Console.WriteLine("- " + command);
    }

    // Prompt the user for input
    Console.Write("\nEnter a command: ");
    string? userInput = Console.ReadLine();

    // Check if the input command is available
    if (userInput != null && availableCommands.Contains(userInput))
    {
        // Execute the corresponding command
        switch (userInput)
        {
            case "Download groups":
                await ExecuteDownloadDirectoryObjects(
                    (GraphApiRestClient graphApiRestClient) => graphApiRestClient.GetGroupsAsync(),
                    (GraphApiStore graphApiStore, List<Group> directoryObjects) => graphApiStore.SaveGroupJsons(directoryObjects,
                    serializationOptions: jsonSerializerOptions)
                );
                break;
            case "Download users":
                await ExecuteDownloadDirectoryObjects(
                    (GraphApiRestClient graphApiRestClient) => graphApiRestClient.GetUsersAsync(),
                    (GraphApiStore graphApiStore, List<User> directoryObjects) => graphApiStore.SaveUserJsons(directoryObjects,
                    serializationOptions: jsonSerializerOptions)
                );
                break;
            // Add more cases for other commands here
            default:
                Console.WriteLine("Command not implemented.");
                break;
        }
    }
    else
    {
        Console.WriteLine("Invalid command. Please try again.");
    }
}

static async Task ExecuteDownloadDirectoryObjects<T>(Func<GraphApiRestClient, Task<List<T>>> fetchDirectoryObjects, Func<GraphApiStore, List<T>, string?> storeDirectoryObjects)
    where T : DirectoryObject
{
    string directoryObjectName = typeof(T).Name;

    Console.WriteLine($"\nFetching {directoryObjectName}(s)...");
    var azureAdConfig = GetAzureAdConfig();
    if (!ValidateAzureConfig(azureAdConfig))
    {
        Console.WriteLine("Invalid Azure AD configuration: Make sure to provide all required properties in your app settings.");
        return;
    }
    List<T> directoryObjects;
    try
    {
        GraphApiRestClient graphApiRestClient = new(azureAdConfig!);
        directoryObjects = await fetchDirectoryObjects(graphApiRestClient);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while fetching {directoryObjectName}: {ex.Message}");
        return;
    }
    Console.WriteLine($"{directoryObjects.Count} {directoryObjectName}(s) fetched.");
    Console.WriteLine($"\nSaving {directoryObjectName}(s)...");
    GraphApiStore graphApiStore = new();
    var saveLocation = storeDirectoryObjects(graphApiStore, directoryObjects);
    if (graphApiStore.Exceptions.Count > 0)
    {
        var exceptionMessages = graphApiStore.Exceptions.Select(s => s.Message);
        Console.WriteLine($"Error(s) occurred while saving JSON files: {string.Join(Environment.NewLine, exceptionMessages)}");
    }

    if (saveLocation != null)
    {
        Console.WriteLine($"JSON file for {graphApiStore.SavedCount} {directoryObjectName}(s) saved at location: \"{saveLocation}\".");
    }
}

static AzureAdConfig? GetAzureAdConfig()
{
    var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

    var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

    IConfiguration config = builder.Build();

    return config.GetRequiredSection("AzureAdConfig").Get<AzureAdConfig>();
}

static bool ValidateAzureConfig(AzureAdConfig? azureAdConfig)
{
    return !string.IsNullOrWhiteSpace(azureAdConfig?.TenantId)
            && !string.IsNullOrWhiteSpace(azureAdConfig?.AppId)
            && !string.IsNullOrWhiteSpace(azureAdConfig?.Secret);
}