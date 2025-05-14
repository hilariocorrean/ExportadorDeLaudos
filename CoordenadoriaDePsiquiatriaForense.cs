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

                List<string> filePaths = Directory.GetFiles(selectedFolder, "*.pdf", SearchOption.AllDirectories).ToList();
                // LÛgica de pattern matching para eliminar os que fogem do padr„o
                // AdiÁ„o de apÛstrofo (D'artagnan e Sant'anna me pegaram desprevenido) e ponto (h· arquivos com nomes abreviados)
                // Mudei de ideia. Deixa os com . para avaliaÁ„o manual. 
                Regex namePattern = new Regex(@"^\d+\.\d{4}-[a-zA-Z·¡È…ÌÕÛ”˙⁄‡¿Ë»ÏÃÚ“˘Ÿ‚¬Í ÓŒÙ‘˚€„√ı’Á«'\s]+\.pdf$", RegexOptions.Compiled);
                foreach (var filePath in filePaths)
                {
                    var fileName = Path.GetFileName(filePath);

                    if (namePattern.IsMatch(fileName))
                    {
                        listFilePaths.Items.Add(filePath);
                    }
                }

                // Usar o ResetComboReportTypeState() caso venha a ter outra modalidade
                UpdateSendToOrbisButtonState();
            }
        }

        // Em princÌpio, vai ser preciso que o mÈtodo seja especÌfico e hard-coded para cada tipo documental. Retorna a lista de
        // Ìndices possÌveis para o tipo documental em quest„o.
        // TODO: Adicionar ‡ lista de argumentos a lista de keywords retornadas do GetKeywordsTypeOfsAsync, de modo que
        // KeywordTypeOf = Id e Value È associado a uma Label retornada.
        private List<OrbisDocKeywordVersion> GetOrbisDocKeywordVersionsList(string modalidadePsiquiatria, string protocolo, string nome)
        {
            var keywordVersionList = new List<OrbisDocKeywordVersion>();

            //ATUALIZAR OS ÕNDICES E GUIDS (ok?)
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
            try
            {
                string reportType = comboReportType.SelectedItem!.ToString()!;
                var maxFilesAsInt = (int)maxFiles.Value;

                // Uma chamada para cada arquivo na lista. O ano È constante, o n˙mero m·ximo de arquivos È constante mas o n˙mero de
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

                        //MessageBox.Show($"Tipo de laudo: {reportType}\nAno: {yearAsDouble}\nN˙mero m·ximo de arquivos: {maxFilesAsDouble}\nProcessando a requisiÁ„o...");
                        var token = await orbisRepository.GetLoginTokenAsync(admUser, admPass);
                        // Por enquanto, vou pular o passo de levantar o Ìndice (keyword) a partir do tipo documental (typeof).
                        // Tipo documental: Coordenadoria de Psiquiatria Forense
                        var typeOf = "a4386e0e-7582-419e-a97d-daa35bc1bc2e";
                        //var testeKeywordsTypeOf = await orbisRepository.GetKeywordsTypeOfAsync(typeOf, token);

                        // Preparo dos objetos aninhados (doc keyword version, doc version e doc properties)
                        var orbisDocKeywordVersions = GetOrbisDocKeywordVersionsList(reportType, protocolCode, name);
                        var orbisDocVersions = GetOrbisDocVersionsList(fileName, typeOf, orbisDocKeywordVersions);
                        var orbisDocProperties = GetOrbisDocProperties(typeOf, orbisDocVersions);

                        // Preparo do objeto do documento a ser enviado em Documents/UploadFile
                        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        var document = new FormFile(fileStream, 0, fileStream.Length, "File", fileName) //VER SE O NOME ESPERADO … file MESMO
                        {
                            Headers = new HeaderDictionary(),
                            ContentType = "application/pdf"
                        };

                        // Chamada a Documents/UploadFile
                        (var responseAsString, var isSuccess) = await orbisRepository.UploadDocumentAsync(document, orbisDocProperties, token);

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
                                MessageBox.Show($"Erro ao renomear o arquivo:\n {ex.Message}\n {ex.StackTrace}");
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
            // Libera o bot„o de envio se houver ao menos um arquivo na lista e se tiver um tipo definido de laudo
            buttonSendToOrbis.Enabled = listFilePaths.Items.Count > 0 && comboReportType.SelectedIndex != -1; //mudar o segundo pra 'is not null'?
        }
    }
}
