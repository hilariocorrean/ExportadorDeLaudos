using System.ComponentModel.DataAnnotations;

namespace ExportadorDeLaudos.Models
{
    public class RelatorioAtualizado
    {
        [Required]
        [Key]
        public int ID { get; set; }
        public double ANO { get; set; }
        public double REG_ID { get; set; }
        [MaxLength(255)]
        public string? REGIONAL { get; set; }
        public double PROTOCOLO_REQ_NR { get; set; }
        [MaxLength(255)]
        public string? NOME { get; set; }
        [MaxLength(255)]
        public string? PROTOCOLO { get; set; }
        [MaxLength(255)]
        public string? EXAME { get; set; }
        [MaxLength(255)]
        public string? STATUS { get; set; }
        [MaxLength(255)]
        public string? LAUDO_PERITOS { get; set; }
    }
}
