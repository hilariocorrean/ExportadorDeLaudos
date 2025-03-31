using ExportadorDeLaudos.Contracts;
using ExportadorDeLaudos.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ExportadorDeLaudos.Repository
{
    public class RelatorioAtualizadoRepository : IRelatorioAtualizadoRepository
    {
        private readonly string _connectionString;
        //private readonly string teste = _configuration.GetConnectionString("PCPALaudos");

        public RelatorioAtualizadoRepository(IConfiguration configuration)
        {
            // Retrieve the connection string from the appsettings.json
            _connectionString = configuration.GetConnectionString("PCPALaudos")!;
        }

        public RelatorioAtualizado GetRelatorioAtualizadoByAnoAndProtocoloReqNr(double ANO, double PROTOCOLO_REQ_NR)
        {
            try
            {
                var relatorioAtualizado = new RelatorioAtualizado();

                using (var connection = new SqlConnection(_connectionString))
                {
                    //connection.CommandTimeout = 10;
                    connection.Open();
                    string query = "SELECT * " +
                                   "FROM dbo.RELATORIO_ATUALIZADO " +
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
                                    PROTOCOLO = reader.GetString(reader.GetOrdinal("PROTOCOLO")),
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
                return null;
            }
        }
    }
}
