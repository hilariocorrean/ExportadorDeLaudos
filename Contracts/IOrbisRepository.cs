using ImportadorDeLaudos.Models.Orbis;
using ImportadorDeLaudos.Models.Orbis.Document;
using ImportadorDeLaudos.Models.Orbis.TypeOfs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace ImportadorDeLaudos.Contracts;

public interface IOrbisRepository
{
	Task<string> GetLoginTokenAsync(string username, string password);
	Task<List<OrbisDocument>> FindDocumentAsync(string documentTypeId, Dictionary<string, string> searchValues, string authToken);
	Task<List<OrbisDocument>> FindDocumentByTermAsync(string term, string authToken);
	Task<(string, bool)> UploadDocumentAsync(FormFile document, OrbisDocProperties docProperties, string authToken);
	Task<string> GetDownloadUrlAsync(string documentId, string authToken);
	Task<List<OrbisKeywordTypeOf>> GetKeywordsTypeOfAsync(string typeOfId, string authToken);
}
