var endpointEnvironmentVariable = Environment.GetEnvironmentVariable("SEARCH_ENDPOINT") ?? "";
var adminKeyEnvironmentVariable = Environment.GetEnvironmentVariable("SEARCH_ADMIN_KEY") ?? "";

var indexName = $"demo-index-{Random.Shared.Next(0, 1000)}";

var credential = new AzureKeyCredential(adminKeyEnvironmentVariable);
var indexClient = new SearchIndexClient(new Uri(endpointEnvironmentVariable), credential);

var index = (await indexClient.GetIndexAsync(indexName)).Value;

if (index == null)
{
  index = new SearchIndex(indexName)
  {
    Fields =
    {
      new SimpleField("id", SearchFieldDataType.String) { IsKey = true, IsFilterable = true, IsSortable = true },
        new SimpleField("idForSort", SearchFieldDataType.Int64) { IsSortable = true },
        new SearchableField("alias") { IsFilterable = true, AnalyzerName = LexicalAnalyzerName.EnLucene },
        new SimpleField("contentCategoryType", SearchFieldDataType.Int32) { IsFilterable = true },
        new SimpleField("contentCategoryTypeName", SearchFieldDataType.String),
        new SimpleField("countryId", SearchFieldDataType.Int32) { IsFilterable = true },
        new SimpleField("countryName", SearchFieldDataType.String),
        new SimpleField("cityName", SearchFieldDataType.String) { IsFilterable = true },
        new SimpleField("coverFileName", SearchFieldDataType.String),
        new SimpleField("coverFilePath", SearchFieldDataType.String),
        new SimpleField("displayOrder", SearchFieldDataType.Int32) { IsSortable = true },
        new SimpleField("seoUrl", SearchFieldDataType.String),
        new SimpleField("shortDescription", SearchFieldDataType.String),
        new SimpleField("status", SearchFieldDataType.Boolean) { IsFilterable = true },
        new SearchableField("title") { IsFilterable = true, IsSortable = true, AnalyzerName = LexicalAnalyzerName.EnLucene },
        new SimpleField("treatmentId", SearchFieldDataType.Int64) { IsFilterable = true },
        new SimpleField("treatmentName", SearchFieldDataType.String),
        new SimpleField("isHighlightContent", SearchFieldDataType.Boolean) { IsFilterable = true },
        new SimpleField("isShowInPage", SearchFieldDataType.Boolean) { IsFilterable = true, IsFacetable = true },
        new SimpleField("language", SearchFieldDataType.String),
        new SimpleField("categoryId", SearchFieldDataType.Int64) { IsFilterable = true },
        new SimpleField("categoryName", SearchFieldDataType.String),
        new SimpleField("countrySeoName", SearchFieldDataType.String) { IsFilterable = true },
        new SimpleField("citySeoName", SearchFieldDataType.String) { IsFilterable = true }
    }
  };

  index.Suggesters.Add(new SearchSuggester("sg", new List<string> { "title" }));

  await indexClient.CreateIndexAsync(index);
}

var client = new SearchClient(new Uri(endpointEnvironmentVariable), indexName, credential);

SearchResults<SearchDocument> response = client.Search<SearchDocument>("facelift");
foreach (var result in response.GetResults().Select(e => e.Document))
{
  Console.WriteLine($"{result.GetString("title")}");
}
