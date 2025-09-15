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
    public partial class CoordenacaoDePericiasNoMortoLaudosForm : Form
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IRelatorioAtualizadoRepository relatorioAtualizadoRepository;
        private readonly IOrbisRepository orbisRepository;
        private readonly string admUser;
        private readonly string admPass;

        public CoordenacaoDePericiasNoMortoLaudosForm(IConfigurationRoot configuration, HttpClient _httpClient)
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
                Regex laudoMortoNamePattern = new Regex(@"^[a-zA-Z·¡È…ÌÕÛ”˙⁄‡¿Ë»ÏÃÚ“˘Ÿ‚¬Í ÓŒÙ‘˚€„√ı’Á«'\s]+\.pdf$", RegexOptions.Compiled);

                foreach (var filePath in filePaths)
                {
                    var fileName = Path.GetFileName(filePath);

                    if (laudoMortoNamePattern.IsMatch(fileName))
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
        private List<OrbisDocKeywordVersion> GetOrbisDocKeywordVersionsList(string modalidadeCpm, double? ano, string nome, string numeroLaudo, string protocolo)
        {
            var keywordVersionList = new List<OrbisDocKeywordVersion>();

            var modalidadeCpmKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "0198a3ad-8d9e-77b0-97c6-b400dd1ee93e",
                Enable = 1,
                Status = 1,
                Value = modalidadeCpm
            };
            var anoKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "0198a3ad-8d9f-7d15-89d2-f27c07497df1",
                Enable = 1,
                Status = 1,
                Value = ano is null ? String.Empty : ((int)ano!).ToString()
            };
            var nomeKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "0198a3ad-8d9f-7906-86fe-6e2c95026d29",
                Enable = 1,
                Status = 1,
                Value = nome
            };
            var noLaudoKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "0198a3ad-8d9f-78e7-b72d-cbbaf7e70ad0",
                Enable = 1,
                Status = 1,
                Value = numeroLaudo
            };
            var protocoloKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "0198a3ad-8d9f-7bb4-9c10-33897c0fcfc6",
                Enable = 1,
                Status = 1,
                Value = protocolo
            };                                  

            keywordVersionList.Add(modalidadeCpmKeyword);
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

                        switch(comboReportType.SelectedItem!.ToString()!) //N„o pode ser por reportType porque ele È alterado no laudo morto sipl
                        {                            
                            case "PERÕCIA NO MORTO":
                                // Caso exista entrada correspondente, o n˙mero de protocolo È buscado. Como È um Ìndice opcional que a PCEPA sugeriu que
                                // nem fosse usado para o laudo morto, foda-se.
                                reportType = comboReportType.SelectedItem.ToString()!; // HorrÌvel, mas È um jeito suficientemente simples
                                nomeCidadao = fileNameWithoutExtension.ToUpper();
                                relatorioAtualizado = relatorioAtualizadoRepository.GetRelatorioAtualizadoByAnoAndNome((double)yearAsNullableDouble!, nomeCidadao);                                
                                protocoloReqNr = relatorioAtualizado.PROTOCOLO_REQ_NR;
                                // N„o havendo registro, presume que È da regional de BelÈm.
                                protocoloGerado = year.Value.ToString() + ".01." + protocoloReqNr.ToString().PadLeft(6, '0');
                                protocolo = relatorioAtualizado.PROTOCOLO is null ? protocoloGerado : relatorioAtualizado.PROTOCOLO;
                                noLaudo = String.Empty;
                                break;
                            case "PERÕCIA NO MORTO (SEM ANO)":
                                // Caso dos documentos mais recentes (09/2025). Mesmo sistema dos antigos n„o-sipl, mas sem divis„o de
                                // ano por pastas.
                                reportType = "PERÕCIA NO MORTO"; // HorrÌvel, mas È um jeito suficientemente simples                                
                                nomeCidadao = fileNameWithoutExtension.ToUpper();
                                protocolo = String.Empty;
                                noLaudo = String.Empty;
                                yearAsNullableDouble = null;
                                break;
                            default:
                                // Caso dos laudos mortos da pasta Sipl. Ao contr·rio dos vivos, n„o precisa de um tipo adicional.
                                // Em compensaÁ„o, n„o pode usar o lookup na tabela e nem tem n˙mero de protocolo associado.
                                // AlÈm disso, a organizaÁ„o n„o È por ano, e o ano deve ser ignorado/mandado em branco.
                                // A modalidade PERÕCIA NO MORTO (SIPL) sÛ existe no programa, a modalidade no orbis È a mesma dos demais laudos mortos.
                                reportType = "PERÕCIA NO MORTO"; // HorrÌvel, mas È um jeito suficientemente simples
                                nomeCidadao = fileNameWithoutExtension.ToUpper();
                                protocolo = String.Empty;
                                noLaudo = String.Empty;
                                yearAsNullableDouble = null; // Gambiarra pro ano ficar em branco tendo que alterar o mÌnimo possÌvel.
                                break;
                        }                        

                        var token = await orbisRepository.GetLoginTokenAsync(admUser, admPass);
                        // Por enquanto, vou pular o passo de levantar o Ìndice (keyword) a partir do tipo documental (typeof).
                        // Tipo documental: COORDENA«√O DE PERÕCIAS NO MORTO (CPM) LAUDOS
                        var typeOf = "0198a3a7-cc08-713d-bd48-a6352520ace3";
                        //var testeKeywordsTypeOf = await orbisRepository.GetKeywordsTypeOfAsync(typeOf, token);

                        // Preparo dos objetos aninhados (doc keyword version, doc version e doc properties)
                        var orbisDocKeywordVersions = GetOrbisDocKeywordVersionsList(reportType, yearAsNullableDouble, nomeCidadao, noLaudo, protocolo);
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

                if (listFilePaths.Items.Count == 0)
                {
                    UpdateSendToOrbisButtonState();
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
            
            if (comboReportType.SelectedItem is not null && comboReportType.SelectedItem.ToString() == "PERÕCIA NO MORTO (SIPL)")
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
