using System.Text.Json.Serialization;

namespace ExportadorDeLaudos.Models.Orbis;

public class OrbisDocProperties
{
	[JsonPropertyName("enable")]
	public int Enable { get; set; }

	[JsonPropertyName("status")]
	public int Status { get; set; }

	[JsonPropertyName("typeoflastversion")]
	public string TypeOfLastVersion { get; set; }

	[JsonPropertyName("version")]
	public List<OrbisDocVersion> Version { get; set; }
}


