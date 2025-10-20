using ImportadorDeLaudos.Contracts;
using System;
using System.Windows.Forms;

namespace ImportadorDeLaudos
{
    partial class PCEPACastanhalIMOLLaudosMedicinaForm
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
        private CheckBox noYear;

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
            noYear = new CheckBox();
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
            // 
            // comboReportType
            // 
            comboReportType.Items.AddRange(new object[] { "SEXOLOGIA", "TRAUMATOLOGIA", "TANATOLOGIA" });
            comboReportType.Location = new Point(272, 396);
            comboReportType.Name = "comboReportType";
            comboReportType.Size = new Size(276, 28);
            comboReportType.TabIndex = 2;
            comboReportType.Text = "Selecionar...";
            comboReportType.SelectedIndexChanged += ComboReportType_SelectedIndexChanged;
            // 
            // year
            // 
            year.Location = new Point(272, 426);
            year.Maximum = new decimal(new int[] { 2125, 0, 0, 0 });
            year.Name = "year";
            year.Size = new Size(275, 27);
            year.TabIndex = 3;
            year.Value = new decimal(new int[] { 2025, 0, 0, 0 });
            // 
            // maxFiles
            // 
            maxFiles.Location = new Point(272, 456);
            maxFiles.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            maxFiles.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            maxFiles.Name = "maxFiles";
            maxFiles.Size = new Size(275, 27);
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
            labelReportType.Location = new Point(15, 399);
            labelReportType.Name = "labelReportType";
            labelReportType.Size = new Size(255, 20);
            labelReportType.TabIndex = 6;
            labelReportType.Text = "Modalidade (IMOL) Laudos Medicina";
            // 
            // labelYear
            // 
            labelYear.AutoSize = true;
            labelYear.Location = new Point(230, 428);
            labelYear.Name = "labelYear";
            labelYear.Size = new Size(36, 20);
            labelYear.TabIndex = 7;
            labelYear.Text = "Ano";
            // 
            // labelMaxFiles
            // 
            labelMaxFiles.AutoSize = true;
            labelMaxFiles.Location = new Point(101, 456);
            labelMaxFiles.Name = "labelMaxFiles";
            labelMaxFiles.Size = new Size(165, 20);
            labelMaxFiles.TabIndex = 8;
            labelMaxFiles.Text = "Nº máximo de arquivos";
            // 
            // noYear
            // 
            noYear.AutoSize = true;
            noYear.Location = new Point(553, 429);
            noYear.Name = "useYear";
            noYear.Size = new Size(101, 24);
            noYear.TabIndex = 9;
            noYear.Text = "Sem ano";
            noYear.UseVisualStyleBackColor = true;
            noYear.CheckedChanged += NoYear_CheckedChanged;
            // 
            // PCEPACastanhalIMOLLaudosMedicinaForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1083, 542);
            Controls.Add(noYear);
            Controls.Add(labelMaxFiles);
            Controls.Add(labelYear);
            Controls.Add(labelReportType);
            Controls.Add(buttonSelectFolder);
            Controls.Add(listFilePaths);
            Controls.Add(comboReportType);
            Controls.Add(year);
            Controls.Add(maxFiles);
            Controls.Add(buttonSendToOrbis);
            Name = "PCEPACastanhalIMOLLaudosMedicinaForm";
            Text = "Importador de Laudos - PCEPA Castanhal (IMOL) Laudos Medicina";
            ((System.ComponentModel.ISupportInitialize)year).EndInit();
            ((System.ComponentModel.ISupportInitialize)maxFiles).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion        
    }
}
