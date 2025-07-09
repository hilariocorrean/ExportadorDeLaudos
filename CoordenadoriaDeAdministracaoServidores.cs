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
    public partial class CoordenadoriaDeAdministracaoServidoresForm : Form
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IOrbisRepository orbisRepository;
        private readonly string admUser;
        private readonly string admPass;

        public CoordenadoriaDeAdministracaoServidoresForm(IConfigurationRoot configuration, HttpClient _httpClient)
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
                // Nomes representados como qualquer sequÍncia de letras + acentos padr„o + apÛstrofo e espaÁo em branco.
                // CPF esperado como uma sequÍncia simples de 11 dÌgitos. Arquivos com outros n˙meros ser„o ignorados.
                Regex namePattern = new Regex(@"^[a-zA-Z·¡È…ÌÕÛ”˙⁄‡¿Ë»ÏÃÚ“˘Ÿ‚¬Í ÓŒÙ‘˚€„√ı’Á«'\s]*\s\d{11}\.pdf$", RegexOptions.Compiled);
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
        private List<OrbisDocKeywordVersion> GetOrbisDocKeywordVersionsList(string modalidadeServidores, string ativoInativo, string matricula, string nome, string cpf)
        {
            var keywordVersionList = new List<OrbisDocKeywordVersion>();

            //ATUALIZAR OS ÕNDICES E GUIDS
            var modalidadeServidoresKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "6e06f83f-940b-44a9-aa21-4a39859c1f8d",
                Enable = 1,
                Status = 1,
                Value = modalidadeServidores
            };            
            var ativoInativoKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "604f1bcc-56f1-4f93-a597-d7f2e665e0e3",
                Enable = 1,
                Status = 1,
                Value = ativoInativo
            };
            var matriculaKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "26c3d89d-4c7e-4917-8c6c-635391196ba1",
                Enable = 1,
                Status = 1,
                Value = matricula
            };
            var nomeKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "0576ce1f-3fee-4351-9db9-76a53309d6de",
                Enable = 1,
                Status = 1,
                Value = nome
            };
            var cpfKeyword = new OrbisDocKeywordVersion
            {
                KeywordTypeOf = "b0527d0b-0c91-4d8d-95b2-f0e839020017",
                Enable = 1,
                Status = 1,
                Value = cpf
            };
           
            keywordVersionList.Add(modalidadeServidoresKeyword);
            keywordVersionList.Add(ativoInativoKeyword);
            keywordVersionList.Add(matriculaKeyword);
            keywordVersionList.Add(nomeKeyword);
            keywordVersionList.Add(cpfKeyword);


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
                string docType = comboDocType.SelectedItem!.ToString()!;
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
                        string pdfName = Path.GetFileNameWithoutExtension(filePath);

                        string cpf = pdfName.Split(' ').LastOrDefault()!;
                        string name = pdfName.Replace(cpf, string.Empty).TrimEnd();

                        //MessageBox.Show($"Tipo de laudo: {docType}\nAno: {yearAsDouble}\nN˙mero m·ximo de arquivos: {maxFilesAsDouble}\nProcessando a requisiÁ„o...");
                        var token = await orbisRepository.GetLoginTokenAsync(admUser, admPass);
                        // Por enquanto, vou pular o passo de levantar o Ìndice (keyword) a partir do tipo documental (typeof).
                        // Tipo documental: Coordenadoria de AdministraÁ„o (COAD) Servidores
                        var typeOf = "e9fe2c91-5cc1-41f3-8c47-670a7f301b08";

                        // Preparo dos objetos aninhados (doc keyword version, doc version e doc properties)
                        var orbisDocKeywordVersions = GetOrbisDocKeywordVersionsList(docType, string.Empty, string.Empty, name, cpf);
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

        private void ComboDocType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSendToOrbisButtonState();
        }

        private void ResetComboDocTypeState()
        {
            comboDocType.SelectedItem = null;
            comboDocType.Text = "Selecionar...";
        }

        private void UpdateSendToOrbisButtonState()
        {
            // Libera o bot„o de envio se houver ao menos um arquivo na lista e se tiver um tipo definido de laudo
            buttonSendToOrbis.Enabled = listFilePaths.Items.Count > 0 && comboDocType.SelectedIndex != -1; //mudar o segundo pra 'is not null'?
        }
    }
}
