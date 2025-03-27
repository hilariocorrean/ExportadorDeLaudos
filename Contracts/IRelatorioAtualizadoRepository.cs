using ExportadorDeLaudos.Models;

namespace ExportadorDeLaudos.Contracts
{
    public interface IRelatorioAtualizadoRepository
    {
        RelatorioAtualizado GetRelatorioAtualizadoByAnoAndProtocoloReqNr(double ANO, double PROTOCOLO_REQ_NR);
    }
}
