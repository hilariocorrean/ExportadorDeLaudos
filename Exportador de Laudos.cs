using ExportadorDeLaudos.Contracts;
using ExportadorDeLaudos.Repository;
using Microsoft.Extensions.Configuration;

namespace ExportadorDeLaudos
{
    public partial class Form1 : Form
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IRelatorioAtualizadoRepository _relatorioAtualizadoRepository;


        public Form1()
        {
            InitializeComponent();
        }

        public Form1(IConfigurationRoot configuration)
        {
            this._configuration = configuration;
            InitializeComponent();
            _relatorioAtualizadoRepository = new RelatorioAtualizadoRepository(configuration);
        }


        private void ButtonSelectFolder_Click(object sender, EventArgs e)
        {
            // Open folder browser dialog to select a folder
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFolder = folderBrowserDialog.SelectedPath;

                // Get all files from the selected folder and display them in the list
                listFilePaths.Items.Clear();  // Clear the previous list
                List<string> files = Directory.GetFiles(selectedFolder, "*.pdf", SearchOption.AllDirectories)
                                          .Where(s => !s.Contains("OK_")).ToList();
                foreach (var file in files)
                {
                    listFilePaths.Items.Add(file);  // Add file path to the list
                }

                // Check and update the button state
                UpdateSendToOrbisButtonState();
            }
        }

        private void ButtonSendToOrbis_Click(object sender, EventArgs e)
        {
            // Here you can implement the behavior for the second button
            // For now, let's show the selected inputs for demonstration
            string reportType = comboReportType.SelectedItem.ToString();
            var yearAsDouble = (double)year.Value;
            var maxFilesAsDouble = (double)maxFiles.Value;

            // Uma chamada para cada arquivo na lista. O ano é constante, o número máximo de arquivos é constante mas o número de
            // protocolo precisa ser atualizado a cada registro dentro do loop.
            var unsuccessfulFilePathsList = new List<string>();
            var fileCounter = 0;
            foreach (string filePath in listFilePaths.Items)
            {
                if (fileCounter < (int)maxFilesAsDouble)
                {
                    var protocoloReqNr = double.Parse(filePath.Substring(filePath.Length - 10, 6));
                    var relatorioAtualizado = _relatorioAtualizadoRepository.GetRelatorioAtualizadoByAnoAndProtocoloReqNr(yearAsDouble, protocoloReqNr);

                    MessageBox.Show($"Tipo de laudo: {reportType}\nAno: {yearAsDouble}\nNúmero máximo de arquivos: {maxFilesAsDouble}\nProcessando a requisição...");

                    //var token = 

                    // Daqui para baixo é o tratamento pós-retorno da API

                    // if (returnCode == 200)
                    string directoryPath = Path.GetDirectoryName(filePath); // Get the directory path
                    string fileName = Path.GetFileName(filePath); // Get the original file name
                    string newFileName = "OK_" + fileName; // New file name with "OK_" prefix
                    string newFilePath = Path.Combine(directoryPath, newFileName); // Combine path with new name
                    try
                    {
                        File.Move(filePath, newFilePath); // Rename the file on the disk
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error renaming file: {ex.Message}");
                    }

                    // else
                    //unsuccessfulFilePathsList.Add(filePath);
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
        }

        private void ComboReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check and update the button state whenever the ComboBox selection changes
            UpdateSendToOrbisButtonState();
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
