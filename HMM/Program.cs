using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HMM
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            void ExecuteAsAdmin()
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C rd /s /q \"C:\\Program Files\\Honeysuckle-Meadery\"&/C cd C:\\Program Files&git clone https://github.com/SlashRobotics/HMMUpdates";
                process.StartInfo = startInfo;
                process.StartInfo.Verb = "runas";
                startInfo.WorkingDirectory = @"C:\\Program Files";               
                process.Start();
            }
            try
            {
                var url = "https://github.com/SlashRobotics/Honeysuckle-Meadery/blob/main/HMM/Properties/AssemblyInfo.cs";
                var web = new HtmlWeb();
                var doc = web.Load(url);
                if (!(doc.DocumentNode.InnerHtml.ToString().Contains("[assembly: AssemblyVersion(\"1.0.0.0\")]")))
                {

                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show($"There is an update for the program. Would you like to download the update?", "Update Notice", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        new Task(() =>
                        {
                            ExecuteAsAdmin();                
                        }).Start();                       
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("Error checking for update.");
            }

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new LoginForm());
        }
    }
}
