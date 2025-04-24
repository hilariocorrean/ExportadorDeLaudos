namespace ImportadorDeLaudos.Models.DTOs
{
    public class GetTypeofsKeywordsDTO
    {
        // DTO que recebe o retorno do método Keywordtypeofs/GET
        public Guid id { get; set; }
        public string? label { get; set; }
    }
}
