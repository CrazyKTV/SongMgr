using System;
using System.Collections.Generic;
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
        public static string WebUpdaterTempFile = Application.StartupPath + @"\CrazyKTV_WebUpdater.tmp";
        public static string WebUpdaterUrl = "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/CrazyKTV_WebUpdater.ver";

        public static List<List<string>> LocaleVerList = new List<List<string>>();
        public static List<List<string>> RemoteVerList = new List<List<string>>();
    }
}
