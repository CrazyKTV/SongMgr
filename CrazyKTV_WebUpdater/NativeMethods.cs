using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CrazyKTV_WebUpdater
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

    }



}
