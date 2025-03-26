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
        private ListBox listFiles;
        private ComboBox comboStatus;
        private NumericUpDown year;
        private NumericUpDown maxFiles;
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
            listFiles = new ListBox();
            comboStatus = new ComboBox();
            year = new NumericUpDown();
            maxFiles = new NumericUpDown();
            buttonSendToOrbis = new Button();
            folderBrowserDialog = new FolderBrowserDialog();
            openFileDialog = new OpenFileDialog();
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
            // listFiles
            // 
            listFiles.AllowDrop = true;
            listFiles.Location = new Point(10, 53);
            listFiles.Name = "listFiles";
            listFiles.SelectionMode = SelectionMode.None;
            listFiles.Size = new Size(1004, 324);
            listFiles.TabIndex = 1;
            listFiles.DragDrop += ListFiles_DragDrop;
            listFiles.DragEnter += ListFiles_DragEnter;
            // 
            // comboStatus
            // 
            //comboStatus.Items.AddRange(new object[] { "Laudo vivo", "Laudo morto" });
            comboStatus.Items.Add("Laudo vivo");
            comboStatus.Items.Add("Laudo morto");
            comboStatus.Location = new Point(10, 402);
            comboStatus.Name = "comboStatus";
            comboStatus.Text = "Tipo de laudo (vivo/morto): ";
            comboStatus.Size = new Size(121, 28);
            comboStatus.TabIndex = 2;
            // 
            // year
            // 
            year.Location = new Point(10, 432);
            year.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            //year.Maximum = new decimal(new int[] { 1, 1, 1, 1 });
            year.Name = "year";
            year.Text = "Ano: ";
            year.Size = new Size(120, 27);
            year.TabIndex = 3;
            year.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // maxFiles
            // 
            maxFiles.Location = new Point(10, 462);
            maxFiles.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            //maxFiles.Maximum = new decimal(new int[] { 1, 1, 1, 1 });
            maxFiles.Name = "maxFiles";
            maxFiles.Text = "Número máximo de arquivos para envio: ";
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1083, 542);
            Controls.Add(buttonSelectFolder);
            Controls.Add(listFiles);
            Controls.Add(comboStatus);
            Controls.Add(year);
            Controls.Add(maxFiles);
            Controls.Add(buttonSendToOrbis);
            Name = "Form1";
            Text = "Exportador de Laudos";
            ((System.ComponentModel.ISupportInitialize)year).EndInit();
            ((System.ComponentModel.ISupportInitialize)maxFiles).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private void ButtonSelectFolder_Click(object sender, EventArgs e)
        {
            // Open folder browser dialog to select a folder
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFolder = folderBrowserDialog.SelectedPath;

                // Get all files from the selected folder and display them in the list
                listFiles.Items.Clear();  // Clear the previous list
                string[] files = Directory.GetFiles(selectedFolder);
                foreach (var file in files)
                {
                    listFiles.Items.Add(file);  // Add file path to the list
                }

                // Enable the process button if there are files in the folder
                buttonSendToOrbis.Enabled = listFiles.Items.Count > 0;
            }
        }

        private void ListFiles_DragEnter(object sender, DragEventArgs e)
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

        private void ListFiles_DragDrop(object sender, DragEventArgs e)
        {
            // Get the files being dragged
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Add files to the list box and ensure no duplicates
            foreach (var file in files)
            {
                if (!listFiles.Items.Contains(file))
                {
                    listFiles.Items.Add(file);
                }
            }

            // Enable the process button if files are selected
            buttonSendToOrbis.Enabled = listFiles.Items.Count > 0;
        }

        private void ButtonSendToOrbis_Click(object sender, EventArgs e)
        {
            // Here you can implement the behavior for the second button
            // For now, let's show the selected inputs for demonstration
            string status = comboStatus.SelectedItem.ToString();
            decimal yearAsDecimalForSomeReason = year.Value;
            decimal maxFilesAsDecimalForSomeReason = maxFiles.Value;

            MessageBox.Show($"Status: {status}\nAno: {yearAsDecimalForSomeReason}\nNúmero máximo de arquivos: {maxFilesAsDecimalForSomeReason}\nProcessando a requisição...");
        }
    }
}
