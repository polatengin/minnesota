using Azure;
using Azure.Search.Documents;

var endpointEnvironmentVariable = Environment.GetEnvironmentVariable("SEARCH_ENDPOINT") ?? "";
var apiKeyEnvironmentVariable = Environment.GetEnvironmentVariable("SEARCH_API_KEY") ?? "";

