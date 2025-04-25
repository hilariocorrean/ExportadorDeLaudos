using ImportadorDeLaudos.Contracts;
using ImportadorDeLaudos.Models.Orbis;
using ImportadorDeLaudos.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;

namespace ImportadorDeLaudos
{
    public partial class CoordenadoriaDePsiquiatriaForenseForm : Form
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IOrbisRepository orbisRepository;
        private readonly string admUser;
        private readonly string admPass;

        public CoordenadoriaDePsiquiatriaForenseForm(IConfigurationRoot configuration, HttpClient _httpClient)
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
                List<string> files = Directory.GetFiles(selectedFolder, "*.*-*.pdf", SearchOption.AllDirectories)
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
        private List<OrbisDocKeywordVersion> GetOrbisDocKeywordVersionsList(string modalidadePsiquiatria, string protocolo, string nome)
        {
            var keywordVersionList = new List<OrbisDocKeywordVersion>();

            //ATUALIZAR OS ÍNDICES E GUIDS (ok?)
            var modalidadePsiquiatriaKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "a85bf015-8283-4f4e-bd0a-9275d0fdbc92",
                Enable = 1,
                Status = 1,
                Value = modalidadePsiquiatria
            };            
            var nomeKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "2c79842d-51ae-45d2-b215-59c922312d1f",
                Enable = 1,
                Status = 1,
                Value = nome
            };
            var protocoloKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "29d181fb-3df8-4ef4-969b-19add378d179",
                Enable = 1,
                Status = 1,
                Value = protocolo
            };
           
            keywordVersionList.Add(modalidadePsiquiatriaKeyword);
            keywordVersionList.Add(protocoloKeyword);
            keywordVersionList.Add(nomeKeyword);


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
            string reportType = comboReportType.SelectedItem!.ToString()!;
            var maxFilesAsInt = (int)maxFiles.Value;

            // Uma chamada para cada arquivo na lista. O ano é constante, o número máximo de arquivos é constante mas o número de
            // protocolo precisa ser atualizado a cada registro dentro do loop.
            var unsuccessfulFilePathsList = new List<string>();
            var fileCounter = 0;

            var processingRequestForm = new ProcessingRequestForm();
            processingRequestForm.Show();

            foreach (string filePath in listFilePaths.Items)
            {
                if (fileCounter < maxFilesAsInt)
                {
                    string fileName = Path.GetFileName(filePath);

                    string pdfName = fileName.Substring(0, fileName.Length - 4);
                    (string protocolCode, string name) = (pdfName.Split("-")[0], pdfName.Split("-")[1]);

                    //MessageBox.Show($"Tipo de laudo: {reportType}\nAno: {yearAsDouble}\nNúmero máximo de arquivos: {maxFilesAsDouble}\nProcessando a requisição...");
                    var token = await orbisRepository.GetLoginTokenAsync(admUser, admPass);
                    // Por enquanto, vou pular o passo de levantar o índice (keyword) a partir do tipo documental (typeof).
                    // Tipo documental: Coordenadoria de Psiquiatria Forense
                    var typeOf = "a4386e0e-7582-419e-a97d-daa35bc1bc2e";
                    var testeKeywordsTypeOf = await orbisRepository.GetKeywordsTypeOfAsync(typeOf, token);

                    // Preparo dos objetos aninhados (doc keyword version, doc version e doc properties)
                    var orbisDocKeywordVersions = GetOrbisDocKeywordVersionsList(reportType, protocolCode, name);
                    var orbisDocVersions = GetOrbisDocVersionsList(fileName, typeOf, orbisDocKeywordVersions);
                    var orbisDocProperties = GetOrbisDocProperties(typeOf, orbisDocVersions);

                    // Preparo do objeto do documento a ser enviado em Documents/UploadFile
                    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    var document = new FormFile(fileStream, 0, fileStream.Length, "File", fileName) //VER SE O NOME ESPERADO É file MESMO
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "application/pdf"  
                    };                   

                    // Chamada a Documents/UploadFile
                    (var responseAsString, var isSuccess) = await orbisRepository.UploadDocumentAsync(document, orbisDocProperties, token);
                    
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
                            MessageBox.Show($"Error renaming file:\n {ex.Message}\n {ex.StackTrace}");
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
