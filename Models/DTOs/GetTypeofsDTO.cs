namespace ExportadorDeLaudos.Models.DTOs
{
    public class GetTypeofsDTO
    {
        // DTO que recebe o retorno do método Typeofs/GET
        public Guid id { get; set; }
        public string? label { get; set; }
    }
}
