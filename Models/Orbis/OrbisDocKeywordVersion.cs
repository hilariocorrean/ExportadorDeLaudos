using System.Text.Json.Serialization;

namespace ExportadorDeLaudos.Models.Orbis;

public class OrbisDocKeywordVersion
{
	[JsonPropertyName("keywordtypeof")]
	public string KeywordTypeOf { get; set; }

	[JsonPropertyName("enable")]
	public int Enable { get; set; }

	[JsonPropertyName("status")]
	public int Status { get; set; }

	[JsonPropertyName("value")]
	public string? Value { get; set; }
}