var endpointEnvironmentVariable = Environment.GetEnvironmentVariable("SEARCH_ENDPOINT") ?? "";
var apiKeyEnvironmentVariable = Environment.GetEnvironmentVariable("SEARCH_API_KEY") ?? "";

var indexName = "demo_index";

var credential = new AzureKeyCredential(apiKeyEnvironmentVariable);
var client = new SearchClient(new Uri(endpointEnvironmentVariable), indexName, credential);
