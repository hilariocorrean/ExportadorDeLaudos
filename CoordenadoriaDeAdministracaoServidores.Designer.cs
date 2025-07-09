using ImportadorDeLaudos.Contracts;
using System;
using System.Windows.Forms;

namespace ImportadorDeLaudos
{
    partial class CoordenadoriaDeAdministracaoServidoresForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Button buttonSelectFolder;
        private Button buttonSendToOrbis;
        private ListBox listFilePaths;
        private ComboBox comboDocType;
        private NumericUpDown maxFiles;
        private Label labelDocType;
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
            comboDocType = new ComboBox();
            maxFiles = new NumericUpDown();
            buttonSendToOrbis = new Button();
            folderBrowserDialog = new FolderBrowserDialog();
            openFileDialog = new OpenFileDialog();
            labelDocType = new Label();
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
            // comboDocType
            // 
            comboDocType.Items.AddRange(new object[] { "TEMPORÁRIO POLÍCIA CIVIL", "TEMPORÁRIO" });
            comboDocType.Location = new Point(209, 403);
            comboDocType.Name = "comboDocType";
            comboDocType.Size = new Size(271, 28);
            comboDocType.TabIndex = 2;
            comboDocType.Text = "Selecionar...";
            comboDocType.SelectedIndexChanged += ComboDocType_SelectedIndexChanged;
            // 
            // maxFiles
            // 
            maxFiles.Location = new Point(209, 436);
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
            // labelDocType
            // 
            labelDocType.AutoSize = true;
            labelDocType.Location = new Point(12, 406);
            labelDocType.Name = "labelDocType";
            labelDocType.Size = new Size(191, 20);
            labelDocType.TabIndex = 6;
            labelDocType.Text = "Modalidade dos Servidores";
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
            // CoordenadoriaDeAdministracaoServidoresForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1083, 542);
            Controls.Add(labelMaxFiles);
            Controls.Add(labelDocType);
            Controls.Add(buttonSelectFolder);
            Controls.Add(listFilePaths);
            Controls.Add(comboDocType);
            Controls.Add(maxFiles);
            Controls.Add(buttonSendToOrbis);
            Name = "CoordenadoriaDeAdministracaoServidoresForm";
            Text = "Importador de Laudos - Coordenadoria de Administração (COAD) Servidores";
            ((System.ComponentModel.ISupportInitialize)maxFiles).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion        
    }
}
