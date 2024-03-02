using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models;
using MicrosoftGraphApiFetcher.Models;
using MicrosoftGraphApiFetcher.RestClient;
using MicrosoftGraphApiFetcher.Store;

// List of available commands
List<string> availableCommands =
[
    "Download groups",
    // Add more commands here as needed
];

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
                await ExecuteDownloadGroups();
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

static async Task ExecuteDownloadGroups()
{
    Console.WriteLine("\nFetching groups...");
    var azureAdConfig = GetAzureAdConfig();
    if (!validateAzureConfig(azureAdConfig))
    {
        Console.WriteLine("Invalid Azure AD configuration: Make sure to provide all required properties in your app settings.");
        return;
    }
    List<Group> groups;
    try
    {
        GraphApiRestClient graphApiRestClient = new (azureAdConfig!);
        groups = await graphApiRestClient.GetGroupsAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while fetching groups: {ex.Message}");
        return;
    }
    Console.WriteLine($"{groups.Count} Group(s) fetched.");
    Console.WriteLine("\nSaving groups...");
    GraphApiGroupStore graphApiGroupStore = new();
    var saveLocation = graphApiGroupStore.SaveGroupJsons(groups, serializationOptions: new() { WriteIndented = true });
    if (graphApiGroupStore.Exceptions.Count > 0)
    {
        var exceptionMessages = graphApiGroupStore.Exceptions.Select(s => s.Message);
        Console.WriteLine($"Error(s) occurred while saving JSON files: {string.Join(Environment.NewLine, exceptionMessages)}");
    }

    if (saveLocation != null)
    {
        Console.WriteLine($"JSON file for {graphApiGroupStore.SavedCount} group(s) saved at location: \"{saveLocation}\".");
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

static bool validateAzureConfig(AzureAdConfig? azureAdConfig)
{
    return !string.IsNullOrWhiteSpace(azureAdConfig?.TenantId)
            && !string.IsNullOrWhiteSpace(azureAdConfig?.AppId)
            && !string.IsNullOrWhiteSpace(azureAdConfig?.Secret);
}