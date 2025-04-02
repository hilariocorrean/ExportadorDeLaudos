using System.Text.Json.Serialization;

namespace ExportadorDeLaudos.Models.Orbis.Document;

public class OrbisDocumentKey
{
	[JsonPropertyName("label")]
	public string? Label { get; set; }

	[JsonPropertyName("value")]
	public string? Value { get; set; }
}