var settings = new ElasticsearchClientSettings(new Uri("https://localhost:9200"))
  .CertificateFingerprint(Environment.GetEnvironmentVariable("CERTIFICATE_FINGERPRINT") ?? string.Empty)
  .Authentication(new BasicAuthentication("elastic", Environment.GetEnvironmentVariable("ELASTIC_PASSWORD") ?? string.Empty))
  .DisablePing()
  .DisableDirectStreaming(true)
  .SniffOnStartup(false)
  .SniffOnConnectionFault(false);;

var client = new ElasticsearchClient(settings);

var indexName = $"demo-index-{Random.Shared.Next(0, 1000)}";

var jsonContent = File.ReadAllText("data.json");

var contentList = JsonSerializer.Deserialize<List<ContentItem>>(jsonContent) ?? new List<ContentItem>();

Console.WriteLine($"Uploading {contentList.Count} documents to the {indexName}...");

foreach (var item in contentList)
{
  await client.IndexAsync(item, indexName);
}

do {
  Console.ForegroundColor = ConsoleColor.Blue;
  Console.Write("Search for documents (type '' to exit): ");

  var search = Console.ReadLine() ?? string.Empty;

  if (search == "")
  {
    break;
  }

var response = await client.SearchAsync<ContentItem>(s => s
  .Index(indexName)
  .From(0)
  .Size(1000)
  .Query(q => q
    .Bool(b => b
      .Should(
        bs => bs.Match(m => m.Field(f => f.Title).Query(search.ToLowerInvariant().Trim())),
        bs => bs.Match(m => m.Field(f => f.CoverFilePath).Query(search.ToLowerInvariant().Trim()))
      )
    )
  )
);

  var totalCount = response.Documents.Count;

  Console.SetCursorPosition(search.Length + 40, Console.CursorTop - 1);
  Console.WriteLine($" (Total count: {totalCount})");

  foreach (var result in response.Documents)
  {
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"* {result.Title}");
  }
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
