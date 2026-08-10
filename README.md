The .NET SDK of Lark Open API.

## About

This is .NET implementation to access Lark resources via Open API.

### Introduction

This repo is the .NET SDK of LarkSuite Open API.
It helps you to access the resources.

### Owner

This is maintained by community (3rd-party) but not offical (Lark or ByteDance) currently now.

This repo is [MIT licensed](./LICENSE).

### Main Types

- `LarkApi`: This is the entry point to access the resources of LarkSuite.
- `LarkApiUtils`: Additional utitilies for `LarkApi` and its response models.

## How to Use

You can import this library into your project.

### Initialization

The first step is to initialize an instance of LarkSuite API.
It requires an app ID and the app secret key.

```csharp
using LarkSuite;
```

```csharp
var larkApi = new LarkApi("{LARK_OAPI_APP_ID}", "{LARK_OAPI_APP_SECRET}");
```

Then resolve token.

```csharp
larkApi.GetTenantTokenAsync(cancellationToken);
```

The method above is used to obtain the app’s tenant token,
which will then be stored persistently in the `LarkApi` instance.
To obtain a user token instead, call the following method.
Note that only one of these two methods should be used.
If either method is called multiple times, the token from the most recent successful call will take precedence.

```csharp
var codeToken = new CodeTokenRequestBody("{Code}", new Uri("{REDIR_URL_AFTER_LOGIN}"));
larkApi.GetUserTokenAsync(codeToken, cancellationToken);
```

The obtained token has an expiration period.
When necessary, call the following method to retrieve the latest token.
This method must be called while the current token is still valid;
otherwise, you will need to obtain a new token using one of the two methods described above.

```csharp
larkApi.RefreshTokenAsync(cancellationToken);
```

If the current token is still far from expiring, this method may return the same token as before the refresh.
Refer to the Lark documentation for the exact behavior.

You can also set a timer to call the above method to keep the token valid.

### Accessing resources

Once the token has been obtained, you can access resources in Lark.
Before doing so, ensure that the relevant app has the required permissions configured in the admin console.

The `LarkApi` type provides a set of built-in methods for making HTTPS requests.
The response body (`Data`) is returned as a JSON object and can also be deserialized into a specified type.

```csharp
public class LarkBaseTableForm
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    // Other fields are omitted here.
}
```

```csharp
public static Task<LarkResponseBody<LarkBaseTableForm>> GetBaseTableForm(this LarkApi larkApi, string nodeToken, string tableId, string formId, CancellationToken cancellationToken = default)
    => (larkApi ?? LarkApi.DefaultInstance).GetAsync(
        $"https://open.feishu.cn/open-apis/bitable/v1/apps/{nodeToken}/tables/{tableId}/forms/{formId}", // The Open API URL.
        "form", // The result data is in `form` property.
        cancellationToken);
```

```csharp
var form = await larkApi.GetBaseTableForm("{NODE_TOKEN}", "{TABLE_ID}", "{FORM_ID}", cancellationToken);
```

In the example above, retrieving the Lark Base table metadata uses a GET request, so the `GetAsync` method is called.
Similarly, for a POST request, use the corresponding `PostAsync` method and provide an additional request body of type `JsonObjectNode`.
For a PUT request, use the `PutAsync` method.

Some Lark Open API requests return a paginated list.
In such cases, you can use the following `GetItemsAsync` method.
It returns a `LarkResponsePagingBody` instance with result collection and page token.
The page token is used to get the next page result.

```csharp
public class LarkBaseTableField
{
    [JsonPropertyName("field_id")]
    public string Id { get; set; }

    [JsonPropertyName("field_name")]
    public string Name { get; set; }

    // Other fields are omitted here.
}
```

```csharp
public static Task<LarkResponsePagingBody<LarkBaseTableField>> ListBaseTableField(this LarkApi larkApi, string nodeToken, string tableId, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
    => (larkApi ?? LarkApi.DefaultInstance).GetItemsAsync(
        $"https://open.feishu.cn/open-apis/bitable/v1/apps/{nodeToken}/tables/{tableId}/fields", // The Open API URL.
        null,
        paging, // The page size and page token.
        cancellationToken);
```

```csharp
// Get first 50 items.
var firstPage = await larkApi.ListBaseTableField("{NODE_TOKEN}", "{TABLE_ID}", new(50), cancellationToken);
foreach (var item in firstPage.Data)
{
    // …
}

// Get next 50 items.
if (firstPage.HasMore)
{
    var paging = firstPage.NextPageInfo(50);
    var secondPage = await larkApi.ListBaseTableField("{NODE_TOKEN}", "{TABLE_ID}", paging, cancellationToken);
    foreach (var item in secondPage.Data)
    {
        // …
    }
}
```

### Docs APIs and Utils

The `LarkApi` type also provides wrappers for several commonly used APIs, including APIs for online documents, users, and recruitment.

For example, the following are some of the online docs related APIs.

- Wiki space and node
  - `GetWikiSpaceNodesAsync` List nodes under a specific wiki space or a parent node.
  - `GetWikiSpaceInfoAsync` Get wiki space info.
  - `GetWikiNodeAsync` Get the node info.
- Lark Base (former named Bitable)
  - `GetBaseTableAsync` Get base table information.
  - `ListBaseTableTablesAsync` List tables (sheets) of a base table instance.
  - `ReadBaseTableAsync` List records of the table (sheet) of a base table instance.
  - `UpdateBaseTableRecordAsync` Update the record of base table.
- File
  - `DownloadFileAsync` Download file.
  - `ReadDocsTextFileAsync` Get text format content of an online file in wiki.

And for online docs, it provides a set of tools to simplify accessing.

```csharp
// List all blocks of an online docs.
var blocks = await larkApi.GetDocsBlocksAsync("{NODE_TOKEN}", true, cancellationToken);
```

In Lark docs, blocks are the basic units that make up the content.
Blocks have different types depending on their content and may contain child blocks, forming a tree structure.

However, the data structure of a block is highly complex.
To obtain a more concise representation of its core structure—at the cost of losing styling and other details—you can call the `ToTree` extension method.
This is particularly useful when feeding the content into an LLM, as it helps reduce token usage.

```csharp
using LarkSuite.Docs;
```

```csharp
var contentTree = blocks.ToTree();
```

### Low-level API

```csharp
var http = larkApi.CreateJsonHttpClient<Model>();
var resp = await http.GetAsync(uri, cancellationToken);
```

It creates an instance of `JsonHttpClient` to access Open API with `bearer` authentication.

### On Message

You can listen events from Lark service such as user chat and status changed.

```csharp
larkApi.OnEventReceived("{MESSAGE_TYPE}", callback);
```

## Codes and build

It is implemented by C# and .NET 10.
The solution file is `LarkSuite.slnx`.

### Projects

- `OapiClient`: The core library and CLI. [(Link)](./OapiClient/)

### Build

You can open the solution file by Visual Studio to explore the codes and files.
Then restore the NuGet packages and build the projects.

You can also build it by following command line by latest [.NET SDK](https://dotnet.microsoft.com/zh-cn/download/dotnet/10.0).

```sh
dotnet build
```
