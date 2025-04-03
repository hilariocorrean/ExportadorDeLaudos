using ExportadorDeLaudos.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Windows.Forms;

namespace ExportadorDeLaudos
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var configuration = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName) // Set the base path
                                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load appsettings.json
                                        .Build();

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(configuration["OrbisSettings:URL"]!),               
            };

            // Enables visual styles and sets default font rendering to be compatible with Windows Forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(configuration, httpClient));
            //Application.Run(new Form1());
        }
    }
}