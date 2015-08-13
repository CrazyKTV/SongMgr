using System.Runtime.InteropServices;

namespace CrazyKTV_SongMgr
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();
    }



}
