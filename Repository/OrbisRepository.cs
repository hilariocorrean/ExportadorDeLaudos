using ExportadorDeLaudos.Contracts;
using ExportadorDeLaudos.Models.Orbis;
using ExportadorDeLaudos.Models.Orbis.Document;
using ExportadorDeLaudos.Models.Orbis.Login;
using ExportadorDeLaudos.Models.Orbis.TypeOfs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ExportadorDeLaudos.Repository;

public class OrbisRepository : IOrbisRepository
{
	private readonly HttpClient _httpClient;

	public OrbisRepository(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<List<OrbisDocument>> FindDocumentAsync(string documentTypeId, Dictionary<string, string> searchValues, string authToken)
	{
		if (_httpClient.BaseAddress == null || _httpClient.BaseAddress.ToString() == "")
		{
			throw new Exception("Verifique a configuração da URL Orbis");
		}

		var body = new
		{
			From = 0,
			Size = 10,
			IndexName = documentTypeId,
			Operators = new List<object>()
		};

		foreach (var (key, value) in searchValues)
		{
			body.Operators.Add(new
			{
				PropertyName = key,
				Operator = 1,
				Type = 1,
				FirstValue = value
			});
		}

		var requestBody = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
		var response = await _httpClient.PostAsync("Documents/Find", requestBody);
		response.EnsureSuccessStatusCode();

		var responseContent = await response.Content.ReadAsStringAsync();
		var documentsResponseBody = JsonSerializer.Deserialize<FindDocumentResponseBody>(responseContent);

		var documents = documentsResponseBody?.Documents ?? new List<OrbisDocument>();

		return documents;
	}

	public async Task<List<OrbisDocument>> FindDocumentByTermAsync(string term, string authToken)
	{
		if (_httpClient.BaseAddress == null || _httpClient.BaseAddress.ToString() == "")
		{
			throw new Exception("Verifique a configuração da URL Orbis");
		}

		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
		var response = await _httpClient.GetFromJsonAsync<FindDocumentResponseBody>($"Documents/Findterm?term={term}&from=0&size=50");
		var documents = response?.Documents ?? new List<OrbisDocument>();

		return documents;
	}

	public async Task<string> GetDownloadUrlAsync(string documentId, string authToken)
	{
		if (_httpClient.BaseAddress == null || _httpClient.BaseAddress.ToString() == "")
		{
			throw new Exception("Verifique a configuração da URL Orbis");
		}

		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

		var response = await _httpClient.GetAsync($"Documents/DownloadUrl/{documentId}");
		response.EnsureSuccessStatusCode();

		var downloadUrl = JsonSerializer.Deserialize<string>(await response.Content.ReadAsStringAsync()) ?? "";

		return downloadUrl;
	}

	public async Task<List<OrbisKeywordTypeOf>> GetKeywordsTypeOfAsync(string typeOfId, string authToken)
	{
		if (_httpClient.BaseAddress == null || _httpClient.BaseAddress.ToString() == "")
		{
			throw new Exception("Verifique a configuração da URL Orbis");
		}

		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
		var orbisKeywords = await _httpClient.GetFromJsonAsync<List<OrbisKeywordTypeOf>>($"typeofs/keywords?guid={typeOfId}");

		return orbisKeywords ?? new List<OrbisKeywordTypeOf>();
	}

	public async Task<string> GetLoginTokenAsync(string username, string password)
	{
		if (_httpClient.BaseAddress == null || _httpClient.BaseAddress.ToString() == "")
		{
			throw new Exception("Verifique a configuração da URL Orbis");
		}

		var credentials = new
		{
			login = username,
			password = password
		};

		var requestBody = new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, "application/json");

		var response = await _httpClient.PostAsync("login", requestBody);

		response.EnsureSuccessStatusCode();

		var responseContent = await response.Content.ReadAsStringAsync();
		OrbisJwt orbisJwt = JsonSerializer.Deserialize<OrbisJwt>(responseContent) ?? new OrbisJwt();

		return orbisJwt.Token ?? "";
	}

	public async Task<(string, bool)> UploadDocumentAsync(FormFile document, OrbisDocProperties docProperties, string authToken)
	{
		if (_httpClient.BaseAddress == null || _httpClient.BaseAddress.ToString() == "")
		{
			throw new Exception("Verifique a configuração da URL Orbis");
		}

		var properties = new
		{
			Document = docProperties
		};

		var content = new MultipartFormDataContent
		{
			{ new StreamContent(document.OpenReadStream()), "File", document.FileName },
			{ new StringContent(JsonSerializer.Serialize(properties), Encoding.UTF8, "application/json"), "json" }
		};

        //foreach (var part in content)
        //{
        //    if (part is StringContent stringContent)
        //    {
        //        var jsonText = stringContent.ReadAsStringAsync().Result;
        //    }
        //}

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
		var response = await _httpClient.PostAsync("Documents/UploadFile", content);
		//response.EnsureSuccessStatusCode(); // desabilitar

		var responseContent = await response.Content.ReadAsStringAsync();
        var isSuccess = response.IsSuccessStatusCode;

        return (responseContent, isSuccess);
	}
}