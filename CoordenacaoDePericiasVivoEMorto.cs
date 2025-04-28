using ImportadorDeLaudos.Contracts;
using ImportadorDeLaudos.Models;
using ImportadorDeLaudos.Models.Orbis;
using ImportadorDeLaudos.Repository;
using ImportadorDeLaudos.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;

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
                List<string> files = Directory.GetFiles(selectedFolder, "*.pdf", SearchOption.AllDirectories)
                                          .Where(s => !s.Contains("OK_")).ToList();
                foreach (var file in files)
                {
                    listFilePaths.Items.Add(file);
                }

                ResetComboReportTypeState();
                UpdateSendToOrbisButtonState();
            }
        }

        // Em princípio, vai ser preciso que o método seja específico e hard-coded para cada tipo documental. Retorna a lista de
        // índices possíveis para o tipo documental em questão.
        // TODO: Adicionar à lista de argumentos a lista de keywords retornadas do GetKeywordsTypeOfsAsync, de modo que
        // KeywordTypeOf = Id e Value é associado a uma Label retornada.
        private List<OrbisDocKeywordVersion> GetOrbisDocKeywordVersionsList(string modalidadeDeLaudos, string protocolo, string nome, double ano)
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
                Value = ((int)ano).ToString()
            };

            keywordVersionList.Add(modalidadeDeLaudosKeyword);
            keywordVersionList.Add(protocoloKeyword);
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
                var yearAsDouble = (double)year.Value;
                var maxFilesAsDouble = (double)maxFiles.Value;

                // Uma chamada para cada arquivo na lista. O ano é constante, o número máximo de arquivos é constante mas o número de
                // protocolo precisa ser atualizado a cada registro dentro do loop.
                var unsuccessfulFilePathsList = new List<string>();
                var fileCounter = 0;

                var processingRequestForm = new ProcessingRequestForm();
                processingRequestForm.Show();

                foreach (string filePath in listFilePaths.Items)
                {
                    if (fileCounter < (int)maxFilesAsDouble)
                    {
                        double protocoloReqNr; // O QUE É USADO PARA ACHAR NO BANCO
                        string nomeCidadao;
                        string protocolo; // O QUE REALMENTE É ENVIADO
                        var relatorioAtualizado = new RelatorioAtualizado();
                        string fileName = Path.GetFileName(filePath);


                        if (reportType == "PERÍCIA NO VIVO")
                        {
                            protocoloReqNr = double.Parse(fileName.Substring(0, fileName.Length - 4));
                            relatorioAtualizado = relatorioAtualizadoRepository.GetRelatorioAtualizadoByAnoAndProtocoloReqNr(yearAsDouble, protocoloReqNr);
                            // Caso exista entrada correspondente, o nome é buscado. Como é um índice opcional que a PCEPA sugeriu que
                            // nem fosse usado para o laudo vivo, foda-se. FODA-SE O CARALHO, tinha esquecido que essa porra é um ponteiro,
                            // então, se a entrada não bate, o método ToUpper() é chamado num ponteiro nulo.

                            nomeCidadao = relatorioAtualizado.NOME is null ? string.Empty : relatorioAtualizado.NOME.ToUpper();
                            // Não havendo registro, presume que é da regional de Belém.
                            var protocoloGerado = year.Value.ToString() + ".01." + protocoloReqNr.ToString().PadLeft(6, '0');
                            protocolo = relatorioAtualizado.PROTOCOLO is null ? protocoloGerado : relatorioAtualizado.PROTOCOLO;
                        }
                        else
                        {
                            nomeCidadao = fileName.Substring(0, fileName.Length - 4);
                            relatorioAtualizado = relatorioAtualizadoRepository.GetRelatorioAtualizadoByAnoAndNome(yearAsDouble, nomeCidadao);
                            // Caso exista entrada correspondente, o número de protocolo é buscado. Como é um índice opcional que a PCEPA sugeriu que
                            // nem fosse usado para o laudo morto, foda-se.
                            protocoloReqNr = relatorioAtualizado.PROTOCOLO_REQ_NR;
                            // Não havendo registro, presume que é da regional de Belém.
                            var protocoloGerado = year.Value.ToString() + ".01." + protocoloReqNr.ToString().PadLeft(6, '0');
                            protocolo = relatorioAtualizado.PROTOCOLO is null ? protocoloGerado : relatorioAtualizado.PROTOCOLO;
                        }

                        //MessageBox.Show($"Tipo de laudo: {reportType}\nAno: {yearAsDouble}\nNúmero máximo de arquivos: {maxFilesAsDouble}\nProcessando a requisição...");
                        var token = await orbisRepository.GetLoginTokenAsync(admUser, admPass);
                        // Por enquanto, vou pular o passo de levantar o índice (keyword) a partir do tipo documental (typeof).
                        // Tipo documental: Coordenação de perícias vivo ou morto
                        var typeOf = "5d937fae-6564-491b-bc3b-4385bd2de9de";
                        var testeKeywordsTypeOf = await orbisRepository.GetKeywordsTypeOfAsync(typeOf, token);

                        // Preparo dos objetos aninhados (doc keyword version, doc version e doc properties)
                        var orbisDocKeywordVersions = GetOrbisDocKeywordVersionsList(reportType, protocolo, nomeCidadao, yearAsDouble);
                        var orbisDocVersions = GetOrbisDocVersionsList(fileName, typeOf, orbisDocKeywordVersions);
                        var orbisDocProperties = GetOrbisDocProperties(typeOf, orbisDocVersions);

                        // Preparo do objeto do documento a ser enviado em Documents/UploadFile
                        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        var document = new FormFile(fileStream, 0, fileStream.Length, "File", fileName) //VER SE O NOME ESPERADO É file MESMO
                        {
                            Headers = new HeaderDictionary(),
                            ContentType = "application/pdf"
                        };

                        // Chamada a Documents/UploadFile SE houver entrada correspondente na tabela
                        string responseAsString = String.Empty;
                        bool isSuccess = false;
                        (responseAsString, isSuccess) = await orbisRepository.UploadDocumentAsync(document, orbisDocProperties, token);


                        // Tratamento pós-retorno da API
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
            UpdateSendToOrbisButtonState();
        }

        private void ResetComboReportTypeState()
        {
            comboReportType.SelectedItem = null;
            comboReportType.Text = "Selecione...";
        }

        private void UpdateSendToOrbisButtonState()
        {
            // Enable the button only if both conditions are met:
            // 1. The list is not empty
            // 2. The ComboBox has a selected item
            buttonSendToOrbis.Enabled = listFilePaths.Items.Count > 0 && comboReportType.SelectedIndex != -1; //mudar o segundo pra 'is not null'?
        }
    }
}
