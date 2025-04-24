using System.Text.Json.Serialization;

namespace ImportadorDeLaudos.Models.Orbis.Document;

public class OrbisDocumentKey
{
	[JsonPropertyName("label")]
	public string? Label { get; set; }

	[JsonPropertyName("value")]
	public string? Value { get; set; }
}