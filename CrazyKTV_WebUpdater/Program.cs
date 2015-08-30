using System;
using System.Windows.Forms;

namespace CrazyKTV_WebUpdater
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Environment.OSVersion.Version.Major >= 6) NativeMethods.SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    class Global
    {
        public static string WebUpdaterFile = Application.StartupPath + @"\CrazyKTV_WebUpdater.ver";
    }
}
