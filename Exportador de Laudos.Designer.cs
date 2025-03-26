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
        private Button buttonSelectFiles;
        private Button buttonSendToOrbis;
        private ListBox listFiles;
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
            buttonSelectFiles = new Button();
            listFiles = new ListBox();
            buttonSendToOrbis = new Button();
            openFileDialog = new OpenFileDialog();
            SuspendLayout();
            // 
            // buttonSelectFiles
            // 
            buttonSelectFiles.Location = new Point(15, 15);
            buttonSelectFiles.Name = "buttonSelectFiles";
            buttonSelectFiles.Size = new Size(150, 30);
            buttonSelectFiles.TabIndex = 0;
            buttonSelectFiles.Text = "Selecionar arquivos";
            buttonSelectFiles.Click += ButtonSelectFiles_Click;
            // 
            // listFiles
            // 
            listFiles.AllowDrop = true;
            listFiles.Location = new Point(12, 50);
            listFiles.Name = "listFiles";
            listFiles.SelectionMode = SelectionMode.None;
            listFiles.Size = new Size(560, 424);
            listFiles.TabIndex = 1;
            listFiles.DragDrop += ListFiles_DragDrop;
            listFiles.DragEnter += ListFiles_DragEnter;
            // 
            // buttonSendToOrbis
            // 
            buttonSendToOrbis.Enabled = false;
            buttonSendToOrbis.Location = new Point(12, 480);
            buttonSendToOrbis.Name = "buttonSendToOrbis";
            buttonSendToOrbis.Size = new Size(75, 30);
            buttonSendToOrbis.TabIndex = 2;
            buttonSendToOrbis.Text = "Enviar para o Orbis";
            buttonSendToOrbis.Click += ButtonSendToOrbis_Click;
            // 
            // openFileDialog
            // 
            openFileDialog.Filter = "All files|*.*";
            openFileDialog.Multiselect = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1083, 542);
            Controls.Add(buttonSelectFiles);
            Controls.Add(listFiles);
            Controls.Add(buttonSendToOrbis);
            Name = "Form1";
            Text = "Exportador de Laudos";
            ResumeLayout(false);
        }

        #endregion

        private void ButtonSelectFiles_Click(object sender, EventArgs e)
        {
            // Open file dialog to select multiple files
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Add selected files to the list box
                listFiles.Items.Clear();  // Clear the previous list
                foreach (var file in openFileDialog.FileNames)
                {
                    listFiles.Items.Add(file);  // Add file path to the list
                }

                // Enable the process button if files are selected
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
            // For now, let's show a simple message
            MessageBox.Show("Enviando os arquivos para o Orbis...");
        }
    }
}
