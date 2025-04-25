namespace ImportadorDeLaudos
{
    partial class ImportadorDeLaudosForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            coordenacaoPericiasVivoMortoBtn = new Button();
            coordenadoriaPsiquiatriaForenseBtn = new Button();
            tipoDocumentalLabel = new Label();
            SuspendLayout();
            // 
            // coordenacaoPericiasVivoMortoBtn
            // 
            coordenacaoPericiasVivoMortoBtn.Location = new Point(12, 48);
            coordenacaoPericiasVivoMortoBtn.Name = "coordenacaoPericiasVivoMortoBtn";
            coordenacaoPericiasVivoMortoBtn.Size = new Size(333, 32);
            coordenacaoPericiasVivoMortoBtn.TabIndex = 0;
            coordenacaoPericiasVivoMortoBtn.Text = "Coordenação de perícias no vivo e no morto";
            coordenacaoPericiasVivoMortoBtn.UseVisualStyleBackColor = true;
            coordenacaoPericiasVivoMortoBtn.Click += coordenacaoPericiasVivoMortoBtn_Click;
            // 
            // coordenadoriaPsiquiatriaForenseBtn
            // 
            coordenadoriaPsiquiatriaForenseBtn.Location = new Point(12, 86);
            coordenadoriaPsiquiatriaForenseBtn.Name = "coordenadoriaPsiquiatriaForenseBtn";
            coordenadoriaPsiquiatriaForenseBtn.Size = new Size(333, 29);
            coordenadoriaPsiquiatriaForenseBtn.TabIndex = 1;
            coordenadoriaPsiquiatriaForenseBtn.Text = "Coordenadoria de Psiquiatria Forense";
            coordenadoriaPsiquiatriaForenseBtn.UseVisualStyleBackColor = true;
            coordenadoriaPsiquiatriaForenseBtn.Click += coordenadoriaPsiquiatriaForenseBtn_Click;
            // 
            // tipoDocumentalLabel
            // 
            tipoDocumentalLabel.AutoSize = true;
            tipoDocumentalLabel.Location = new Point(12, 9);
            tipoDocumentalLabel.Name = "tipoDocumentalLabel";
            tipoDocumentalLabel.Size = new Size(276, 20);
            tipoDocumentalLabel.TabIndex = 2;
            tipoDocumentalLabel.Text = "Por favor, selecione o Tipo Documental: ";
            // 
            // ImportadorDeLaudosForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(603, 329);
            Controls.Add(tipoDocumentalLabel);
            Controls.Add(coordenadoriaPsiquiatriaForenseBtn);
            Controls.Add(coordenacaoPericiasVivoMortoBtn);
            Name = "ImportadorDeLaudosForm";
            Text = "Importador de Laudos";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button coordenacaoPericiasVivoMortoBtn;
        private Button coordenadoriaPsiquiatriaForenseBtn;
        private Label tipoDocumentalLabel;
    }
}