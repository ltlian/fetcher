# Azure Function with managed identity

A demo project consisting of an Azure Function that fetches a file from external blob storage as well as its local filesystem.

Official documentation for working with a _Functions class library project_ and how it differentiates from _scripted_ functions can be found here:

<https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library?tabs=v4%2Ccmd#functions-class-library-project>

## Azure Storage

Function Apps use Azure Storage for various purposes, and any Function App _must_ have a backing store, including containerized apps.

See <https://learn.microsoft.com/en-us/azure/azure-functions/storage-considerations?tabs=azure-cli#storage-account-requirements>

> ### Storage account requirements
>
> When creating a function app, you must create or link to a general-purpose Azure Storage account that supports Blob, Queue, and Table storage. This requirement exists because Functions relies on Azure Storage for operations such as managing triggers and logging function executions.

While the same store can be used for input/output bindings for the function itself, this is not a requirement or a limitation. It is still important to distinguish between the backing operational store and one used for input/output.

This can make it challenging to properly manage access to Azure Storage for the function without including access keys or clientsecrets as configuration values.

### Managed Identity

In order to access an Azure Storage account using the function's managed identity it should ostensibly only be necessary with this change in the app's settings; ie. in `local.settings.json` or in the function app's Configuration in Azure Portal.

```diff
--- a/<settings>.json
+++ b/<settings>.json
- "AzureWebJobsStorage": "DefaultEndpointsProtocol=https;AccountName=<account_name>;AccountKey=[account_key_we_want_to_get_rid_of]==;EndpointSuffix=core.windows.net",
+ "AzureWebJobsStorage__accountName": "<account_name>"
```

Similarly, for configuring Blob input for Managed Identity

```json
"MyConnection__blobServiceUri": "https://<storage_account_name>.blob.core.windows.net"
```

Where the prefix is a chosen string to be used by the input binding, in this case `BlobInput`.

```cs
public async Task<HttpResponseData> Run
(
    [HttpTrigger(AuthorizationLevel.Function, Http.Get)] HttpRequestData req,
    [BlobInput("index.html", Connection = "MyConnection")] string indexHtml
)
```

Read more about connecting with managed identities:

- <https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob-input>
- <https://learn.microsoft.com/en-us/azure/azure-functions/functions-identity-based-connections-tutorial#edit-the-azurewebjobsstorage-configuration>

Note a few issues with this at the moment. See links below.

- <https://github.com/Azure/azure-functions-core-tools/issues/2564>
- <https://github.com/Azure/Azure-Functions/issues/2189>

## Dependency Injection in Azure Functions

Be aware that the DI approach may vary depending on whether the app is running in-process or isolated.

In-process:

<https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection>

Isolated:

<https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#dependency-injection>
