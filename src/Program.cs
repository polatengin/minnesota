var endpointEnvironmentVariable = Environment.GetEnvironmentVariable("SEARCH_ENDPOINT") ?? "";
var adminKeyEnvironmentVariable = Environment.GetEnvironmentVariable("SEARCH_ADMIN_KEY") ?? "";

var indexName = $"demo-index-{Random.Shared.Next(0, 1000)}";

var credential = new AzureKeyCredential(adminKeyEnvironmentVariable);

var client = new SearchClient(new Uri(endpointEnvironmentVariable), indexName, credential);

var indexClient = new SearchIndexClient(new Uri(endpointEnvironmentVariable), credential);

var indexes = indexClient.GetIndexNames().Where(e => e == indexName).ToList();

if (indexes.Count == 0 || indexes.Exists(e => e != indexName))
{
  var index = new SearchIndex(indexName)
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

  var jsonContent = File.ReadAllText("data.json");

  var contentList = JsonSerializer.Deserialize<List<ContentItem>>(jsonContent) ?? new List<ContentItem>();

  Console.WriteLine($"Uploading {contentList.Count} documents to the {indexName}...");

  foreach (var item in contentList)
  {
    await client.IndexDocumentsAsync(IndexDocumentsBatch.Upload(new[] { item }));
  }
}

do {
  Console.ForegroundColor = ConsoleColor.Blue;
  Console.Write("Search for documents (type '' to exit): ");

  var search = Console.ReadLine() ?? string.Empty;

  if (search == "")
  {
    break;
  }

  SearchResults<SearchDocument> response = client.Search<SearchDocument>(search + "*", new SearchOptions { IncludeTotalCount = true, SearchMode = SearchMode.All, QueryType = SearchQueryType.Full });

  var totalCount = response.TotalCount;

} while (true);

public class ContentItem
{
  public string Id { get; set; } = string.Empty;
  public string CoverFileName { get; set; } = string.Empty;
  public string CoverFilePath { get; set; } = string.Empty;
  public string Title { get; set; } = string.Empty;
  public string Alias { get; set; } = string.Empty;
  public int DisplayOrder { get; set; }
  public bool Status { get; set; }
  public bool IsShowInPage { get; set; }
  public string ContentCategoryTypeName { get; set; } = string.Empty;
  public string Language { get; set; } = string.Empty;
}
