var endpointEnvironmentVariable = Environment.GetEnvironmentVariable("SEARCH_ENDPOINT") ?? "";
var adminKeyEnvironmentVariable = Environment.GetEnvironmentVariable("SEARCH_ADMIN_KEY") ?? "";

var indexName = "demo_index";

var credential = new AzureKeyCredential(adminKeyEnvironmentVariable);
var indexClient = new SearchIndexClient(new Uri(endpointEnvironmentVariable), credential);

var client = new SearchClient(new Uri(endpointEnvironmentVariable), indexName, credential);
