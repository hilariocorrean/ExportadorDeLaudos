using System.Text.Json.Serialization;

namespace ExportadorDeLaudos.Models.Orbis;

public class OrbisDocVersion
{
	[JsonPropertyName("filename")]
	public string FileName { get; set; }

	[JsonPropertyName("typeof")]
	public string TypeOf { get; set; }

	[JsonPropertyName("enable")]
	public int Enable { get; set; }

	[JsonPropertyName("status")]
	public int Status { get; set; }

	[JsonPropertyName("keywordversion")]
	public List<OrbisDocKeywordVersion> KeywordVersion { get; set; }
}