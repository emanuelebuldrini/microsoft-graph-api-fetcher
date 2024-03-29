using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Core;
using MicrosoftGraphApiFetcher.Core.Strategies;
using MicrosoftGraphApiFetcher.Infrastructure;
using System.Text.Json;
using MicrosoftGraphApiFetcher.RestClient.DirectoryObjectStrategies;
using MicrosoftGraphApiFetcher.Infrastructure.NameDirectoryObject;
using MicrosoftGraphApiFetcher.Infrastructure.Config;

// List of available commands
List<string> availableCommands =
[
    "Download groups",
    "Download users",
    // Add more commands here as needed
];

Console.WriteLine("Welcome to the Microsoft Graph API App!");

// Get Azure configure to initialize the REST client.
var azureAdConfig = GetAzureAdConfig();
if (!ValidateAzureConfig(azureAdConfig))
{
    Console.WriteLine("Invalid Azure AD configuration: Make sure to provide all required properties in your app settings.");
    return;
}

var graphClientInitializer = new GraphClientInitializer(azureAdConfig!);
var graphClient = graphClientInitializer.GetInstance();

// Commands menu loop
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
                await ExecuteDownloadDirectoryObjects(graphClient, new FetchGroupStrategy(),
                    new NameGroupStrategy(), containingFolder: "Groups");
                break;
            case "Download users":
                await ExecuteDownloadDirectoryObjects(graphClient, new FetchUserStrategy(),
                    new NameUserStrategy(), containingFolder: "Users");
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

static async Task ExecuteDownloadDirectoryObjects<T, V>(GraphServiceClient graphClient,
    IFetchDirectoryObject<T, V> fetchStrategy,
    INameDirectoryObject<V> nameStrategy, string containingFolder)
    where T : BaseCollectionPaginationCountResponse
    where V : DirectoryObject, new()
{
    string directoryObjectName = typeof(T).Name;
    Console.WriteLine($"\nFetching {directoryObjectName}(s)...");
    List<V> directoryObjects;
    var fetcher = new DirectoryObjectFetcher<T, V>(graphClient);
    try
    {
        directoryObjects = await fetcher.GetDirectoryObjectsAsync(fetchStrategy);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while fetching {directoryObjectName}: {ex.Message}");
        return;
    }
    Console.WriteLine($"{directoryObjects.Count} {directoryObjectName}(s) fetched.");

    Console.WriteLine($"\nSaving {directoryObjectName}(s)...");
    var store = new DirectoryObjectStore<V>();
    var saveLocation = store.SaveDirectoryObjectJson(directoryObjects, nameStrategy,
        containingFolder, new JsonSerializerOptions() { WriteIndented = true }); // Write beautified JSONs.
    if (store.Exceptions.Count > 0)
    {
        var exceptionMessages = store.Exceptions.Select(s => s.Message);
        Console.WriteLine($"Error(s) occurred while saving JSON files: {string.Join(Environment.NewLine, exceptionMessages)}");
    }
    if (saveLocation != null)
    {
        Console.WriteLine($"JSON file for {store.SavedCount} {directoryObjectName}(s) saved at location: \"{saveLocation}\".");
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