using System.Text.Json.Serialization;

namespace Contract;

public class QueryResult<TDomain>
{
    [JsonPropertyName("items")]
    public IEnumerable<TDomain> Items { get; set; } = [];

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }
}