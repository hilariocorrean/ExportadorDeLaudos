using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportadorDeLaudos.Utils
{
    public static class WindowCopyableException
    {
        public static void ShowException(Exception ex)
        {
            Form errorForm = new Form
            {
                Text = "Um erro ocorreu.\nStack trace: \n",
                Width = 600,
                Height = 400
            };

            TextBox textBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Both,
                Text = ex.ToString() // includes message + stack trace
            };

            errorForm.Controls.Add(textBox);
            errorForm.ShowDialog();
        }
    }
}
