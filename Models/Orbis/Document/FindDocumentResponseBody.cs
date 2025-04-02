using System.Text.Json.Serialization;

namespace ExportadorDeLaudos.Models.Orbis.Document;

public class FindDocumentResponseBody
{
	[JsonPropertyName("from")]
	public int From { get; set; }

	[JsonPropertyName("nextFrom")]
	public int NextFrom { get; set; }

	[JsonPropertyName("total")]
	public int Total { get; set; }

	[JsonPropertyName("totalPages")]
	public int TotalPages { get; set; }

	[JsonPropertyName("size")]
	public int Size { get; set; }

	[JsonPropertyName("documents")]
	public List<OrbisDocument>? Documents { get; set; }
}
