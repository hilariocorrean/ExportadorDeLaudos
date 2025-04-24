using System.Text.Json.Serialization;

namespace ImportadorDeLaudos.Models.Orbis.Document;

public class OrbisDocument
{
	public string? Id { get; set; }

	[JsonPropertyName("Lastversion")]
	public string? LastVersion { get; set; }

	public int Status { get; set; }

	[JsonPropertyName("typeOfId")]
	public string? TypeOfId { get; set; }

	[JsonPropertyName("fileName")]
	public string? FileName { get; set; }

	[JsonPropertyName("keys")]
	public List<OrbisDocumentKey>? Keys { get; set; }

	[JsonPropertyName("typeOfName")]
	public string? TypeOfName { get; set; }
}
