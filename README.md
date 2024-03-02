# Microsoft Graph API Console App

This console application fetches data from your Microsoft tenant using the Microsoft Graph API.

## Setup

1. **Clone the repository:**  
   Clone this repository to your local machine using the following command:

   ```bash
   git clone https://github.com/emanuelebuldrini/microsoft-graph-api-fetcher.git
   ```

2. **Register an application in Azure AD:**  
   To use the Microsoft Graph API, you need to register an application in Azure Active Directory (Azure AD). Follow these steps:

   - Go to the [Azure portal](https://portal.azure.com/).
   - Navigate to **Azure Active Directory** > **App registrations**.
   - Click **New registration** and fill in the required details.
   - Note down the **Application (client) ID**, **Directory (tenant) ID**, and generate and note down a new **Client secret**.
   - Under **API permissions**, grant the necessary permissions (e.g., `User.Read` for user data).
   - Click **Add a permission** and add the required Microsoft Graph API permissions.

3. **Configure appsettings.json:**  
   Open the `appsettings.json` file in the console application project and update the following placeholders with the values obtained from the Azure portal:

   ```json
   {
     "AzureAdConfig": {
       "AppId": "<client_id>",
       "Secret": "<client_secret>",
       "TenantId": "<tenant_id>"
     }
   }
   ```

4. **Build the solution:**  
   Build the solution using Visual Studio or the following command:

   ```bash
   dotnet build
   ```

## Run the Application

Once the setup is complete, you can run the console application. Use the following command:

```bash
dotnet run
```
The application will provide you a list of available commands: Type the command and press enter to execute it. The app fetch data from your Azure tenant using the configured Microsoft Graph API credentials.

**Console App commands**
  ```bash
   Download groups
  ```
_Download groups_ fetches all groups from the configured Azure tenant and save each of them as a JSON file into the following location "/MSGraph/Groups/{group name}.json". The save location is relative to the app executable.
The folder must be empty if already existing. You need write permission on that folder.

