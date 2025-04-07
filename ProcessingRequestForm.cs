namespace ExportadorDeLaudos
{
    public class ProcessingRequestForm : Form
    {
        // TODO: Colocar um contador de arquivos no formato <quantidade de respostas 200>/<min(quantidade de arquivos na lista, máximo de arquivos)>
        public ProcessingRequestForm()
        {
            Text = "Aguarde";
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            waitingLabel = new Label();
            SuspendLayout();
            // 
            // waitingLabel
            // 
            waitingLabel.AutoSize = true;
            waitingLabel.Location = new Point(134, 116);
            waitingLabel.Name = "waitingLabel";
            waitingLabel.Size = new Size(165, 20);
            waitingLabel.TabIndex = 0;
            waitingLabel.Text = "Enviando para o Orbis...";
            // 
            // ProcessingRequestForm
            // 
            ClientSize = new Size(438, 253);
            Controls.Add(waitingLabel);
            Name = "ProcessingRequestForm";
            ResumeLayout(false);
            PerformLayout();

        }
        private Label waitingLabel;
    }
}
