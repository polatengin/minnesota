using Azure;
using Azure.Search.Documents;

var endpointEnvironmentVariable = Environment.GetEnvironmentVariable("SEARCH_ENDPOINT") ?? "";
