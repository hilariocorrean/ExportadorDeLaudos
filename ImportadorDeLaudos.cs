using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportadorDeLaudos
{
    public partial class ImportadorDeLaudosForm : Form
    {
        private readonly IConfigurationRoot _configuration;
        private readonly HttpClient _httpClient;
        public ImportadorDeLaudosForm(IConfigurationRoot configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            InitializeComponent();
        }

        private void coordenacaoPericiasVivoMortoBtn_Click(object sender, EventArgs e)
        {
            var window = new CoordenacaoDePericiasVivoEMortoForm(_configuration, _httpClient);
            window.ShowDialog();
        }

        private void coordenadoriaPsiquiatriaForenseBtn_Click(object sender, EventArgs e)
        {
            var window = new CoordenadoriaDePsiquiatriaForenseForm(_configuration, _httpClient);
            window.ShowDialog();
        }
    }
}
