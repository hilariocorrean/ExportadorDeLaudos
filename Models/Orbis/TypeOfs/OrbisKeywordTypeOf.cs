using System.Text.Json.Serialization;

namespace ExportadorDeLaudos.Models.Orbis.TypeOfs;

public class OrbisKeywordTypeOf
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	[JsonPropertyName("order")]
	public int Order { get; set; }

	[JsonPropertyName("label")]
	public string? Label { get; set; }

	[JsonPropertyName("length")]
	public int Length { get; set; }

	[JsonPropertyName("type")]
	public int Type { get; set; }

	[JsonPropertyName("content")]
	public string? Content { get; set; }
}