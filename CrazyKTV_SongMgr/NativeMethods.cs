using System.Runtime.InteropServices;

namespace CrazyKTV_SongMgr
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int LCMapString(int locale, int dwMapFlags, string lpSrcStr, int cchSrc, [Out] string lpDestStr, int cchDest);
    }



}
