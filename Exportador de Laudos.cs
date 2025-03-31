using ExportadorDeLaudos.Repository;
using Microsoft.Extensions.Configuration;

namespace ExportadorDeLaudos
{
    public partial class Form1 : Form
    {
        private readonly IConfigurationRoot _configuration;

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(IConfigurationRoot configuration)
        {
            this._configuration = configuration;
            InitializeComponent();
            relatorioAtualizadoRepository = new RelatorioAtualizadoRepository(configuration);
        }
    }
}
