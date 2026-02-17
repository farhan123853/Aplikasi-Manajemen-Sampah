using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Aplikasi_Manajemen_Sampah
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                // Load Environment Variables (.env)
                Aplikasi_Manajemen_Sampah.Config.AppConfig.LoadEnv();

                ApplicationConfiguration.Initialize();
                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Critical Error: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "Application Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}