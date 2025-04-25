using ImportadorDeLaudos.Contracts;
using System;
using System.Windows.Forms;

namespace ImportadorDeLaudos
{
    partial class CoordenadoriaDePsiquiatriaForenseForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Button buttonSelectFolder;
        private Button buttonSendToOrbis;
        private ListBox listFilePaths;
        private ComboBox comboReportType;
        private NumericUpDown maxFiles;
        private Label labelReportType;
        private Label labelMaxFiles;
        private FolderBrowserDialog folderBrowserDialog;
        private OpenFileDialog openFileDialog;

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
            maxFiles = new NumericUpDown();
            buttonSendToOrbis = new Button();
            folderBrowserDialog = new FolderBrowserDialog();
            openFileDialog = new OpenFileDialog();
            labelReportType = new Label();
            labelMaxFiles = new Label();
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
            comboReportType.Enabled = false;
            comboReportType.Items.AddRange(new object[] { "LAUDOS" }); // se não, tentar MODALIDADE PSIQUIATRIA
            comboReportType.Location = new Point(183, 403);
            comboReportType.Name = "comboReportType";
            comboReportType.Size = new Size(271, 28);
            comboReportType.TabIndex = 2;
            comboReportType.Text = "LAUDOS";
            comboReportType.SelectedIndexChanged += ComboReportType_SelectedIndexChanged;
            // 
            // maxFiles
            // 
            maxFiles.Location = new Point(183, 436);
            maxFiles.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            maxFiles.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            maxFiles.Name = "maxFiles";
            maxFiles.Size = new Size(270, 27);
            maxFiles.TabIndex = 4;
            maxFiles.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // buttonSendToOrbis
            // 
            buttonSendToOrbis.Enabled = false;
            buttonSendToOrbis.Location = new Point(7, 482);
            buttonSendToOrbis.Name = "buttonSendToOrbis";
            buttonSendToOrbis.Size = new Size(232, 28);
            buttonSendToOrbis.TabIndex = 5;
            buttonSendToOrbis.Text = "Enviar os arquivos para o Orbis";
            buttonSendToOrbis.Click += ButtonSendToOrbis_Click;
            // 
            // labelReportType
            // 
            labelReportType.AutoSize = true;
            labelReportType.Location = new Point(75, 406);
            labelReportType.Name = "labelReportType";
            labelReportType.Size = new Size(102, 20);
            labelReportType.TabIndex = 6;
            labelReportType.Text = "Tipo de laudo";
            // 
            // labelMaxFiles
            // 
            labelMaxFiles.AutoSize = true;
            labelMaxFiles.Location = new Point(12, 438);
            labelMaxFiles.Name = "labelMaxFiles";
            labelMaxFiles.Size = new Size(165, 20);
            labelMaxFiles.TabIndex = 8;
            labelMaxFiles.Text = "Nº máximo de arquivos";
            // 
            // CoordenadoriaDePsiquiatriaForenseForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1083, 542);
            Controls.Add(labelMaxFiles);
            Controls.Add(labelReportType);
            Controls.Add(buttonSelectFolder);
            Controls.Add(listFilePaths);
            Controls.Add(comboReportType);
            Controls.Add(maxFiles);
            Controls.Add(buttonSendToOrbis);
            Name = "CoordenadoriaDePsiquiatriaForenseForm";
            Text = "Importador de Laudos - Coordenadoria de Psiquiatria Forense";
            ((System.ComponentModel.ISupportInitialize)maxFiles).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion        
    }
}
