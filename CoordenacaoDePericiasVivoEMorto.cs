using ImportadorDeLaudos.Contracts;
using ImportadorDeLaudos.Models;
using ImportadorDeLaudos.Models.Orbis;
using ImportadorDeLaudos.Repository;
using ImportadorDeLaudos.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace ImportadorDeLaudos
{
    public partial class CoordenacaoDePericiasVivoEMortoForm : Form
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IRelatorioAtualizadoRepository relatorioAtualizadoRepository;
        private readonly IOrbisRepository orbisRepository;
        private readonly string admUser;
        private readonly string admPass;

        public CoordenacaoDePericiasVivoEMortoForm(IConfigurationRoot configuration, HttpClient _httpClient)
        {
            _configuration = configuration;
            InitializeComponent();
            relatorioAtualizadoRepository = new RelatorioAtualizadoRepository(configuration);
            orbisRepository = new OrbisRepository(_httpClient);
            admUser = _configuration["OrbisSettings:AdmUser"]!;
            admPass = _configuration["OrbisSettings:AdmPass"]!;
        }


        private void ButtonSelectFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFolder = folderBrowserDialog.SelectedPath;
                listFilePaths.Items.Clear(); 

                List<string> filePaths = Directory.GetFiles(selectedFolder, "*.pdf", SearchOption.AllDirectories).ToList();
                // LÛgica de pattern matching para eliminar os que fogem do padr„o
                Regex laudoVivoNamePattern = new Regex(@"^[0-9]{6}\.pdf$", RegexOptions.Compiled);
                Regex laudoMortoNamePattern = new Regex(@"^[a-zA-Z·¡È…ÌÕÛ”˙⁄‡¿Ë»ÏÃÚ“˘Ÿ‚¬Í ÓŒÙ‘˚€„√ı’Á«'\s]+\.pdf$", RegexOptions.Compiled);
                Regex laudoSiplNamePattern = new Regex(@"^[0-9]{4}\-[0-9]+\=[a-zA-Z·¡È…ÌÕÛ”˙⁄‡¿Ë»ÏÃÚ“˘Ÿ‚¬Í ÓŒÙ‘˚€„√ı’Á«']+\.pdf$", RegexOptions.Compiled);

                foreach (var filePath in filePaths)
                {
                    var fileName = Path.GetFileName(filePath);

                    if (laudoVivoNamePattern.IsMatch(fileName) || laudoMortoNamePattern.IsMatch(fileName) || laudoSiplNamePattern.IsMatch(fileName))
                    {
                        listFilePaths.Items.Add(filePath);
                    }
                }

                ResetComboReportTypeState();
                UpdateSendToOrbisButtonState();
            }
        }

        // Em princÌpio, vai ser preciso que o mÈtodo seja especÌfico e hard-coded para cada tipo documental. Retorna a lista de
        // Ìndices possÌveis para o tipo documental em quest„o.
        // TODO: Adicionar ‡ lista de argumentos a lista de keywords retornadas do GetKeywordsTypeOfsAsync, de modo que
        // KeywordTypeOf = Id e Value È associado a uma Label retornada.
        private List<OrbisDocKeywordVersion> GetOrbisDocKeywordVersionsList(string modalidadeDeLaudos, string protocolo, string nome, double? ano, string numeroLaudo)
        {
            var keywordVersionList = new List<OrbisDocKeywordVersion>();

            var modalidadeDeLaudosKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "d6e52ab8-66a4-471d-8a29-2a4e9f315793",
                Enable = 1,
                Status = 1,
                Value = modalidadeDeLaudos
            };
            var protocoloKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "a0f59e97-39ab-4b8d-92b7-368b88ae38de",
                Enable = 1,
                Status = 1,
                Value = protocolo
            };
            var noLaudoKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "f880e01d-a15b-4b76-88d9-5c5cd20e0b00",
                Enable = 1,
                Status = 1,
                Value = numeroLaudo
            };
            var nomeKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "f2b32dcd-427c-4afd-9180-1dfa4c80fa44",
                Enable = 1,
                Status = 1,
                Value = nome
            };
            var anoKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "eff6e4cb-99ca-4909-a9c6-968786d30dbc",
                Enable = 1,
                Status = 1,
                Value = ano is null ? String.Empty : ((int)ano!).ToString()
            };

            keywordVersionList.Add(modalidadeDeLaudosKeyword);
            keywordVersionList.Add(protocoloKeyword);
            keywordVersionList.Add(noLaudoKeyword);
            keywordVersionList.Add(nomeKeyword);
            keywordVersionList.Add(anoKeyword);

            return keywordVersionList;
        }

        private List<OrbisDocVersion> GetOrbisDocVersionsList(string fileName, string typeOfId, List<OrbisDocKeywordVersion> orbisDocKeywordVersions)
        {
            var listOrbisDocVersions = new List<OrbisDocVersion>();

            var version = new OrbisDocVersion
            {
                FileName = fileName,
                TypeOf = typeOfId,
                Enable = 1,
                Status = 1,
                KeywordVersion = orbisDocKeywordVersions
            };

            listOrbisDocVersions.Add(version);
            return listOrbisDocVersions;
        }

        private OrbisDocProperties GetOrbisDocProperties(string typeOf, List<OrbisDocVersion> orbisDocVersions)
        {
            var orbisDocProperties = new OrbisDocProperties
            {
                Enable = 1,
                Status = 1,
                TypeOfLastVersion = typeOf,
                Version = orbisDocVersions
            };

            return orbisDocProperties;
        }

        private async void ButtonSendToOrbis_Click(object sender, EventArgs e)
        {
            try
            {
                string reportType = comboReportType.SelectedItem!.ToString()!;
                double? yearAsNullableDouble = (double)year.Value;
                var maxFilesAsDouble = (double)maxFiles.Value;

                // Uma chamada para cada arquivo na lista. O ano È constante, o n˙mero m·ximo de arquivos È constante mas o n˙mero de
                // protocolo precisa ser atualizado a cada registro dentro do loop.
                var unsuccessfulFilePathsList = new List<string>();
                var fileCounter = 0;

                var processingRequestForm = new ProcessingRequestForm();
                processingRequestForm.Show();

                foreach (string filePath in listFilePaths.Items)
                {
                    if (fileCounter < (int)maxFilesAsDouble)
                    {
                        double protocoloReqNr; // O QUE … USADO PARA ACHAR NO BANCO
                        string nomeCidadao;
                        string protocoloGerado; // AUXILIAR (PARA PADRONIZA«√O DO FORMATO)
                        string protocolo; // O QUE REALMENTE … ENVIADO
                        var relatorioAtualizado = new RelatorioAtualizado();
                        string fileName = Path.GetFileName(filePath);
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                        string noLaudo;

                        switch(reportType)
                        {
                            case "PERÕCIA NO VIVO":
                                // Caso exista entrada correspondente, o nome È buscado. Como È um Ìndice opcional que a PCEPA sugeriu que
                                // nem fosse usado para o laudo vivo, foda-se. FODA-SE O CARALHO, tinha esquecido que essa porra È um ponteiro,
                                // ent„o, se a entrada n„o bate, o mÈtodo ToUpper() È chamado num ponteiro nulo.
                                protocoloReqNr = double.Parse(fileNameWithoutExtension);
                                relatorioAtualizado = relatorioAtualizadoRepository.GetRelatorioAtualizadoByAnoAndProtocoloReqNr((double)yearAsNullableDouble!, protocoloReqNr);
                                nomeCidadao = relatorioAtualizado.NOME is null ? string.Empty : relatorioAtualizado.NOME.ToUpper();
                                // N„o havendo registro, presume que È da regional de BelÈm.
                                protocoloGerado = year.Value.ToString() + ".01." + protocoloReqNr.ToString().PadLeft(6, '0');
                                protocolo = relatorioAtualizado.PROTOCOLO is null ? protocoloGerado : relatorioAtualizado.PROTOCOLO;
                                noLaudo = String.Empty;
                                break;
                            case "PERÕCIA NO MORTO":
                                // Caso exista entrada correspondente, o n˙mero de protocolo È buscado. Como È um Ìndice opcional que a PCEPA sugeriu que
                                // nem fosse usado para o laudo morto, foda-se.
                                nomeCidadao = fileNameWithoutExtension.ToUpper();
                                relatorioAtualizado = relatorioAtualizadoRepository.GetRelatorioAtualizadoByAnoAndNome((double)yearAsNullableDouble!, nomeCidadao);                                
                                protocoloReqNr = relatorioAtualizado.PROTOCOLO_REQ_NR;
                                // N„o havendo registro, presume que È da regional de BelÈm.
                                protocoloGerado = year.Value.ToString() + ".01." + protocoloReqNr.ToString().PadLeft(6, '0');
                                protocolo = relatorioAtualizado.PROTOCOLO is null ? protocoloGerado : relatorioAtualizado.PROTOCOLO;
                                noLaudo = String.Empty;
                                break;
                            case "LAUDOS SIPL":
                                // Caso dos Laudos.Sipl. S„o laudos de perÌcia no vivo cujo nome È no formato
                                // <ano>-<nro de laudo>=<primeiro nome da pessoa>.pdf
                                var nameTokens = fileNameWithoutExtension.Split(['-', '=']);
                                nomeCidadao = nameTokens[2].ToUpper();
                                protocolo = String.Empty;
                                noLaudo = nameTokens[1];
                                break;
                            default:
                                // Caso dos laudos mortos da pasta Sipl. Ao contr·rio dos vivos, n„o precisa de um tipo adicional.
                                // Em compensaÁ„o, n„o pode usar o lookup na tabela e nem tem n˙mero de protocolo associado.
                                // AlÈm disso, a organizaÁ„o n„o È por ano, e o ano deve ser ignorado/mandado em branco.
                                nomeCidadao = fileNameWithoutExtension.ToUpper();
                                protocolo = String.Empty;
                                noLaudo = String.Empty;
                                yearAsNullableDouble = null; // Gambiarra pro ano ficar em branco tendo que alterar o mÌnimo possÌvel.
                                // A modalidade PERÕCIA NO MORTO (SIPL) sÛ existe no programa, a modalidade no orbis È a mesma dos demais laudos mortos.
                                reportType = "PERÕCIA NO MORTO";
                                break;
                        }                        

                        var token = await orbisRepository.GetLoginTokenAsync(admUser, admPass);
                        // Por enquanto, vou pular o passo de levantar o Ìndice (keyword) a partir do tipo documental (typeof).
                        // Tipo documental: CoordenaÁ„o de perÌcias vivo ou morto
                        var typeOf = "5d937fae-6564-491b-bc3b-4385bd2de9de";
                        //var testeKeywordsTypeOf = await orbisRepository.GetKeywordsTypeOfAsync(typeOf, token);

                        // Preparo dos objetos aninhados (doc keyword version, doc version e doc properties)
                        var orbisDocKeywordVersions = GetOrbisDocKeywordVersionsList(reportType, protocolo, nomeCidadao, yearAsNullableDouble, noLaudo);
                        var orbisDocVersions = GetOrbisDocVersionsList(fileName, typeOf, orbisDocKeywordVersions);
                        var orbisDocProperties = GetOrbisDocProperties(typeOf, orbisDocVersions);

                        // Preparo do objeto do documento a ser enviado em Documents/UploadFile
                        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        var document = new FormFile(fileStream, 0, fileStream.Length, "File", fileName) //VER SE O NOME ESPERADO … file MESMO
                        {
                            Headers = new HeaderDictionary(),
                            ContentType = "application/pdf"
                        };

                        // Chamada a Documents/UploadFile SE houver entrada correspondente na tabela
                        string responseAsString = String.Empty;
                        bool isSuccess = false;
                        (responseAsString, isSuccess) = await orbisRepository.UploadDocumentAsync(document, orbisDocProperties, token);


                        // Tratamento pÛs-retorno da API
                        fileStream.Close();
                        if (isSuccess)
                        {
                            string directoryPath = Path.GetDirectoryName(filePath)!;
                            string newFileName = "OK_" + fileName;
                            string newFilePath = Path.Combine(directoryPath, newFileName);
                            try
                            {
                                File.Move(filePath, newFilePath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Erro renomeando o arquivo:\n {ex.Message}\n {ex.StackTrace}");
                                Application.Exit();
                            }
                        }
                        else
                        {
                            unsuccessfulFilePathsList.Add(filePath);
                        }
                        fileCounter++;
                    }
                }

                while (fileCounter > 0)
                {
                    listFilePaths.Items.RemoveAt(0); // Elimina os arquivos em ordem
                    fileCounter--;
                }
                foreach (string unsuccessfulFilePath in unsuccessfulFilePathsList)
                {
                    listFilePaths.Items.Add(unsuccessfulFilePath);
                }

                processingRequestForm.Close();
            }
            catch (Exception ex)
            {
                WindowCopyableException.ShowException(ex);
                Application.Exit();
            }
        }

        private void ComboReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboReportType.SelectedItem!.ToString() == "PERÕCIA NO MORTO (SIPL)")
            {
                labelYear.Enabled = false;
                year.Enabled = false;
            }
            else
            {
                labelYear.Enabled = true;
                year.Enabled = true;
            }

            UpdateSendToOrbisButtonState();
        }

        private void ResetComboReportTypeState()
        {
            comboReportType.SelectedItem = null;
            comboReportType.Text = "Selecionar...";
        }

        private void UpdateSendToOrbisButtonState()
        {
            // Libera o bot„o de envio se houver ao menos um arquivo na lista e se tiver um tipo definido de laudo
            buttonSendToOrbis.Enabled = listFilePaths.Items.Count > 0 && comboReportType.SelectedIndex != -1; //mudar o segundo pra 'is not null'?
        }
    }
}
