using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyKTV_ConfigTool
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
        public static string CrazyktvCfgFile = Application.StartupPath + @"\CrazyKTV.cfg";
        public static string CrazyktvAutoScreen = "False";
        public static string CrazyktvScreenDpi = "0";
        public static string CrazyktvWinState = "False";
        public static string CrazyktvD3DButton = "True";
        public static string CrazyktvMainScreen = "0";
        public static string CrazyktvPlayScreen = "1";
    }
}
