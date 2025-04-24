using ImportadorDeLaudos.Models;

namespace ImportadorDeLaudos.Contracts
{
    public interface IRelatorioAtualizadoRepository
    {
        RelatorioAtualizado GetRelatorioAtualizadoByAnoAndProtocoloReqNr(double ANO, double PROTOCOLO_REQ_NR);
    }
}
