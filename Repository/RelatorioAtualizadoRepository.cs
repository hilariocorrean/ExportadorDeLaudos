using ExportadorDeLaudos.Contracts;
using ExportadorDeLaudos.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ExportadorDeLaudos.Repository
{
    public class RelatorioAtualizadoRepository : IRelatorioAtualizadoRepository
    {
        private readonly string connectionString = "Exemplo de connection string que funcionaria pro nosso banco";
        
        public RelatorioAtualizado GetRelatorioAtualizadoByAnoAndProtocoloReqNr(double ANO, double PROTOCOLO_REQ_NR)
        {
            try
            {
                var relatorioAtualizado = new RelatorioAtualizado();

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * " +
                                   "FROM dbo.PCPA_LAUDOS.RELATORIO_ATUALIZADO " +
                                   "WHERE ANO = @ANO AND PROTOCOLO_REQ_NR = @PROTOCOLO_REQ_NR";

                    using (var command = new SqlCommand(query, connection))
                    {
                        // Adding parameters to avoid SQL injection
                        command.Parameters.AddWithValue("@ANO", ANO);
                        command.Parameters.AddWithValue("@PROTOCOLO_REQ_NR", PROTOCOLO_REQ_NR);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                relatorioAtualizado = new RelatorioAtualizado //verificar o método de parse
                                {
                                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                    ANO = reader.GetDouble(reader.GetOrdinal("ANO")),
                                    REG_ID = reader.GetDouble(reader.GetOrdinal("REG_ID")),
                                    REGIONAL = reader.GetString(reader.GetOrdinal("REGIONAL")),
                                    PROTOCOLO_REQ_NR = reader.GetDouble(reader.GetOrdinal("PROTOCOLO_REQ_NR")),
                                    NOME = reader.GetString(reader.GetOrdinal("NOME")),
                                    PROTOCOLO = reader.GetString(reader.GetOrdinal("PROTCOLO")),
                                    EXAME = reader.GetString(reader.GetOrdinal("EXAME")),
                                    STATUS = reader.GetString(reader.GetOrdinal("STATUS")),
                                    LAUDO_PERITOS = reader.GetString(reader.GetOrdinal("LAUDO_PERITOS")),
                                };
                            }
                        }
                    }
                }

                return relatorioAtualizado;
            }
            catch
            {
                throw;
            }
        }
    }
}
