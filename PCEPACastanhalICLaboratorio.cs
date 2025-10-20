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
    public partial class PCEPACastanhalICLaboratorioForm : Form
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IOrbisRepository orbisRepository;
        private readonly string admUser;
        private readonly string admPass;

        public PCEPACastanhalICLaboratorioForm(IConfigurationRoot configuration, HttpClient _httpClient)
        {
            _configuration = configuration;
            InitializeComponent();
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
                // Lógica de pattern matching para eliminar os que fogem do padrão não se aplica ao tipo atual.

                foreach (var filePath in filePaths)
                {
                    var fileName = Path.GetFileName(filePath);
                    listFilePaths.Items.Add(filePath);
                }

                ResetComboReportTypeState();
                UpdateSendToOrbisButtonState();
            }
        }

        // Em princípio, vai ser preciso que o método seja específico e hard-coded para cada tipo documental. Retorna a lista de
        // índices possíveis para o tipo documental em questão.
        // TODO: Adicionar à lista de argumentos a lista de keywords retornadas do GetKeywordsTypeOfsAsync, de modo que
        // KeywordTypeOf = Id e Value é associado a uma Label retornada.
        private List<OrbisDocKeywordVersion> GetOrbisDocKeywordVersionsList(string modalidadeICLaboratorio, string nome, double? ano)
        {
            var keywordVersionList = new List<OrbisDocKeywordVersion>();

            var modalidadeICLaboratorioKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "0199816e-0bdd-77e4-bc39-41119d51c39a",
                Enable = 1,
                Status = 1,
                Value = modalidadeICLaboratorio
            };            
            var nomeKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "0199816e-0bdd-7ea6-92a5-460b663fec5f",
                Enable = 1,
                Status = 1,
                Value = nome
            };
            var anoKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "0199816e-0bdd-70a0-ba0a-0fcb7e08cd95",
                Enable = 1,
                Status = 1,
                Value = ano is null ? String.Empty : ((int)ano!).ToString()
            };

            keywordVersionList.Add(modalidadeICLaboratorioKeyword);
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
                double? yearAsNullableDouble = noYear.Checked ? null : (double)year.Value;
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
                        string fileName = Path.GetFileName(filePath);
                        string docName = Path.GetFileNameWithoutExtension(filePath);                               

                        var token = await orbisRepository.GetLoginTokenAsync(admUser, admPass);
                        // Tipo documental: PCEPA CASTANHAL (IC) LABORATORIO
                        var typeOf = "01998168-49da-7d70-b45b-96695011fe3e";
                        //var testeKeywordsTypeOf = await orbisRepository.GetKeywordsTypeOfAsync(typeOf, token);

                        // Preparo dos objetos aninhados (doc keyword version, doc version e doc properties)
                        var orbisDocKeywordVersions = GetOrbisDocKeywordVersionsList(reportType, docName, yearAsNullableDouble);
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
            UpdateSendToOrbisButtonState();
        }

        private void ResetComboReportTypeState()
        {
            comboReportType.SelectedItem = null;
            comboReportType.Text = "Selecionar...";
        }

        private void UpdateSendToOrbisButtonState()
        {
            // Libera o botão de envio se houver ao menos um arquivo na lista e se tiver um tipo definido de laudo
            buttonSendToOrbis.Enabled = listFilePaths.Items.Count > 0 && comboReportType.SelectedIndex != -1; //mudar o segundo pra 'is not null'?
        }

        private void NoYear_CheckedChanged(object sender, EventArgs e)
        {
            year.Enabled = !noYear.Checked;
        }
    }
}
