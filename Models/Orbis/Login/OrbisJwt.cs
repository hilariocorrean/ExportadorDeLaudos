using System.Text.Json.Serialization;

namespace ExportadorDeLaudos.Models.Orbis.Login;

public class OrbisJwt
{
	[JsonPropertyName("token")]
	public string? Token { get; set; }
}
