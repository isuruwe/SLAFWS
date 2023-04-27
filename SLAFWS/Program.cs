using MapSurfer.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLAFWS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string version = MSNUtility.TryDetectInstalledVersion();  // or one can define version manually MSNUtility.SetCurrentMSNVersion(..)
            MSNUtility.SetCurrentMSNVersion("2.6");
            AssemblyLoader.AddSearchPath(Path.Combine(MSNUtility.GetMSNInstallPath(), "Studio"));
            AssemblyLoader.Register(AppDomain.CurrentDomain, version);
            Application.Run(new Form1());
        }
    }
}
