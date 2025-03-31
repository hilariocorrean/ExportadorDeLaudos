using ExportadorDeLaudos.Contracts;
using System;
using System.Windows.Forms;

namespace ExportadorDeLaudos
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Button buttonSelectFolder;
        private Button buttonSendToOrbis;
        private ListBox listFilePaths;
        private ComboBox comboReportType;
        private NumericUpDown year;
        private NumericUpDown maxFiles;
        private Label labelReportType;
        private Label labelYear;
        private Label labelMaxFiles;
        private FolderBrowserDialog folderBrowserDialog;
        private OpenFileDialog openFileDialog;
        private IRelatorioAtualizadoRepository relatorioAtualizadoRepository;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonSelectFolder = new Button();
            listFilePaths = new ListBox();
            comboReportType = new ComboBox();
            year = new NumericUpDown();
            maxFiles = new NumericUpDown();
            buttonSendToOrbis = new Button();
            folderBrowserDialog = new FolderBrowserDialog();
            openFileDialog = new OpenFileDialog();
            labelReportType = new Label();
            labelYear = new Label();
            labelMaxFiles = new Label();
            ((System.ComponentModel.ISupportInitialize)year).BeginInit();
            ((System.ComponentModel.ISupportInitialize)maxFiles).BeginInit();
            SuspendLayout();
            // 
            // buttonSelectFolder
            // 
            buttonSelectFolder.Location = new Point(10, 10);
            buttonSelectFolder.Name = "buttonSelectFolder";
            buttonSelectFolder.Size = new Size(269, 37);
            buttonSelectFolder.TabIndex = 0;
            buttonSelectFolder.Text = "Selecionar a pasta com os arquivos...";
            buttonSelectFolder.Click += ButtonSelectFolder_Click;
            // 
            // listFilePaths
            // 
            listFilePaths.AllowDrop = true;
            listFilePaths.Location = new Point(10, 53);
            listFilePaths.Name = "listFilePaths";
            listFilePaths.SelectionMode = SelectionMode.None;
            listFilePaths.Size = new Size(1004, 324);
            listFilePaths.TabIndex = 1;
            listFilePaths.DragDrop += ListFilePaths_DragDrop;
            listFilePaths.DragEnter += ListFilePaths_DragEnter;
            // 
            // comboReportType
            // 
            comboReportType.Items.AddRange(new object[] { "Laudo vivo", "Laudo morto" });
            comboReportType.Location = new Point(186, 396);
            comboReportType.Name = "comboReportType";
            comboReportType.Size = new Size(121, 28);
            comboReportType.TabIndex = 2;
            comboReportType.Text = "Selecionar...";
            comboReportType.SelectedIndex = -1; // No default selection
            comboReportType.SelectedIndexChanged += ComboReportType_SelectedIndexChanged; // Handle selection change
            // 
            // year
            // 
            year.Location = new Point(186, 426);
            year.Minimum = 0;
            year.Maximum = 2125;
            year.Name = "year";
            year.Size = new Size(120, 27);
            year.TabIndex = 3;
            year.Value = DateTime.Now.Year;
            // 
            // maxFiles
            // 
            maxFiles.Location = new Point(186, 456);
            maxFiles.Minimum = 1;
            maxFiles.Maximum = 2147483647; //int max lol
            maxFiles.Name = "maxFiles";
            maxFiles.Size = new Size(120, 27);
            maxFiles.TabIndex = 4;
            maxFiles.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // buttonSendToOrbis
            // 
            buttonSendToOrbis.Enabled = false;
            buttonSendToOrbis.Location = new Point(10, 502);
            buttonSendToOrbis.Name = "buttonSendToOrbis";
            buttonSendToOrbis.Size = new Size(232, 28);
            buttonSendToOrbis.TabIndex = 5;
            buttonSendToOrbis.Text = "Enviar os arquivos para o Orbis";
            buttonSendToOrbis.Click += ButtonSendToOrbis_Click;
            // 
            // labelReportType
            // 
            labelReportType.AutoSize = true;
            labelReportType.Location = new Point(78, 399);
            labelReportType.Name = "labelReportType";
            labelReportType.Size = new Size(102, 20);
            labelReportType.TabIndex = 6;
            labelReportType.Text = "Tipo de laudo";
            // 
            // labelYear
            // 
            labelYear.AutoSize = true;
            labelYear.Location = new Point(144, 428);
            labelYear.Name = "labelYear";
            labelYear.Size = new Size(36, 20);
            labelYear.TabIndex = 7;
            labelYear.Text = "Ano";
            // 
            // labelMaxFiles
            // 
            labelMaxFiles.AutoSize = true;
            labelMaxFiles.Location = new Point(15, 458);
            labelMaxFiles.Name = "labelMaxFiles";
            labelMaxFiles.Size = new Size(165, 20);
            labelMaxFiles.TabIndex = 8;
            labelMaxFiles.Text = "Nº máximo de arquivos";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1083, 542);
            Controls.Add(labelMaxFiles);
            Controls.Add(labelYear);
            Controls.Add(labelReportType);
            Controls.Add(buttonSelectFolder);
            Controls.Add(listFilePaths);
            Controls.Add(comboReportType);
            Controls.Add(year);
            Controls.Add(maxFiles);
            Controls.Add(buttonSendToOrbis);
            Name = "Form1";
            Text = "Exportador de Laudos";
            ((System.ComponentModel.ISupportInitialize)year).EndInit();
            ((System.ComponentModel.ISupportInitialize)maxFiles).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private void ButtonSelectFolder_Click(object sender, EventArgs e)
        {
            // Open folder browser dialog to select a folder
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFolder = folderBrowserDialog.SelectedPath;

                // Get all files from the selected folder and display them in the list
                listFilePaths.Items.Clear();  // Clear the previous list
                string[] files = Directory.GetFiles(selectedFolder);
                foreach (var file in files)
                {
                    listFilePaths.Items.Add(file);  // Add file path to the list
                }

                // Check and update the button state
                UpdateSendToOrbisButtonState();
            }
        }

        private void ListFilePaths_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the dragged data is of file type
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;  // Allow move operation
            }
            else
            {
                e.Effect = DragDropEffects.None;  // Disallow drop
            }
        }

        private void ListFilePaths_DragDrop(object sender, DragEventArgs e)
        {
            // Get the files being dragged
            string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Add files to the list box and ensure no duplicates
            foreach (var filePath in filePaths)
            {
                if (!listFilePaths.Items.Contains(filePath))
                {
                    listFilePaths.Items.Add(filePath);
                }
            }

            // Enable the process button if files are selected
            buttonSendToOrbis.Enabled = listFilePaths.Items.Count > 0;
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

            foreach (string filePath in listFilePaths.Items)
            {
                var protocoloReqNr = double.Parse(filePath.Substring(filePath.Length - 10, 6));
                var relatorioAtualizado = relatorioAtualizadoRepository.GetRelatorioAtualizadoByAnoAndProtocoloReqNr(yearAsDouble, protocoloReqNr);

                MessageBox.Show($"Tipo de laudo: {reportType}\nAno: {yearAsDouble}\nNúmero máximo de arquivos: {maxFilesAsDouble}\nProcessando a requisição...");
            }
            //var teste = relatorioAtualizadoRepository.GetRelatorioAtualizadoByAnoAndProtocoloReqNr((double)yearAsDouble, (double)4);

            //MessageBox.Show($"Tipo de laudo: {reportType}\nAno: {yearAsDouble}\nNúmero máximo de arquivos: {maxFilesAsDouble}\nProcessando a requisição...");
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
